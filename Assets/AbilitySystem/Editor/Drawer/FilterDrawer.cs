using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    class FilterDrawer<T> : OdinValueDrawer<Filter<T>>
    {
        private const string ItemFieldName    = "m_Item";
        private const string ValueFieldName   = "m_Value";
        private const string FilterFieldName  = "m_Filter";
        private const string ReverseFieldName = "m_Reverse";
        private const string ItemsFieldName   = "m_Items";
        private const string AndGroupsName    = "m_AndGroups";
        private const string OrGroupsName     = "m_OrGroups";

        private InspectorProperty m_ItemProperty;
        private InspectorProperty m_ValueProperty;
        private InspectorProperty m_FilterProperty;
        private InspectorProperty m_AndGroupsProperty;
        private InspectorProperty m_OrGroupsProperty;

        protected override void Initialize()
        {
            base.Initialize();

            m_ItemProperty      = Property.FindChild(ItemFieldName);
            m_ValueProperty     = m_ItemProperty.FindChild(ValueFieldName);
            m_FilterProperty    = m_ItemProperty.FindChild(FilterFieldName);
            m_AndGroupsProperty = Property.FindChild(AndGroupsName);
            m_OrGroupsProperty  = Property.FindChild(OrGroupsName);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginBox();
            GUILayout.BeginVertical();
            SirenixEditorGUI.Title(label.text, Property.GetValue<object>()?.ToString(), TextAlignment.Left, false, false);
            GUILayout.EndVertical();

            if (m_FilterProperty.ValueEntry.WeakSmartValue == null && GUI.Button(GUILayoutUtility.GetLastRect(), String.Empty, GUIStyle.none))
            {
                ShowSelector(x =>
                {
                    m_ItemProperty.SetValue(x);
                    Property.Tree.ApplyChanges();
                });
            }

            if (m_FilterProperty.ValueEntry.WeakSmartValue != null)
            {
                DrawItem(m_ItemProperty);
            }

            SirenixEditorGUI.EndBox();
        }

        private void ShowSelector(Action<Filter<T>.Item> onSelect)
        {
            IEnumerable<GenericSelectorItem<Type>> types = typeof(IFilter<T>)
                .GetImplementTypes()
                .Append(typeof(Filter<T>.AndGroup))
                .Append(typeof(Filter<T>.OrGroup))
                .Select(GetItem);
            GenericSelector<Type> selector = new GenericSelector<Type>(null, false, types);
            selector.EnableSingleClickToSelect();
            selector.SelectionConfirmed += x =>
            {
                Filter<T>.Item item = new Filter<T>.Item((IFilter<T>)Activator.CreateInstance(x.First()));
                onSelect(item);
            };

            selector.ShowInPopup();

            GenericSelectorItem<Type> GetItem(Type type)
            {
                string text = type.GetAttribute<DescriptionAttribute>()?.Name ?? type.Name;
                return new GenericSelectorItem<Type>(text, type);
            }
        }

        private void DrawItem(InspectorProperty property)
        {
            InspectorProperty filterProperty = property.FindChild(FilterFieldName);
            switch (filterProperty.ValueEntry.WeakSmartValue)
            {
                case Filter<T>.AndGroup temp1:
                case Filter<T>.OrGroup temp2:
                    DrawGroup(property);
                    break;
                case IFilter<T> temp:
                    DrawFilter(property, property.FindChild(ValueFieldName));
                    break;
                default:
                    SirenixEditorGUI.MessageBox("Invalid FilterType => " + filterProperty.ValueEntry.TypeOfValue.Name, MessageType.Error);
                    break;
            }
        }

        private void DrawFilter(InspectorProperty property, InspectorProperty valueProperty)
        {
            SirenixEditorGUI.BeginBox();
            EditorGUILayout.BeginHorizontal();

            InspectorProperty reverseProperty = property.FindChild(ReverseFieldName);
            GUIContent        label           = valueProperty.GetDescription();
            if (reverseProperty.GetValue<bool>())
            {
                label.text += "(取反)";
            }

            property.State.Expanded = SirenixEditorGUI.Foldout(property.State.Expanded, label);

            EditorGUILayout.Space(24f);

            bool reverse = EditorGUILayout.ToggleLeft(GUIHelper.TempContent("结果取反?"), reverseProperty.GetValue<bool>(), GUILayout.Width(96f));
            reverseProperty.SetValue(reverse);

            if (SirenixEditorGUI.IconButton(EditorIcons.X))
            {
                if (property.Parent.ChildResolver is ICollectionResolver collectionResolver)
                {
                    collectionResolver.QueueRemove(property.GetValue<object>().TempArray());
                }
                else
                {
                    Property.FindChild(ItemFieldName).FindChild(FilterFieldName).SetValue(null);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(property, this), property.State.Expanded))
            {
                foreach (InspectorProperty child in valueProperty.Children)
                {
                    child.Draw();
                }
            }

            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }

        private void DrawGroup(InspectorProperty property)
        {
            InspectorProperty filterProperty = property.FindChild(FilterFieldName);
            InspectorProperty itemsProperty  = filterProperty.FindChild(ItemsFieldName);

            SirenixEditorGUI.BeginBox();
            EditorGUILayout.BeginHorizontal();

            InspectorProperty reverseProperty = property.FindChild(ReverseFieldName);
            GUIContent        label           = filterProperty.ValueEntry.TypeOfValue.GetDescription();
            if (reverseProperty.GetValue<bool>())
            {
                label.text += "(取反)";
            }

            if (GUILayout.Button(label, EditorStyles.label) && itemsProperty != null)
            {
                ShowSelector(x =>
                {
                    Property.Tree.DelayAction(() =>
                    {
                        itemsProperty.ChildResolver.As<ICollectionResolver>().QueueAdd(x.TempArray());
                        Property.Tree.ApplyChanges();
                    });
                });
            }

            EditorGUILayout.Space(24f);

            bool reverse = EditorGUILayout.ToggleLeft(GUIHelper.TempContent("结果取反?"), reverseProperty.GetValue<bool>(), GUILayout.Width(96f));
            reverseProperty.SetValue(reverse);

            if (SirenixEditorGUI.IconButton(EditorIcons.X))
            {
                if (property.Parent.ChildResolver is ICollectionResolver collectionResolver)
                {
                    collectionResolver.QueueRemove(property.GetValue<object>().TempArray());
                }
                else
                {
                    Property.FindChild(ItemFieldName).FindChild(FilterFieldName).SetValue(null);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (itemsProperty != null)
            {
                foreach (InspectorProperty child in itemsProperty.Children)
                {
                    DrawItem(child);
                }
            }

            SirenixEditorGUI.EndBox();
        }
    }
}