using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor
{
    public static class AbilityEditorUtility
    {
        public static readonly Type[]   AllTypes;
        public static readonly string[] AllVariables;

        private static readonly object[]                      m_TempArray        = new object[1];
        private static readonly Dictionary<Type, Type[]>      m_ImplementTypeMap = new Dictionary<Type, Type[]>();
        private static readonly Dictionary<Type, Attribute[]> m_AttributeMap     = new Dictionary<Type, Attribute[]>();
        private static readonly StringBuilder                 m_StringBuilder    = new StringBuilder();

        static AbilityEditorUtility()
        {
            List<Type> allTypes            = new List<Type>();
            Assembly[] assemblyArray       = AppDomain.CurrentDomain.GetAssemblies();
            int        assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                allTypes.AddRange(assemblyArray[i].GetTypes());
            }

            AllTypes     = allTypes.ToArray();
            AllVariables = GetAllVariables();

            string[] GetAllVariables()
            {
                HashSet<string> variables = new HashSet<string>();
                foreach (Type type in AllTypes)
                {
                    Collect(type);
                }

                return variables.ToArray();

                void Collect(Type type)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].FieldType == typeof(AbilityVariable))
                        {
                            variables.Add(GetVariableName(fields[i].Name));
                        }
                    }
                }
            }
        }

        public static StringBuilder GetSB()
        {
            m_StringBuilder.Clear();
            return m_StringBuilder;
        }

        public static Type GetType(string typeName)
        {
            return AllTypes.FirstOrDefault(x => x.FullName == typeName || x.Name == typeName);
        }

        public static Type[] GetImplementTypes(this Type parent)
        {
            if (m_ImplementTypeMap.TryGetValue(parent, out Type[] implements) == false)
            {
                implements = AllTypes.Where(IsImplementType).ToArray();
                m_ImplementTypeMap.Add(parent, implements);
            }

            return implements;

            bool IsImplementType(Type type)
            {
                return type.IsAbstract == false && (type.IsSubclassOf(parent) || parent.IsAssignableFrom(type));
            }
        }

        public static void ShowImplementSelector(this Type parent, Action<IEnumerable<Type>> onSelected)
        {
            GenericSelector<Type> selector = new GenericSelector<Type>(null, false, parent.GetImplementTypes().Select(GetItem));
            selector.EnableSingleClickToSelect();
            selector.SelectionConfirmed += onSelected;
            selector.ShowInPopup();

            GenericSelectorItem<Type> GetItem(Type type)
            {
                string text = type.GetAttribute<DescriptionAttribute>()?.Name ?? type.Name;
                return new GenericSelectorItem<Type>(text, type);
            }
        }

        public static object[] TempArray(this object obj)
        {
            m_TempArray[0] = obj;
            return m_TempArray;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            if (m_AttributeMap.TryGetValue(type, out Attribute[] attributes) == false)
            {
                attributes = type.GetAttributes();
                m_AttributeMap.Add(type, attributes);
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is T result)
                {
                    return result;
                }
            }

            return null;
        }

        public static string GetName(this Type type)
        {
            return type.GetAttribute<DescriptionAttribute>().Name;
        }

        public static GUIContent GetDescription(this Type type)
        {
            DescriptionAttribute attribute = type.GetAttribute<DescriptionAttribute>();
            GUIContent           content   = new GUIContent();
            content.text    = attribute?.Name ?? type.Name;
            content.tooltip = attribute?.ToolTip;
            return content;
        }

        public static string GetName(this InspectorProperty property)
        {
            return property.GetAttribute<DescriptionAttribute>().Name;
        }

        public static T GetState<T>(this InspectorProperty property, string key, T defaultValue = default)
        {
            if (property.State.Exists(key))
            {
                return property.State.Get<T>(key);
            }

            return defaultValue;
        }

        public static void SetState<T>(this InspectorProperty property, string key, T value)
        {
            if (property.State.Exists(key))
            {
                property.State.Set(key, value);
            }
            else
            {
                property.State.Create(key, false, value);
            }
        }

        public static GUIContent GetDescription(this InspectorProperty property)
        {
            DescriptionAttribute attribute = property.GetAttribute<DescriptionAttribute>();
            GUIContent           content   = new GUIContent();
            content.text    = attribute.Name;
            content.tooltip = attribute.ToolTip;
            return content;
        }

        public static T As<T>(this object obj)
        {
            return (T)obj;
        }

        public static InspectorProperty FindChild(this InspectorProperty property, string name)
        {
            string path = property.Path + "." + name;
            return property.FindChild(x => x.Path == path, false);
        }

        public static T GetValue<T>(this InspectorProperty property)
        {
            return property?.ValueEntry?.WeakSmartValue is T tValue ? tValue : default;
        }

        public static void SetValue(this InspectorProperty property, object value)
        {
            if (property?.ValueEntry != null)
                property.ValueEntry.WeakSmartValue = value;
        }

        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            Type type = obj?.GetType();
            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(obj) is T result ? result : default;
                }

                type = type.BaseType;
            }

            return default;
        }

        public static void SetFieldValue(this object obj, string fieldName, object value)
        {
            Type type = obj?.GetType();
            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(obj, value);
                    break;
                }

                type = type.BaseType;
            }
        }

        public static List<AbilityVariable> GetAbilityVariables(Ability ability)
        {
            List<AbilityVariable> variables = new List<AbilityVariable>();
            PropertyTree          tree      = PropertyTree.Create(new SerializedObject(ability));
            CollectVariables(tree.RootProperty);
            tree.Dispose();
            return variables;

            void CollectVariables(InspectorProperty property)
            {
                AbilityVariable variable = property.GetValue<AbilityVariable>();
                if (variable != null)
                {
                    variables.Add(variable);
                }

                foreach (var child in property.Children)
                {
                    CollectVariables(child);
                }
            }
        }

        public static string GetVariableName(this InspectorProperty variable)
        {
            if (variable.Info.TypeOfValue != typeof(AbilityVariable))
            {
                throw new ArgumentException();
            }

            VariableNameAttribute variableNameAttribute = variable.GetAttribute<VariableNameAttribute>();
            if (variableNameAttribute == null)
            {
                return GetVariableName(variable.Name);
            }

            InspectorProperty nameField = variable.ParentValueProperty.FindChild(variableNameAttribute.Name);
            if (nameField != null && nameField.Info.TypeOfValue == typeof(string))
            {
                return nameField.GetValue<string>();
            }

            Debug.LogError($"无法找到匹配的变量{variableNameAttribute.Name} => {variable.UnityPropertyPath}");
            return GetVariableName(variable.Name);
        }

        public static string GetVariableName(string filedName)
        {
            return AbilityVariable.GetVariableName(filedName, 0);
        }

        public static string MakeUniqueVariableName(HashSet<string> variableNames, string inputName)
        {
            if (variableNames.Contains(inputName) == false)
            {
                return inputName;
            }

            for (int i = 1; i < 100; i++)
            {
                string temp = AbilityVariable.GetVariableName(inputName, i);
                if (variableNames.Contains(temp) == false)
                {
                    return temp;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}