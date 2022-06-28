using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("跳向目标")]
    public class JumpToTarget : TriggerFeatureEffect
    {
        [Description("最大跳跃距离")] [Min(0f)]
        public float MaxJumpDistance;
        
        [Description("跳跃时长")] [Min(0f)]
        public float JumpDuration;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (Ability.Owner is Actor owner)
            {
                Actor target = null;
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] is Actor actor)
                    {
                        target = actor;
                        break;
                    }
                }

                if (target == null)
                {
                    return;
                }

                Vector3 endPosition = target.Position;
                Vector3 offset      = endPosition - owner.Position;
                float   distance    = offset.magnitude;
                if (distance > MaxJumpDistance)
                {
                    endPosition = owner.Position + offset / (distance / MaxJumpDistance);
                }

                GameManager.Instance.StartCoroutine(Jump.Jumping(Ability, owner, JumpDuration, endPosition));
            }
        }
    }
}