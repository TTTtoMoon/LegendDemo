using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abilities;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    class AbilityTagDrawer : OdinValueDrawer<AbilityTag>
    {
        private static List<GenericSelectorItem<long>> s_Items;

        static AbilityTagDrawer()
        {
            s_Items = new List<GenericSelectorItem<long>>();
            Type defineType = AbilityEditorUtility.AllTypes.FirstOrDefault(x => x.GetCustomAttribute<AbilityTagDefineAttribute>() != null);
            if (defineType == null) return;
            FieldInfo[] defines    = defineType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
            for (int i = 0; i < defines.Length; i++)
            {
                if (defines[i].FieldType != typeof(AbilityTag)) continue;

                string name = defines[i].GetCustomAttribute<DescriptionAttribute>()?.Name ?? defines[i].Name;
                long  tag  = (long)(AbilityTag)defines[i].GetValue(null);
                s_Items.Add(new GenericSelectorItem<long>(name, tag));
            }
        }

        private static GenericSelector<long> CreateSelector()
        {
            var selector = new GenericSelector<long>(null, true, s_Items);
            selector.CheckboxToggle = true;
            return selector;
        }

        private string                 m_Current;
        private GenericSelector<long> m_Selector  = CreateSelector();
        private List<long>            m_Selection = new List<long>();

        protected override void Initialize()
        {
            base.Initialize();
            UpdateState();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            IEnumerable<long> selection = GenericSelector<long>.DrawSelectorDropdown(label, m_Current, ShowSelector);
            if (selection != null)
            {
                long tag = 0;
                foreach (long item in selection)
                {
                    tag |= item;
                }

                long current = (long)Property.FindChild(x => x.Name == "Tag", false).ValueEntry.WeakSmartValue;
                if (current != tag)
                {
                    Property.FindChild(x => x.Name == "Tag", false).ValueEntry.WeakSmartValue = tag;
                    UpdateState();
                }
            }
        }

        private void UpdateState()
        {
            m_Selection.Clear();
            long  current = (long)Property.FindChild(x => x.Name == "Tag", false).ValueEntry.WeakSmartValue;
            m_Current    = "";
            for (int i = 0; i < 64; i++)
            {
                long temp = 1L << i;
                if ((temp & current) == temp)
                {
                    m_Current += s_Items.Find(x => x.Value == temp).Name + ",";
                    m_Selection.Add(temp);
                }
            }

            m_Current = string.IsNullOrEmpty(m_Current) ? "无" : m_Current.Remove(m_Current.Length - 1);
            m_Selector.SetSelection(m_Selection);
        }

        private OdinSelector<long> ShowSelector(Rect rect)
        {
            rect.x      = (float)(int)rect.x;
            rect.y      = (float)(int)rect.y;
            rect.width  = (float)(int)rect.width;
            rect.height = (float)(int)rect.height;
            m_Selector.ShowInPopup(rect, Vector2.zero);
            return m_Selector;
        }
    }
}