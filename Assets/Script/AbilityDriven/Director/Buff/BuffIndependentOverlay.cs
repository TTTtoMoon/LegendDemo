using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("Buff独立叠加")]
    public class BuffIndependentOverlay : AbilityComponent
    {
        [Description("最多叠加层数")]
        public AbilityVariable MaxOverlayLayer = new AbilityVariable(1);

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}