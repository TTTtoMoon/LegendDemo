using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("添加Buff")]
    public sealed class AttachBuff : TriggerFeatureEffect
    {
        [Description("BuffID")]
        public AbilityReference Buff;

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
                    actor.BuffDirector.Acquire(new BuffDescriptor(Buff.AbilityID, Buff), Ability);
                }
            }
        }
    }
}