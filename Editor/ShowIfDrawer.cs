#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    /// <summary>
    /// Provides functionality for conditionally processing and monitoring serialized properties in the Unity Inspector
    /// based on custom attributes.
    /// </summary>
    /// <remarks>The <see cref="ShowIfDrawer"/> class integrates with the Unity Editor's property inspection
    /// system to enable conditional handling of properties decorated with <c>ShowIfAttribute</c> or <c>ShowIfNotAttribute</c>.
    /// It monitors properties and determines their visibility or processing state based on the values of other
    /// properties.</remarks>
    public static class ShowIfDrawer
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
        /// cref="ShowIfAttribute"/> with a matching field name.  The filtered properties are stored for monitoring
        /// purposes.</remarks>
        public static void OnInitialization()
        {
            s_monitoredProperties = new();

            InspectorHook.GetAllProperties(out var allProperties);

            foreach (var property in allProperties)
                if (InspectorHookUtilities.TryGetAttribute<ShowIfAttribute>(property, out var attribute))
                    if (attribute.FieldName.Equals(property.name))
                        s_monitoredProperties.Add(property);
        }

        /// <summary>
        /// Processes a serialized property and conditionally marks it as handled based on custom attributes.
        /// </summary>
        /// <remarks>This method evaluates the custom attributes <see cref="ShowIfAttribute"/> and <see
        /// cref="ShowIfNotAttribute"/>  applied to the property. If the conditions specified by these attributes are met,
        /// the property is  marked as handled using <see cref="InspectorHook.MarkPropertyAsHandled"/>.  - For <see
        /// cref="ShowIfAttribute"/>, the property is marked as handled if the value of the specified    source field does
        /// not match the expected value. - For <see cref="ShowIfNotAttribute"/>, the property is marked as handled if the
        /// value of the specified    source field matches the expected value.  This method relies on <see
        /// cref="InspectorHookUtilities.TryGetAttributes{T}"/> to retrieve the  attributes and <see cref="GetSourceValue"/>
        /// to resolve the source field values.</remarks>
        /// <param name="property">The <see cref="SerializedProperty"/> to process. This parameter cannot be <see langword="null"/>.</param>
        public static void OnProcessProperty(SerializedProperty property)
        {
            if (InspectorHookUtilities.TryGetAttributes<ShowIfAttribute>(property, out var ifAttributes))
                foreach (var attribute in ifAttributes)
                    foreach (var fieldValue in attribute.FieldValues)
                        if (!GetSourceValue(attribute.FieldName)?.Equals(fieldValue) ?? false)
                            switch (attribute.Visibility)
                            {
                                case Visibility.Hide:
                                    InspectorHook.MarkPropertyAndChildrenAsHandled(property);
                                    break;
                                case Visibility.Disable:
                                    InspectorHook.MarkPropertyAndChildrenDisabled(property);
                                    break;
                            }

            if (InspectorHookUtilities.TryGetAttributes<ShowIfNotAttribute>(property, out var ifNotAttributes))
                foreach (var attribute in ifNotAttributes)
                    foreach (var fieldValue in attribute.FieldValues)
                        if (GetSourceValue(attribute.FieldName)?.Equals(fieldValue) ?? false)
                            switch (attribute.Visibility)
                            {
                                case Visibility.Hide:
                                    InspectorHook.MarkPropertyAndChildrenAsHandled(property);
                                    break;
                                case Visibility.Disable:
                                    InspectorHook.MarkPropertyAndChildrenDisabled(property);
                                    break;
                            }
        }

        private static void OnProcessMethod(MethodInfo method)
        {
            if (InspectorHook.IsMethodHandled(method))
                return;

            if (InspectorHookUtilities.TryGetAttributes<ShowIfAttribute>(method, out var ifAttributes))
                foreach (var attribute in ifAttributes)
                    foreach (var fieldValue in attribute.FieldValues)
                        if (!GetSourceValue(attribute.FieldName)?.Equals(fieldValue) ?? false)
                            switch (attribute.Visibility)
                            {
                                case Visibility.Hide:
                                    InspectorHook.MarkMethodAsHandled(method);
                                    break;
                                case Visibility.Disable:
                                    InspectorHook.MarkMethodDisabled(method);
                                    break;
                            }

            if (InspectorHookUtilities.TryGetAttributes<ShowIfNotAttribute>(method, out var ifNotAttributes))
                foreach (var attribute in ifNotAttributes)
                    foreach (var fieldValue in attribute.FieldValues)
                        if (GetSourceValue(attribute.FieldName)?.Equals(fieldValue) ?? false)
                            switch (attribute.Visibility)
                            {
                                case Visibility.Hide:
                                    InspectorHook.MarkMethodAsHandled(method);
                                    break;
                                case Visibility.Disable:
                                    InspectorHook.MarkMethodDisabled(method);
                                    break;
                            }
        }

        private static object GetSourceValue(string propertyPath) =>
            InspectorHookUtilities.GetPropertyValue(InspectorHook.SerializedObject.FindProperty(propertyPath));
    }
}
#endif