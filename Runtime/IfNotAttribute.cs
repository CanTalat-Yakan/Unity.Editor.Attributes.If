using System;

namespace UnityEssentials
{
    public class IfNotAttribute : Attribute
    {
        public readonly string FieldName;
        public readonly object FieldValue;

        public IfNotAttribute(string fieldName, object fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
    }
}
