using System;

namespace UnityEssentials
{
    public class IfAttribute : Attribute
    {
        public readonly string FieldName;
        public readonly object FieldValue;

        public IfAttribute(string fieldName, object fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
    }
}
