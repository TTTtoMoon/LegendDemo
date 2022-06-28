using System;
using Abilities;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.Condition
{
    [Serializable]
    [Description("概率")]
    public sealed class RandomChance : ICondition
    {
        [Description("概率值(0-1)")]
        public AbilityVariable Chance = new AbilityVariable(1f);

        public bool Verify(in Ability ability)
        {
            return Random.value <= Chance;
        }
    }
}