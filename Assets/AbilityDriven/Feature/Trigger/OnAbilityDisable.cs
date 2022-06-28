using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当该能力失效时")]
    public class OnAbilityDisable : FeatureTrigger
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            InvokeEffect();
        }

        protected override void OnUpdate()
        {
        }
    }
}