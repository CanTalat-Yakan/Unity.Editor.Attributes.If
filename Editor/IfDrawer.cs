#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace UnityEssentials
{
    /// <summary>
    /// Provides functionality for conditionally processing and monitoring serialized properties in the Unity Inspector
    /// based on custom attributes.
    /// </summary>
    /// <remarks>The <see cref="IfDrawer"/> class integrates with the Unity Editor's property inspection
    /// system to enable conditional handling of properties decorated with <c>IfAttribute</c> or <c>IfNotAttribute</c>.
    /// It monitors properties and determines their visibility or processing state based on the values of other
    /// properties.</remarks>
    public static class IfDrawer
    {
        private static List<SerializedProperty> s_monitoredProperties;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            InspectorHook.AddInitialization(OnInitialization);
            InspectorHook.AddProcessProperty(OnProcessProperty, 1001);
            InspectorHook.AddProcessMethod(OnProcessMethod, 1001);
        }

        /// <summary>
        /// Initializes the monitored properties by inspecting and filtering properties with specific attributes.
        /// </summary>
        /// <remarks>This method retrieves all properties using the <see
        /// cref="InspectorHook.GetAllProperties"/> method  and filters them based on the presence of an <see
        /// cref="IfAttribute"/> with a matching field name.  The filtered properties are stored for monitoring
        /// purposes.</remarks>
        public static void OnInitialization()
        {
            s_monitoredProperties = new();

            InspectorHook.GetAllProperties(out var allProperties);

            foreach (var property in allProperties)
                if (InspectorHookUtilities.TryGetAttribute<IfAttribute>(property, out var attribute))
                    if (attribute.FieldName.Equals(property.name))
                        s_monitoredProperties.Add(property);
        }

        /// <summary>
        /// Processes a serialized property and conditionally marks it as handled based on custom attributes.
        /// </summary>
        /// <remarks>This method evaluates the custom attributes <see cref="IfAttribute"/> and <see
        /// cref="IfNotAttribute"/>  applied to the property. If the conditions specified by these attributes are met,
        /// the property is  marked as handled using <see cref="InspectorHook.MarkPropertyAsHandled"/>.  - For <see
        /// cref="IfAttribute"/>, the property is marked as handled if the value of the specified    source field does
        /// not match the expected value. - For <see cref="IfNotAttribute"/>, the property is marked as handled if the
        /// value of the specified    source field matches the expected value.  This method relies on <see
        /// cref="InspectorHookUtilities.TryGetAttributes{T}"/> to retrieve the  attributes and <see cref="GetSourceValue"/>
        /// to resolve the source field values.</remarks>
        /// <param name="property">The <see cref="SerializedProperty"/> to process. This parameter cannot be <see langword="null"/>.</param>
        public static void OnProcessProperty(SerializedProperty property)
        {
            if (InspectorHookUtilities.TryGetAttributes<IfAttribute>(property, out var ifAttributes))
                foreach (var attribute in ifAttributes)
                    if (!GetSourceValue(attribute.FieldName)?.Equals(attribute.FieldValue) ?? false)
                        InspectorHook.MarkPropertyAndChildrenAsHandled(property);

            if (InspectorHookUtilities.TryGetAttributes<IfNotAttribute>(property, out var ifNotAttributes))
                foreach (var attribute in ifNotAttributes)
                    if (GetSourceValue(attribute.FieldName)?.Equals(attribute.FieldValue) ?? false)
                        InspectorHook.MarkPropertyAndChildrenAsHandled(property);
        }

        private static void OnProcessMethod(MethodInfo method)
        {
            if (InspectorHook.IsMethodHandled(method))
                return;

            if (InspectorHookUtilities.TryGetAttributes<IfAttribute>(method, out var ifAttributes))
                foreach (var attribute in ifAttributes)
                    if (!GetSourceValue(attribute.FieldName)?.Equals(attribute.FieldValue) ?? false)
                        InspectorHook.MarkPropertyAsHandled(method);

            if (InspectorHookUtilities.TryGetAttributes<IfNotAttribute>(method, out var ifNotAttributes))
                foreach (var attribute in ifNotAttributes)
                    if (GetSourceValue(attribute.FieldName)?.Equals(attribute.FieldValue) ?? false)
                        InspectorHook.MarkPropertyAsHandled(method);
        }

        private static object GetSourceValue(string propertyPath) =>
            InspectorHookUtilities.GetPropertyValue(InspectorHook.SerializedObject.FindProperty(propertyPath));
    }
}
#endif