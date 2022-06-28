using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    [CustomEditor(typeof(AbilityConfiguration))]
    class AbilityConfigurationInspector : OdinEditor
    {
        const string ScrollPos = "SCROLL_POS";

        private AbilityConfiguration    m_Configuration => target as AbilityConfiguration;
        private PropertyTree            m_AbilityTree;
        private List<InspectorProperty> m_Variables = new List<InspectorProperty>();
        private InspectorProperty       m_Group, m_ConfigurationID, m_Tag, m_Duration, m_Components, m_Features;
        private InspectorProperty       m_Selected;
        private Vector2                 m_VariableScrollPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_AbilityTree     = PropertyTree.Create(new SerializedObject(m_Configuration.Ability));
            m_Group           = Tree.GetPropertyAtUnityPath("m_Group");
            m_ConfigurationID = m_AbilityTree.GetPropertyAtUnityPath("m_ConfigurationID");
            m_Tag             = m_AbilityTree.GetPropertyAtUnityPath("m_Tag");
            m_Duration        = m_AbilityTree.GetPropertyAtUnityPath("m_Duration");
            m_Components      = m_AbilityTree.GetPropertyAtUnityPath("m_Components");
            m_Features        = m_AbilityTree.GetPropertyAtUnityPath("m_Features");
            ValidateVariables();
            Tree.OnPropertyValueChanged += (property, index) =>
            {
                Tree.DelayAction(ValidateVariables);
                m_Configuration.IsDirty = true;
            };

            m_AbilityTree.OnPropertyValueChanged += (property, index) =>
            {
                m_AbilityTree.DelayAction(ValidateVariables);
                m_Configuration.IsDirty = true;
            };
        }

        protected override void DrawTree()
        {
            EditorGUI.BeginChangeCheck();
            Tree.BeginDraw(true);
            m_AbilityTree.BeginDraw(true);
            EditorGUILayout.BeginHorizontal();
            DrawBaseInfo();
            DrawSelected();
            EditorGUILayout.EndHorizontal();
            m_AbilityTree.EndDraw();
            Tree.EndDraw();
            if (EditorGUI.EndChangeCheck())
            {
                m_Configuration.IsDirty = true;
            }
        }

        private void DrawBaseInfo()
        {
            const float Width = 480f;
            EditorGUILayout.BeginVertical(GUILayout.Width(Width));

            SirenixEditorGUI.BeginBox("基础信息");
            GUIHelper.PushLabelWidth(60f);
            m_ConfigurationID.Draw();
            m_Group.Draw();
            m_Configuration.CustomName = EditorGUILayout.TextField("名称", m_Configuration.CustomName);
            m_Tag.Draw();
            m_Duration.Draw();
            GUIHelper.PopLabelWidth();

            SirenixEditorGUI.Title("备注", null, TextAlignment.Left, false, false);
            m_Configuration.Description = EditorGUILayout.TextField(m_Configuration.Description, EditorStyles.textArea, GUILayout.Height(60f));
            SirenixEditorGUI.EndBox();

            SirenixEditorGUI.BeginBox("变量一览", false, GUILayout.Height(240f));
            m_VariableScrollPosition = EditorGUILayout.BeginScrollView(m_VariableScrollPosition);
            GUIHelper.PushLabelWidth(120f);
            for (int i = 0; i < m_Variables.Count; i++)
            {
                InspectorProperty variable = m_Variables[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{i:00}]", GUILayout.Width(30f));
                variable.Draw();
                if (SirenixEditorGUI.IconButton(EditorIcons.Link))
                {
                    Clipboard.Copy(variable.FindChild("m_UniqueName").GetValue<string>());
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2f);
            }

            GUIHelper.PopLabelWidth();
            EditorGUILayout.EndScrollView();
            SirenixEditorGUI.EndBox();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawList(m_Components, Width / 2f);
                DrawList(m_Features,   Width / 2f);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSelected()
        {
            GUIContent title = m_Selected == null ? GUIHelper.TempContent("请先选择一个Component或Feature") : m_Selected.ValueEntry.TypeOfValue.GetDescription();
            SirenixEditorGUI.BeginBox(title, false, GUILayout.ExpandHeight(true));
            GUIHelper.PushLabelWidth(360f);
            if (m_Selected == null)
            {
                SirenixEditorGUI.MessageBox(title.text, MessageType.Info);
            }
            else if (m_Selected.Children.Count > 0)
            {
                Vector2 scrollPos = EditorGUILayout.BeginScrollView(m_Selected.GetState(ScrollPos, Vector2.zero));
                for (int index = 0; index < m_Selected.Children.Count; ++index)
                {
                    InspectorProperty child = m_Selected.Children[index];
                    child.Draw();
                }

                EditorGUILayout.EndScrollView();
                m_Selected.SetState(ScrollPos, scrollPos);

                if (m_Configuration.CheckValid(out string error) == false)
                {
                    SirenixEditorGUI.MessageBox(error, MessageType.Error);
                }
            }
            else
            {
                SirenixEditorGUI.MessageBox("当前选择无需任何配置", MessageType.Info);
            }

            GUIHelper.PopLabelWidth();
            SirenixEditorGUI.EndBox();
        }

        private void DrawList(InspectorProperty listProperty, float width)
        {
            SirenixEditorGUI.BeginBox(listProperty.GetAttribute<DescriptionAttribute>()?.Name ?? listProperty.Name, false, GUILayout.Width(width));

            if (SirenixEditorGUI.IconButton(GUILayoutUtility.GetLastRect().AlignRight(18).AlignCenterY(18), EditorIcons.Plus))
            {
                listProperty.ChildResolver.As<ICollectionResolver>().ElementType.ShowImplementSelector(OnSelectionConfirmed);

                void OnSelectionConfirmed(IEnumerable<Type> selection)
                {
                    listProperty.ChildResolver.As<ICollectionResolver>().QueueAdd(Activator.CreateInstance(selection.First()).TempArray());
                }
            }

            Vector2           scrollPos = EditorGUILayout.BeginScrollView(listProperty.GetState(ScrollPos, Vector2.zero));
            InspectorProperty delete    = null;
            foreach (InspectorProperty child in listProperty.Children)
            {
                GUIHelper.PushColor(child == m_Selected ? Color.cyan : GUI.color);
                EditorGUILayout.BeginHorizontal(SirenixGUIStyles.ToggleGroupTitleBg);
                if (GUILayout.Button(child.ValueEntry.TypeOfValue.GetDescription(), SirenixGUIStyles.BoldTitle, GUILayout.MaxWidth(width - 32f)))
                {
                    m_Selected = child;
                }

                if (SirenixEditorGUI.IconButton(EditorIcons.X))
                {
                    delete = child;
                }

                EditorGUILayout.EndHorizontal();
                GUIHelper.PopColor();
            }

            if (delete != null)
            {
                if (delete == m_Selected)
                {
                    m_Selected = null;
                }

                listProperty.ChildResolver.As<ICollectionResolver>().QueueRemove(delete.ValueEntry.WeakSmartValue.TempArray());
            }

            EditorGUILayout.EndScrollView();
            listProperty.SetState(ScrollPos, scrollPos);

            SirenixEditorGUI.EndBox();
        }

        private void ValidateVariables()
        {
            m_Variables.Clear();
            CollectVariables(m_AbilityTree.RootProperty);
            HashSet<string> variableNames = new HashSet<string>();
            foreach (var variable in m_Variables)
            {
                string variableName = variable.FindChild("m_UniqueName").GetValue<string>();
                if (string.IsNullOrWhiteSpace(variableName) ||
                    variableNames.Add(variableName) == false)
                {
                    variable.FindChild("m_UniqueName").SetValue(string.Empty);
                }
            }

            foreach (var variable in m_Variables)
            {
                string variableName = variable.FindChild("m_UniqueName").GetValue<string>();
                if (string.IsNullOrEmpty(variableName))
                {
                    variableName = AbilityEditorUtility.MakeUniqueVariableName(variableNames, variable.GetVariableName());
                    variableNames.Add(variableName);
                    variable.FindChild("m_UniqueName").SetValue(variableName);
                }
            }

            m_AbilityTree.ApplyChanges();

            void CollectVariables(InspectorProperty property)
            {
                AbilityVariable variable = property.GetValue<AbilityVariable>();
                if (variable != null)
                {
                    m_Variables.Add(property);
                }

                foreach (var child in property.Children)
                {
                    CollectVariables(child);
                }
            }
        }
    }
}