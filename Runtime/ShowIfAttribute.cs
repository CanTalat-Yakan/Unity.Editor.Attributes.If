using System;

namespace UnityEssentials
{
    public enum Visibility
    {
        Hide,
        Disable,
    }

    public class ShowIfAttribute : Attribute
    {
        public Visibility Visibility { get; private set; }
        public string FieldName { get; private set; }
        public object[] FieldValues{ get; private set; }

        public ShowIfAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }

        public ShowIfAttribute(Visibility visibility, string fieldName, params object[] fieldValues)
        {
            Visibility = visibility;
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }

    public class ShowIfNotAttribute : Attribute
    {
        public Visibility Visibility { get; private set; }
        public string FieldName { get; private set; }
        public object[] FieldValues { get; private set; }

        public ShowIfNotAttribute(string fieldName, params object[] fieldValues)
        {
            FieldName = fieldName;
            FieldValues = fieldValues;
        }

        public ShowIfNotAttribute(Visibility visibility, string fieldName, params object[] fieldValues)
        {
            Visibility = visibility;
            FieldName = fieldName;
            FieldValues = fieldValues;
        }
    }
}
