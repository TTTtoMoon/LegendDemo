using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Filter
{
    [Serializable]
    [Description("标签过滤")]
    public class AbilityTagFilter : IFilter<Ability>
    {
        [Description("目标标签")]
        public AbilityTag TargetTag;
        
        public bool Verify(in Ability arg)
        {
            return arg != null && arg.Tag.ContainsAll(TargetTag);
        }
    }
}