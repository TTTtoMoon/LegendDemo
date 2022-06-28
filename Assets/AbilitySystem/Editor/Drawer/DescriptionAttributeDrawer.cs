using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    class DescriptionAttributeDrawer : OdinAttributeDrawer<DescriptionAttribute>
    {
        private GUIContent m_OverrideLabel;

        protected override void Initialize()
        {
            base.Initialize();
            m_OverrideLabel = new GUIContent();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            m_OverrideLabel.text    = ObjectNames.NicifyVariableName(Attribute.Name);
            m_OverrideLabel.tooltip = Attribute.ToolTip;
            CallNextDrawer(m_OverrideLabel);
        }
    }
}