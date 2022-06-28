using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("百分比恢复当前血量")]
    public class IncreaseHealthByPercent : TriggerFeatureEffect
    {
        [Description("修改系数(0 ~ 1)")]
        public AbilityVariable IncreaseHealthPercent = new AbilityVariable(0f);
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] is Actor actor)
                {
                    actor.IncreaseCurrentHealth(IncreaseHealthPercent.Value * actor.Attribute[AttributeType.MaxHealth]);
                }
            }
        }
    }
}