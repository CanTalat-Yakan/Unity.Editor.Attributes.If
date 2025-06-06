using System;

namespace UnityEssentials
{
    public class IfNotAttribute : Attribute
    {
        public readonly string FieldName;
        public readonly object[] FieldValues;

        public IfNotAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }
}
