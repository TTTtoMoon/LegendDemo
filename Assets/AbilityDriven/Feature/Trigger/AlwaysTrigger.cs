using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("始终触发")]
    public class AlwaysTrigger : FeatureTrigger
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnUpdate()
        {
            InvokeEffect();
        }
    }
}