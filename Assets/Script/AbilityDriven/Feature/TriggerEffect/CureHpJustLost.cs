using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("根据刚受到的伤害恢复生命")]
    public class CureHpJustLost : TriggerFeatureEffect
    {
        /// <summary>
        /// 时间阙值(单位秒)
        /// </summary>
        [Description("时间阙值(单位秒)")]
        public float TimeThreshold;

        /// <summary>
        /// 恢复百分比
        /// </summary>
        [Description("恢复百分比")]
        public AbilityVariable CurePercent;

        private float m_JustLostHP;
        private float m_JustLostTime;
    
        protected override void OnEnable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnTakeDamage += OnTakeDamage;
            }
        }

        protected override void OnDisable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnTakeDamage -= OnTakeDamage;
            }
        }

        private void OnTakeDamage(DamageResponse damageResult)
        {
            m_JustLostHP   = damageResult.Damage;
            m_JustLostTime = Time.time;
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (Time.time <= m_JustLostTime + TimeThreshold && Ability.Owner is Actor actor)
            {
                actor.IncreaseCurrentHealth(m_JustLostHP * CurePercent.Value);
            }
        }
    }
}