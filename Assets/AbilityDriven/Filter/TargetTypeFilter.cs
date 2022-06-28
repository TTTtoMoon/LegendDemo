using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Filter
{
    [Serializable]
    [Description("目标类型筛选")]
    public class TargetTypeFilter : IFilter<IAbilityTarget>
    {
        [Description("有效类型")]
        public TargetFilter Filter;
        
        public bool Verify(in IAbilityTarget arg)
        {
            return IsValid(arg.GetFilterType());
        }

        private bool IsValid(TargetFilter target)
        {
            return (Filter & target) != TargetFilter.None;
        }
    }
}