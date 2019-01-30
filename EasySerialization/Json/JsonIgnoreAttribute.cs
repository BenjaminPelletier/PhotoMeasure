using System;

namespace EasySerialization.Json
{
    /// <summary>
    /// When a class field or property is decorated with this attribute, it will not be serialized to JSON, nor will JSON labeled with this field be deserialized to the object.
    /// </summary>
    public class JsonIgnoreAttribute : Attribute
    {
    }
}
