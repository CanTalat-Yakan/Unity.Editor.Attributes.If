using System;

namespace UnityEssentials
{
    public enum Visibility
    {
        Hide,
        Disable,
    }

    public class IfAttribute : Attribute
    {
        public Visibility Visibility { get; private set; }
        public string FieldName { get; private set; }
        public object[] FieldValues{ get; private set; }

        public IfAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }

        public IfAttribute(Visibility visibility, string fieldName, params object[] fieldValues)
        {
            Visibility = visibility;
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }

    public class IfNotAttribute : Attribute
    {
        public Visibility Visibility { get; private set; }
        public string FieldName { get; private set; }
        public object[] FieldValues { get; private set; }

        public IfNotAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }

        public IfNotAttribute(Visibility visibility, string fieldName, params object[] fieldValues)
        {
            Visibility = visibility;
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }
}
