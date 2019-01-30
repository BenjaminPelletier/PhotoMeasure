using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace EasySerialization.Json
{
    /// <summary>
    /// Translates between native .NET objects and JsonObjects
    /// </summary>
    public class JsonTranslator
    {
        private static readonly Dictionary<Type, JsonObject.Type[]> SIMPLE_TYPES = new Dictionary<Type, JsonObject.Type[]>()
        {
            { typeof(bool), new JsonObject.Type[] { JsonObject.Type.Boolean } },
            { typeof(int), new JsonObject.Type[] { JsonObject.Type.Number } },
            { typeof(float), new JsonObject.Type[] { JsonObject.Type.Number } },
            { typeof(double), new JsonObject.Type[] { JsonObject.Type.Number } },
        };

        private ITranslatorExtensions _TranslatorExtensions;

        public JsonTranslator(ITranslatorExtensions customTranslators = null)
        {
            if (customTranslators == null)
                customTranslators = DefaultTranslatorExtensions.Singleton;

            _TranslatorExtensions = customTranslators;
        }

        private string AdjustedFieldName(string rawName)
        {
            if (rawName.EndsWith("k__BackingField"))
                return rawName.Substring(1, rawName.Length - "k__BackingField".Length - 2);
            else
                return rawName;
        }

        #region JSON to Object

        public delegate object ObjectMaker(JsonObject json);
        private Dictionary<Type, ObjectMaker> _ObjectMakers = new Dictionary<Type, ObjectMaker>();

        private delegate void MemberSetter(object obj, object value);

        private ObjectMaker GetObjectMaker(Type objectType)
        {
            if (!_ObjectMakers.ContainsKey(objectType))
            {
                _ObjectMakers[objectType] = MakeObjectMaker(objectType);
            }
            return _ObjectMakers[objectType];
        }

        private ObjectMaker MakeObjectMaker(Type objectType)
        {
            // Specific custom deserialization
            ObjectMaker customMaker = _TranslatorExtensions?.MakeObjectMaker(objectType);
            if (customMaker != null)
                return customMaker;

            // String
            if (objectType == typeof(string))
            {
                return json =>
                {
                    if (json.ObjectType == JsonObject.Type.Null)
                        return null;
                    else
                        return json.String;
                };
            }

            // Nullable types
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                ObjectMaker subObjectMaker = MakeObjectMaker(objectType.GetGenericArguments()[0]);
                return json =>
                {
                    if (json.ObjectType == JsonObject.Type.Null)
                        return null;
                    else
                        return subObjectMaker(json);
                };
            }

            // Enums
            if (objectType.IsEnum)
            {
                return json =>
                {
                    if (json.ObjectType != JsonObject.Type.String)
                        throw new FormatException("Invalid JSON: Expected JSON String for .NET Enum type " + objectType.FullName + " but found instead " + json.ObjectType);
                    return Enum.Parse(objectType, json.String);
                };
            }

            // Simple types
            if (SIMPLE_TYPES.ContainsKey(objectType))
            {
                JsonObject.Type[] allowableTypes = SIMPLE_TYPES[objectType];
                return json =>
                {
                    if (allowableTypes.Contains(json.ObjectType))
                        return JsonReflection.Cast(json.Value, objectType);
                    else
                        throw new FormatException("Invalid JSON: Expected JSON " + allowableTypes.Select(t => t.ToString()).Aggregate((a, b) => a + " or " + b) + " for .NET " + objectType.Name + "; instead found JSON " + json.ObjectType);
                };
            }

            // Explicit Dictionary
            JsonReflection.DictionaryInfo dinfo = JsonReflection.IsDictionary(objectType);
            if (dinfo != null)
            {
                Func<object> dictConstructor = JsonReflection.GetDefaultMaker(objectType);
                Func<string, object> keyParser = JsonReflection.GetParser(dinfo.KeyType);
                if (keyParser == null)
                    throw new InvalidOperationException("Cannot create JsonMaker for type " + objectType.FullName + " because key type " + dinfo.KeyType.FullName + " cannot be parsed from a string");
                ObjectMaker valueMaker = GetObjectMaker(dinfo.ValueType);
                MethodInfo add = objectType.GetMethod("Add", new Type[] { dinfo.KeyType, dinfo.ValueType });

                return json =>
                {
                    if (json.ObjectType == JsonObject.Type.Null)
                        return null;
                    var result = dictConstructor();
                    foreach (var kvp in json.Dictionary)
                        add.Invoke(result, new object[] { keyParser(kvp.Key), valueMaker(kvp.Value) });
                    return result;
                };
            }

            // Enumerable types
            if (typeof(IEnumerable).IsAssignableFrom(objectType))
            {
                // Arrays
                if (typeof(Array).IsAssignableFrom(objectType))
                {
                    Type itemType = objectType.GetElementType();
                    return json =>
                    {
                        if (json.ObjectType == JsonObject.Type.Null)
                            return null;
                        if (json.ObjectType != JsonObject.Type.Array)
                            throw new FormatException("Invalid JSON: Expected JSON Array for .NET type " + objectType.FullName + " but found instead " + json.ObjectType);
                        JsonObject[] jitems = json.Array;
                        Array result = (Array)Activator.CreateInstance(objectType, new object[] { jitems.Length });
                        ObjectMaker itemMaker = GetObjectMaker(itemType);
                        int i = 0;
                        foreach (JsonObject jitem in jitems)
                        {
                            result.SetValue(itemMaker(jitem), i);
                            i++;
                        }
                        return result;
                    };
                }
                
                // Other enumerable classes
                Func<object> enumConstructor = JsonReflection.GetDefaultMaker(objectType);
                if (enumConstructor == null)
                    throw new InvalidOperationException("Cannot create JSON Array ObjectMaker for type " + objectType.FullName + " because it does not have a default constructor");
                Type contentType = JsonReflection.GetEnumerableType(objectType);
                MethodInfo addMethod = objectType.GetMethod("Add", new Type[] { contentType });
                if (enumConstructor == null)
                    throw new InvalidOperationException("Cannot create JSON Array ObjectMaker for type " + objectType.FullName + " because it does not have an Add method");

                return json =>
                {
                    if (json.ObjectType == JsonObject.Type.Null)
                        return null;
                    ObjectMaker itemMaker = GetObjectMaker(contentType);
                    object result = enumConstructor();
                    foreach (JsonObject jitem in json.Array)
                    {
                        addMethod.Invoke(result, new object[] { itemMaker(jitem) });
                    }
                    return result;
                };
            }

            // Parseable from string
            Func<string, object> parser = JsonReflection.GetParser(objectType);
            if (parser != null)
            {
                return json =>
                {
                    if (json.ObjectType == JsonObject.Type.String || json.ObjectType == JsonObject.Type.Null)
                        return parser(json.String);
                    else
                        throw new FormatException("Invalid JSON: Expected parseable JSON String for .NET type " + objectType.FullName + "; instead found JSON " + json.ObjectType);
                };
            }

            // Object-as-Dictionary (choice of last resort)
            Func<object> ci = JsonReflection.GetDefaultMaker(objectType);
            if (ci == null)
                throw new InvalidOperationException("Cannot create JSON Dictionary ObjectMaker for type " + objectType.FullName + " because a default object cannot be created");

            var setters = new Dictionary<string, MemberSetter>();
            var types = new Dictionary<string, Type>();

            FieldInfo[] fields = objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                if (fi.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                string name = AdjustedFieldName(fi.Name);

                types[name] = fi.FieldType;
                setters[name] = (obj, value) => fi.SetValue(obj, value);
            }

            //PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //foreach (PropertyInfo pi in properties)
            //{
            //    if (pi.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            //        continue;

            //    throw new NotImplementedException("Property serialization not yet supported");
            //}

            return json =>
            {
                if (json.ObjectType != JsonObject.Type.Dictionary)
                    throw new FormatException("Invalid JSON: Expected JSON Dictionary for .NET type " + objectType.FullName + "; instead found JSON " + json.ObjectType);
                object result = ci();
                Dictionary<string, JsonObject> jfields = json.Dictionary;
                foreach (var kvp in jfields)
                {
                    if (!setters.ContainsKey(kvp.Key))
                        throw new FormatException("No field named '" + kvp.Key + "' found in .NET type " + objectType.FullName);
                    ObjectMaker maker = GetObjectMaker(types[kvp.Key]);
                    setters[kvp.Key](result, maker(kvp.Value));
                }
                return result;
            };
        }

        #endregion

        #region Object to JSON

        public delegate JsonObject JsonMaker(object obj);
        private Dictionary<Type, JsonMaker> _JsonMakers = new Dictionary<Type, JsonMaker>();

        private delegate object MemberGetter(object obj);

        private JsonMaker GetJsonMaker(Type objectType)
        {
            if (!_JsonMakers.ContainsKey(objectType))
            {
                _JsonMakers[objectType] = MakeJsonMaker(objectType);
            }
            return _JsonMakers[objectType];
        }

        private JsonMaker MakeJsonMaker(Type objectType)
        {
            // Specific custom deserialization
            JsonMaker customMaker = _TranslatorExtensions?.MakeJsonMaker(objectType);
            if (customMaker != null)
                return customMaker;

            // String
            if (objectType == typeof(string))
            {
                return obj =>
                {
                    if (obj == null)
                        return JsonObject.Null;
                    else
                        return new JsonObject(obj as string);
                };
            }

            // Nullable types
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                JsonMaker subObjectMaker = MakeJsonMaker(objectType.GetGenericArguments()[0]);
                return obj =>
                {
                    if (obj == null)
                        return JsonObject.Null;
                    else
                        return subObjectMaker(obj);
                };
            }

            // Enums
            if (objectType.IsEnum)
            {
                return obj => new JsonObject(obj.ToString());
            }

            // Simple types
            foreach (var kvp in SIMPLE_TYPES)
            {
                if (objectType == kvp.Key)
                {
                    ConstructorInfo jtc = typeof(JsonObject).GetConstructor(new Type[] { kvp.Key });
                    return obj => (JsonObject)jtc.Invoke(new object[] { obj });
                }
            }

            // Parseable from string
            Func<string, object> parser = JsonReflection.GetParser(objectType);
            if (parser != null)
            {
                Func<object, string> toString = JsonReflection.GetToString(objectType);
                return obj => new JsonObject(toString(obj));
            }

            // Explicit Dictionary
            JsonReflection.DictionaryInfo dinfo = JsonReflection.IsDictionary(objectType);
            if (dinfo != null)
            {
                if (JsonReflection.GetParser(dinfo.KeyType) == null)
                    throw new InvalidOperationException("Cannot create JsonMaker for type " + objectType.FullName + " because key type " + dinfo.KeyType.FullName + " cannot be parsed from a string");
                JsonMaker valueMaker = GetJsonMaker(dinfo.ValueType);
                Type kvpType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { dinfo.KeyType, dinfo.ValueType });
                PropertyInfo keyProperty = kvpType.GetProperty("Key");
                PropertyInfo valueProperty = kvpType.GetProperty("Value");

                return obj =>
                {
                    if (obj == null)
                        return JsonObject.Null;
                    var result = JsonObject.EmptyDictionary;
                    foreach (var kvp in obj as IEnumerable)
                        result[keyProperty.GetValue(kvp).ToString()] = valueMaker(valueProperty.GetValue(kvp));
                    return result;
                };
            }

            // Array
            Type contentType = JsonReflection.GetEnumerableType(objectType);
            if (contentType != null)
            {
                return obj =>
                {
                    if (obj == null)
                        return JsonObject.Null;
                    JsonMaker itemMaker = GetJsonMaker(contentType);
                    var items = new List<JsonObject>();
                    foreach (object item in (IEnumerable)obj)
                    {
                        items.Add(itemMaker(item));
                    }
                    return new JsonObject(items);
                };
            }

            // Object-as-Dictionary (choice of last resort)
            object defaultObj = JsonReflection.GetDefaultMaker(objectType)();

            var getters = new Dictionary<string, MemberGetter>();
            var types = new Dictionary<string, Type>();
            var equalities = new Dictionary<string, MethodInfo>();

            FieldInfo[] fields = objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                if (fi.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                string name = AdjustedFieldName(fi.Name);

                getters[name] = obj => fi.GetValue(obj);
                types[name] = fi.FieldType;
                equalities[name] = fi.FieldType.GetMethod("Equals", new Type[] { typeof(object) });
            }

            //PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //foreach (PropertyInfo pi in properties)
            //{
            //    if (pi.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            //        continue;

            //    throw new NotImplementedException("Property serialization not yet supported");
            //}

            return obj =>
            {
                var jfields = new Dictionary<string, JsonObject>();
                
                foreach (var kvp in getters)
                {
                    object objVal = kvp.Value(obj);
                    object defVal = kvp.Value(defaultObj);
                    MethodInfo equality = equalities[kvp.Key];
                    bool equal = defVal == null ? objVal == null : (bool)equality.Invoke(defVal, new object[] { objVal });
                    if (!equal)
                    {
                        jfields[kvp.Key] = GetJsonMaker(types[kvp.Key])(objVal);
                    }
                }

                return new JsonObject(jfields);
            };
        }

        #endregion

        #region Public accessors

        public T MakeObject<T>(JsonObject json)
        {
            return (T)GetObjectMaker(typeof(T))(json);
        }

        public object MakeObject(JsonObject json, Type t)
        {
            if (t != null)
                return GetObjectMaker(t)(json);
            else
                return null;
        }

        public JsonObject MakeJson<T>(T obj)
        {
            return GetJsonMaker(typeof(T))(obj);
        }

        public JsonObject MakeJson(object obj, Type t)
        {
            if (t != null)
                return GetJsonMaker(t)(obj);
            else
                return JsonObject.Null;
        }

        #endregion
    }
}
