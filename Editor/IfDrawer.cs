#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public static class IfDrawer
    {
        private static List<SerializedProperty> s_monitoredProperties;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            InspectorHook.AddInitialization(OnInitialization);
            InspectorHook.AddProcessProperty(OnProcessProperty, 1001);
        }

        public static void OnInitialization()
        {
            s_monitoredProperties = new();

            InspectorHook.GetAllProperties(out var allProperties);

            foreach (var property in allProperties)
                if (InspectorHookUtilities.TryGetAttribute<IfAttribute>(property, out var attribute))
                    if (attribute.FieldName.Equals(property.name))
                        s_monitoredProperties.Add(property);
        }

        public static void OnProcessProperty(SerializedProperty property)
        {
            if (!InspectorHookUtilities.TryGetAttribute<IfAttribute>(property, out var attribute))
                return;

            var source = InspectorHookUtilities.GetPropertyValue(InspectorHook.SerializedObject.FindProperty(attribute.FieldName));
            if (!source.Equals(attribute.FieldValue))
                InspectorHook.MarkPropertyAsHandled(property.propertyPath);
        }
    }
}
#endif