using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("怪物skill数据")]
    public class MonsterSkillDescription : SkillDescription
    {
        [Description("AI通知")]
        public ActionTime ActionTime = new ActionTime();
        
        /// <summary>
        /// 技能要结束又不结束的标记
        /// </summary>
        public bool IsAction => Ability != null && Ability.Time < (ActionTime.Enable ? ActionTime.Delay : ActingTime.y);
    }
}