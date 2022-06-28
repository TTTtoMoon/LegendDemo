using Abilities;
using RogueGods.Gameplay.AbilityDriven.Condition;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    public static partial class AbilityUtility
    {
        public static bool Result(this CompareOperator compareOperator, float left, float right)
        {
            switch (compareOperator)
            {
                case CompareOperator.Less:        return left < right;
                case CompareOperator.LessOrEqual: return left <= right;
                case CompareOperator.Equal:       return Mathf.Approximately(left, right);
                case CompareOperator.MoreOrEqual: return left >= right;
                case CompareOperator.More:        return left > right;
                default:                          return false;
            }
        }

        public static TargetFilter GetFilterType(this IAbilityTarget _this)
        {
            if (_this is IFilterTarget filterTarget)
            {
                return filterTarget.Type;
            }

            return TargetFilter.None;
        }
    }
}