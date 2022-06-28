using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor.Drawer
{
    [DrawerPriority(0.0, 0.0, 3000.0)]
    class PassivityFeatureEffectDrawer<T> : ReferenceItemDrawer<T> where T : PassivityFeatureEffect
    {
    }

    [DrawerPriority(0.0, 0.0, 3000.0)]
    class TriggerFeatureEffectDrawer<T> : ReferenceItemDrawer<T> where T : TriggerFeatureEffect
    {
    }

    [DrawerPriority(0.0, 0.0, 3000.0)]
    class FeatureTargetDrawer<T> : ReferenceItemDrawer<T> where T : FeatureTarget
    {
    }

    [DrawerPriority(0.0, 0.0, 3000.0)]
    class FeatureTriggerDrawer<T> : ReferenceItemDrawer<T> where T : FeatureTrigger
    {
    }

    abstract class ReferenceItemDrawer<T> : OdinValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property?.Parent?.ChildResolver is ICollectionResolver;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Property.Children.Count > 0)
            {
                Property.State.Expanded = SirenixEditorGUI.Foldout(Property.State.Expanded, label);

                if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(Property, this), Property.State.Expanded))
                {
                    EditorGUI.indentLevel++;
                    foreach (InspectorProperty child in Property.Children)
                    {
                        child.Draw();
                    }
                    EditorGUI.indentLevel--;
                }

                SirenixEditorGUI.EndFadeGroup();
            }
            else
            {
                GUILayout.Label(GUIHelper.TempContent($"{label.text} (无需任何参数配置)", label.tooltip));
            }
        }
    }
}