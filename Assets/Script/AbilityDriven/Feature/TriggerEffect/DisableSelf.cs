using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("禁用自身效果")]
    public sealed class DisableSelf : TriggerFeatureEffect
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            Ability.Director.Disable(Ability);
        }
    }
}