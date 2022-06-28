using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("恢复当前血量")]
    public class IncreaseHealth : TriggerFeatureEffect
    {
        [Description("修改值")]
        public AbilityVariable IncreaseHealthValue = new AbilityVariable(0);
        
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
                    actor.IncreaseCurrentHealth(IncreaseHealthValue.Value);
                }
            }
        }
    }
}