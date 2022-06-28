using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Condition
{
    [Serializable]
    [Description("血量百分比")]
    public sealed class CheckHealthByPercent : ICondition
    {
        [Description("血量系数(0-1)")]
        public AbilityVariable CheckHPRatio = new AbilityVariable(1f);

        [Description("比较方式")]
        public CompareOperator CompareOperator;

        public bool Verify(in Ability ability)
        {
            Actor actor = ability.Owner as Actor;
            if (actor == null) return false;

            float current = actor.CurrentHealth;
            float checker = actor.Attribute[AttributeType.MaxHealth] * CheckHPRatio;
            return CompareOperator.Result(current, checker);
        }
    }
}