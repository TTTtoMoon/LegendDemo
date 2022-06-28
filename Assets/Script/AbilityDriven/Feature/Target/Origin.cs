using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("起始目标")]
    public sealed class Origin : FeatureTarget
    {
        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            collector.Append(origin);
        }
    }
}