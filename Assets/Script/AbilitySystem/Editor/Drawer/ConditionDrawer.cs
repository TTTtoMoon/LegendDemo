using System;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    class ConditionDrawer : OdinValueDrawer<Condition>
    {
        private const string ItemFieldName    = "m_Item";
        private const string ValueFieldName   = "m_Value";
        private const string ReverseFieldName = "m_Reverse";
        private const string ItemsFieldName   = "m_Items";

        protected override void DrawPropertyLayout(GUIContent label)
        {
            InspectorProperty itemProperty  = Property.FindChild(ItemFieldName);
            InspectorProperty valueProperty = itemProperty.FindChild(ValueFieldName);

            SirenixEditorGUI.BeginBox();
            GUILayout.BeginVertical();
            SirenixEditorGUI.Title(label.text, Property.GetValue<object>()?.ToString(), TextAlignment.Left, false, false);
            GUILayout.EndVertical();

            if (valueProperty.ValueEntry.WeakSmartValue == null && GUI.Button(GUILayoutUtility.GetLastRect(), String.Empty, GUIStyle.none))
            {
                typeof(ICondition).ShowImplementSelector(x =>
                {
                    valueProperty.SetValue(Activator.CreateInstance(x.First()));
                });
            }

            if (valueProperty.ValueEntry.WeakSmartValue != null)
            {
                DrawItem(itemProperty);
            }

            SirenixEditorGUI.EndBox();
        }

        private void DrawItem(InspectorProperty property)
        {
            InspectorProperty valueProperty = property.FindChild(ValueFieldName);
            switch (valueProperty.ValueEntry.WeakSmartValue)
            {
                case Condition.AndGroup temp1:
                    DrawGroup(property);
                    break;
                case Condition.OrGroup temp2:
                    DrawGroup(property);
                    break;
                case ICondition temp:
                    DrawCondition(property, valueProperty);
                    break;
                default:
                    SirenixEditorGUI.MessageBox("Invalid ConditionType => " + valueProperty.ValueEntry.TypeOfValue.Name, MessageType.Error);
                    break;
            }
        }

        private void DrawCondition(InspectorProperty property, InspectorProperty valueProperty)
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
                    Property.FindChild(ItemFieldName).FindChild(ValueFieldName).SetValue(null);
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
            InspectorProperty valueProperty = property.FindChild(ValueFieldName);
            InspectorProperty itemsProperty = valueProperty.FindChild(ItemsFieldName);

            SirenixEditorGUI.BeginBox();
            EditorGUILayout.BeginHorizontal();

            InspectorProperty reverseProperty = property.FindChild(ReverseFieldName);
            GUIContent        label           = valueProperty.ValueEntry.TypeOfValue.GetDescription();
            if (reverseProperty.GetValue<bool>())
            {
                label.text += "(取反)";
            }

            if (GUILayout.Button(label, EditorStyles.label))
            {
                typeof(ICondition).ShowImplementSelector(x =>
                {
                    Condition.Item item = new Condition.Item((ICondition)Activator.CreateInstance(x.First()));
                    itemsProperty.ChildResolver.As<ICollectionResolver>().QueueAdd(item.TempArray());
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
                    Property.FindChild(ItemFieldName).FindChild(ValueFieldName).SetValue(null);
                }
            }

            EditorGUILayout.EndHorizontal();

            foreach (InspectorProperty child in itemsProperty.Children)
            {
                DrawItem(child);
            }

            SirenixEditorGUI.EndBox();
        }
    }
}