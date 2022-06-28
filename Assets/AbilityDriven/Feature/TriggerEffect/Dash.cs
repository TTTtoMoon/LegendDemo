using System;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("冲刺一段距离")]
    public sealed class Dash : TriggerFeatureEffect
    {
        [Description("冲刺距离")]
        public AbilityVariable DashDistance = new AbilityVariable(10f);

        [Description("冲刺角度")] [Range(-180f, 180f)]
        public float DashDirectionAngle;

        [Description("冲刺距离缩放")]
        public Vector2 DashDistanceOffset;

        [Description("冲刺时长")] 
        public float DashDuration = 0.2f;

        private Actor m_ActorOwner;
        
        protected override void OnEnable()
        {
            m_ActorOwner = Ability.Owner as Actor;
        }

        protected override void OnDisable()
        {
            if (m_ActorOwner != null)
            {
                DashAgent.ForceStop(m_ActorOwner);
            }
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (m_ActorOwner == null)
            {
                return;
            }

            DashAgent.Begin(m_ActorOwner,
                Quaternion.Euler(0f, 360f + DashDirectionAngle, 0f) * m_ActorOwner.transform.forward,
                DashDistance + Random.Range(DashDistanceOffset.x, DashDistanceOffset.y),
                DashDuration);
        }
    }
}