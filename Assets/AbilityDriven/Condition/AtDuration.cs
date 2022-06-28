using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Condition
{
    [Serializable]
    [Description("在某时间段内")]
    public class AtDuration : ICondition
    {
        [Description("开始时间点(秒)")]
        public float StartTime;
        
        [Description("结束时间点(秒)")]
        public float EndTime;
        
        public bool Verify(in Ability ability)
        {
            return ability.Time >= StartTime && ability.Time < EndTime;
        }
    }
}