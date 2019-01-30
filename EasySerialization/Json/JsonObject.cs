using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EasySerialization.Json
{
    public class JsonObject
    {
        public enum Type
        {
            String,
            Number,
            Dictionary,
            Array,
            Boolean,
            Null
        }

        public Type ObjectType;
        public object Value;

        #region Constructors

        public JsonObject(string value)
        {
            ObjectType = Type.String;
            Value = value;
        }

        public JsonObject(double value)
        {
            ObjectType = Type.Number;
            Value = value;
        }

        public JsonObject(double? value)
        {
            if (value.HasValue)
            {
                ObjectType = Type.Number;
                Value = value.Value;
            }
            else
            {
                ObjectType = Type.Null;
            }
        }

        public JsonObject(int value) : this((double)value) { }
        public JsonObject(int? value) : this((double?)(value.HasValue ? (double?)value.Value : null)) { }
        public JsonObject(float value) : this((double)value) { }
        public JsonObject(float? value) : this((double?)(value.HasValue ? (double?)value.Value : null)) { }

        public JsonObject(Dictionary<string, JsonObject> dictionary)
        {
            ObjectType = Type.Dictionary;
            Value = dictionary;
        }

        public JsonObject(string paramName, JsonObject paramValue)
        {
            ObjectType = Type.Dictionary;
            Value = new Dictionary<string, JsonObject>() { { paramName, paramValue } };
        }

        public JsonObject(IEnumerable<JsonObject> objects)
        {
            ObjectType = Type.Array;
            Value = objects.ToArray();
        }

        public JsonObject(bool value)
        {
            ObjectType = Type.Boolean;
            Value = value;
        }

        public static readonly JsonObject Null = new JsonObject();

        private JsonObject()
        {
            ObjectType = Type.Null;
        }

        public static JsonObject EmptyDictionary { get { return new JsonObject(new Dictionary<string, JsonObject>()); } }

        #endregion

        #region Explicit value accessors

        public Dictionary<string, JsonObject> Dictionary
        {
            get
            {
                if (ObjectType == Type.Dictionary)
                    return Value as Dictionary<string, JsonObject>;
                else if (ObjectType == Type.Null)
                    return null;
                else
                    throw new InvalidOperationException("Json.Object is not a key-value-pair container");
            }
        }

        public string String
        {
            get
            {
                if (ObjectType == Type.String)
                    return Value as string;
                else if (ObjectType == Type.Null)
                    return null;
                else
                    throw new InvalidOperationException("Json.Object is not a string");
            }
        }

        public double Number
        {
            get
            {
                if (ObjectType != Type.Number)
                    throw new InvalidOperationException("Json.Object is not a number");
                return (double)Value;
            }
        }

        public bool Boolean
        {
            get
            {
                if (ObjectType != Type.Boolean)
                    throw new InvalidOperationException("Json.Object is not a boolean");
                return (bool)Value;
            }
        }

        public JsonObject[] Array
        {
            get
            {
                if (ObjectType != Type.Array)
                    throw new InvalidOperationException("Json.Object is not an array");
                return (JsonObject[])Value;
            }
        }

        #endregion

        public JsonObject this[string key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        public override string ToString()
        {
            if (ObjectType == Type.String)
                return "\"" + (Value as string).Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            else if (ObjectType == Type.Number)
                return Value.ToString();
            else if (ObjectType == Type.Dictionary)
            {
                Dictionary<string, JsonObject> dictionary = Value as Dictionary<string, JsonObject>;
                if (dictionary.Count == 0)
                    return "{}";
                else
                    return "{" + dictionary.Select(kvp => "\"" + kvp.Key + "\":" + kvp.Value).Aggregate((a, b) => a + "," + b) + "}";
            }
            else if (ObjectType == Type.Array)
            {
                JsonObject[] array = Value as JsonObject[];
                if (array.Length == 0)
                    return "[]";
                else
                    return "[" + array.Select(v => v.ToString()).Aggregate((a, b) => a + "," + b) + "]";
            }
            else if (ObjectType == Type.Boolean)
                return (bool)Value ? "true" : "false";
            else if (ObjectType == Type.Null)
                return "null";
            else
                throw new NotImplementedException();
        }

        public string ToMultilineString(int indentIncrement = 2, int currentIndent = 0)
        {
            if (ObjectType == Type.String)
                return "\"" + (Value as string).Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            else if (ObjectType == Type.Number)
                return Value.ToString();
            else if (ObjectType == Type.Dictionary)
            {
                var sb = new StringBuilder();
                sb.AppendLine("{");
                var dictionary = Value as Dictionary<string, JsonObject>;
                int n = dictionary.Count;
                foreach (var kvp in dictionary)
                {
                    sb.Append(new string(' ', currentIndent + indentIncrement));
                    sb.Append('"');
                    sb.Append(kvp.Key);
                    sb.Append("\": ");
                    sb.Append(kvp.Value.ToMultilineString(indentIncrement, currentIndent + indentIncrement));
                    sb.AppendLine(--n > 0 ? "," : "");
                }
                sb.Append(new string(' ', currentIndent));
                sb.Append("}");
                return sb.ToString();
            }
            else if (ObjectType == Type.Array)
            {
                var sb = new StringBuilder();
                sb.Append(new string(' ', currentIndent));
                sb.AppendLine("[");
                var array = Value as JsonObject[];
                int n = array.Length;
                foreach (var v in array)
                {
                    sb.Append(new string(' ', currentIndent + indentIncrement));
                    sb.Append(v.ToMultilineString(indentIncrement, currentIndent + indentIncrement));
                    sb.AppendLine(--n > 0 ? "," : "");
                }
                sb.Append(new string(' ', currentIndent));
                sb.Append("]");
                return sb.ToString();
            }
            else if (ObjectType == Type.Boolean)
                return (bool)Value ? "true" : "false";
            else if (ObjectType == Type.Null)
                return "null";
            else
                throw new NotImplementedException();
        }

        public static JsonObject Parse(string json)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(json);
            using (var ms = new MemoryStream(buffer))
            {
                var parser = new JsonStreamParser(ms, buffer.Length);
                JsonObject obj = parser.ReadObject();
                if (parser.Cursor != buffer.Length)
                    throw new FormatException("JSON string contains multiple objects");
                return obj;
            }
        }
    }
}
