using RogueGods.Gameplay;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace RogueGods.GameplayEditor
{
    public sealed class AnimationPropertyDrawer : OdinValueDrawer<AnimationProperty>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            InspectorProperty property = Property.FindChild(x => x.Name == "m_Name", false);
            property.Draw(label);
        }
    }
}