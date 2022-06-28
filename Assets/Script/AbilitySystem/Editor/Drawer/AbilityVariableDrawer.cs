using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    class AbilityVariableDrawer : OdinValueDrawer<AbilityVariable>
    {
        private InspectorProperty m_NameProperty;
        private InspectorProperty m_ValueProperty;
        private bool              m_IsInt;

        protected override void Initialize()
        {
            base.Initialize();
            m_NameProperty  = Property.FindChild("m_UniqueName");
            m_ValueProperty = Property.FindChild("m_Value");
            m_IsInt         = Property.FindChild("m_IsInt").GetValue<bool>();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (m_IsInt)
            {
                m_ValueProperty.SetValue((float)EditorGUILayout.IntField(label, (int)m_ValueProperty.GetValue<float>()));
            }
            else
            {
                m_ValueProperty.SetValue(EditorGUILayout.FloatField(label, m_ValueProperty.GetValue<float>()));
            }

            string suffix = $"[{m_NameProperty.GetValue<string>()}]";
            GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0.0f, 8f), suffix, SirenixGUIStyles.RightAlignedGreyMiniLabel);
        }
    }
}