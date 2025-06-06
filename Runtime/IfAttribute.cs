using System;

namespace UnityEssentials
{
    public class IfAttribute : Attribute
    {
        public readonly string FieldName;
        public readonly object[] FieldValues;

        public IfAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }
}
