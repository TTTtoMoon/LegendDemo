using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当该能力生效时")]
    public class OnAbilityEnable : FeatureTrigger
    {
        protected override void OnEnable()
        {
            InvokeEffect();
        }

        protected override void OnDisable()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}