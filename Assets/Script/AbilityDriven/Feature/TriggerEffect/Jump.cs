using System;
using System.Collections;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("跳跃")]
    public sealed class Jump : TriggerFeatureEffect
    {
        [Description("跳跃距离", "x最小距离 y最大距离")] 
        public Vector2 JumpDistance = new Vector2(5f, 10f);

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
            if (Ability.Owner is Actor actor)
            {
                Vector3 endPosition = Utilities.RandomNavPositionInRing(Ability.Owner.Position, JumpDistance.x, JumpDistance.y);

                GameManager.Instance.StartCoroutine(Jumping(Ability, actor, JumpDuration, endPosition));
            }
            else
            {
                Debugger.LogError($"能力{Ability.ConfigurationID}的持有者不是Actor，无法进行跳跃");
            }
        }

        public static IEnumerator Jumping(Ability ability, Actor actor, float jumpDuration, Vector3 endPosition)
        {
            Vector3 startPosition = actor.Position;
            float   startTime     = Time.time;
            float   duration      = jumpDuration;

            while (ability.IsActive && startTime + duration >= Time.time)
            {
                float now = Time.time - startTime;
                actor.NavMeshAgent.Warp(Vector3.Lerp(startPosition, endPosition, now / duration));
                yield return null;
            }

            actor.NavMeshAgent.Warp(endPosition);
        }
    }
}