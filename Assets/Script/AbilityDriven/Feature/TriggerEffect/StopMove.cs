using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("停止移动(仅限Orb)")]
    public class StopMove : TriggerFeatureEffect
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (Ability.Owner is Orb orb)
            {
                orb.Mode.StopMove();
            }
        }
    }
}