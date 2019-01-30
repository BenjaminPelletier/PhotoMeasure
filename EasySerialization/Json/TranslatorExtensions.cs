using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasySerialization.Json
{
    /// <summary>
    /// Defines special JSON serialization techniques for specific classes
    /// </summary>
    public interface ITranslatorExtensions
    {
        JsonTranslator.JsonMaker MakeJsonMaker(Type objectType);
        JsonTranslator.ObjectMaker MakeObjectMaker(Type objectType);
    }

    class DefaultTranslatorExtensions : ITranslatorExtensions
    {
        #region Infrastructure

        private Dictionary<Type, JsonTranslator.JsonMaker> _JsonMakers = new Dictionary<Type, JsonTranslator.JsonMaker>();
        private Dictionary<Type, JsonTranslator.ObjectMaker> _ObjectMakers = new Dictionary<Type, JsonTranslator.ObjectMaker>();

        public DefaultTranslatorExtensions()
        {
            foreach (MethodInfo mi in this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                SerializedTypeAttribute st = mi.GetCustomAttribute(typeof(SerializedTypeAttribute)) as SerializedTypeAttribute;
                if (st != null)
                {
                    ParameterInfo[] args = mi.GetParameters();
                    if (args.Length != 1)
                        continue;
                    if (args[0].ParameterType == typeof(object) && mi.ReturnType == typeof(JsonObject))
                        _JsonMakers[st.Type] = (JsonTranslator.JsonMaker)Delegate.CreateDelegate(typeof(JsonTranslator.JsonMaker), mi);
                    if (args[0].ParameterType == typeof(JsonObject) && mi.ReturnType == typeof(object))
                        _ObjectMakers[st.Type] = (JsonTranslator.ObjectMaker)Delegate.CreateDelegate(typeof(JsonTranslator.ObjectMaker), mi);
                }
            }
        }

        public JsonTranslator.JsonMaker MakeJsonMaker(Type objectType)
        {
            return _JsonMakers.ContainsKey(objectType) ? _JsonMakers[objectType] : null;
        }

        public JsonTranslator.ObjectMaker MakeObjectMaker(Type objectType)
        {
            return _ObjectMakers.ContainsKey(objectType) ? _ObjectMakers[objectType] : null;
        }

        class SerializedTypeAttribute : Attribute
        {
            public Type Type;

            public SerializedTypeAttribute(Type type)
            {
                this.Type = type;
            }
        }

        private static DefaultTranslatorExtensions _Singleton = null;
        public static DefaultTranslatorExtensions Singleton
        {
            get
            {
                if (_Singleton == null)
                    _Singleton = new DefaultTranslatorExtensions();
                return _Singleton;
            }
        }

        #endregion

        #region Custom serialization routines

        #region Rectangle

        [SerializedType(typeof(Rectangle))]
        static JsonObject MakeJson_Rectangle(Object obj)
        {
            Rectangle r = (Rectangle)obj;
            return new JsonObject(r.Left + "," + r.Top + "," + r.Width + "," + r.Height);
        }

        [SerializedType(typeof(Rectangle))]
        static object MakeObject_Rectangle(JsonObject json)
        {
            if (json.ObjectType != JsonObject.Type.String)
                throw new FormatException("Expected JSON String type for .NET Rectangle but found instead " + json.ObjectType);
            int[] v = json.String.Split(',').Select(s => int.Parse(s)).ToArray();
            return new Rectangle(v[0], v[1], v[2], v[3]);
        }

        #endregion

        #region DateTime

        [SerializedType(typeof(DateTime))]
        static JsonObject MakeJson_DateTime(Object obj)
        {
            return new JsonObject(((DateTime)obj).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }

        [SerializedType(typeof(DateTime))]
        static object MakeObject_DateTime(JsonObject json)
        {
            if (json.ObjectType == JsonObject.Type.String)
                return DateTime.Parse(json.String, null, System.Globalization.DateTimeStyles.RoundtripKind).ToLocalTime(); //"2010-08-20T15:00:00Z"
            else
                throw new FormatException("Invalid JSON: Expected parseable JSON date String; instead found JSON " + json.ObjectType);
        }

        #endregion

        #region IPEndPoint

        [SerializedType(typeof(IPEndPoint))]
        static JsonObject MakeJson_IPEndPoint(Object obj)
        {
            if (obj == null)
                return JsonObject.Null;
            else
                return new JsonObject(((IPEndPoint)obj).ToString());
        }

        [SerializedType(typeof(IPEndPoint))]
        static object MakeObject_IPEndPoint(JsonObject json)
        {
            if (json.ObjectType == JsonObject.Type.Null)
                return null;
            else if (json.ObjectType == JsonObject.Type.String)
            {
                string[] ep = json.String.Split(':');
                return new IPEndPoint(IPAddress.Parse(ep[0]), int.Parse(ep[1]));
            }
            else
                throw new FormatException("Invalid JSON: Expected parseable JSON date String; instead found JSON " + json.ObjectType);
        }

        #endregion

        #region DirectoryInfo

        [SerializedType(typeof(DirectoryInfo))]
        static JsonObject MakeJson_DirectoryInfo(Object obj)
        {
            if (obj == null)
                return JsonObject.Null;
            else
                return new JsonObject(((DirectoryInfo)obj).FullName);
        }

        [SerializedType(typeof(DirectoryInfo))]
        static object MakeObject_DirectoryInfo(JsonObject json)
        {
            if (json.ObjectType == JsonObject.Type.Null)
                return null;
            else if (json.ObjectType == JsonObject.Type.String)
            {
                return new DirectoryInfo(json.String);
            }
            else
                throw new FormatException("Invalid JSON: Expected DirectoryInfo full path as JSON String; instead found JSON " + json.ObjectType);
        }

        #endregion

        #region FileInfo

        [SerializedType(typeof(FileInfo))]
        static JsonObject MakeJson_FileInfo(Object obj)
        {
            if (obj == null)
                return JsonObject.Null;
            else
                return new JsonObject(((FileInfo)obj).FullName);
        }

        [SerializedType(typeof(FileInfo))]
        static object MakeObject_FileInfo(JsonObject json)
        {
            if (json.ObjectType == JsonObject.Type.Null)
                return null;
            else if (json.ObjectType == JsonObject.Type.String)
            {
                return new FileInfo(json.String);
            }
            else
                throw new FormatException("Invalid JSON: Expected FileInfo full path as JSON String; instead found JSON " + json.ObjectType);
        }

        #endregion

        #region double[,]

        [SerializedType(typeof(double[,]))]
        static JsonObject MakeJson_Double2d(Object obj)
        {
            if (obj == null)
                return JsonObject.Null;

            double[,] matrix = (double[,])obj;

            var dict = new Dictionary<string, JsonObject>();
            dict["N"] = new JsonObject(matrix.GetLength(0));
            dict["M"] = new JsonObject(matrix.GetLength(1));
            double[] v = new double[matrix.Length];
            Buffer.BlockCopy(matrix, 0, v, 0, v.Length * sizeof(double));
            dict["V"] = new JsonObject(v.Select(m => new JsonObject(m)));
            return new JsonObject(dict);
        }

        [SerializedType(typeof(double[,]))]
        static object MakeObject_Double2d(JsonObject json)
        {
            if (json.ObjectType == JsonObject.Type.Null)
                return null;
            else if (json.ObjectType == JsonObject.Type.Dictionary)
            {
                Dictionary<string, JsonObject> jmat = json.Dictionary;
                int n = (int)jmat["N"].Number;
                int m = (int)jmat["M"].Number;
                double[] v = jmat["V"].Array.Select(j => j.Number).ToArray();
                double[,] mat = new double[n, m];
                Buffer.BlockCopy(v, 0, mat, 0, n * m * sizeof(double));
                return mat;
            }
            else
                throw new FormatException("Invalid JSON: Expected double[,] as JSON Dictionary; instead found JSON " + json.ObjectType);
        }

        #endregion

        #endregion
    }
}
