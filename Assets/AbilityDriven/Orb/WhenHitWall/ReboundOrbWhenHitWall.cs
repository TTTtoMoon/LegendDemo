using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("反弹")]
    public sealed class ReboundOrbWhenHitWall : OrbWhenHitWall
    {
        [Description("可反弹次数")]
        public AbilityVariable ReboundTimes = new AbilityVariable(1);
        
        [Description("反弹后攻击系数修改")]
        public AbilityVariable ReboundAttackMultipleModifier = new AbilityVariable(0f);

        private int            m_ReboundTimes;
        private DamageModifier m_DamageModifier;

        protected override void OnEnable()
        {
            m_ReboundTimes                          =  0;
            m_DamageModifier                        =  OnPrepareMakeDamage;
            GameManager.DamageSystem.DamageModifier += m_DamageModifier;
            
            void OnPrepareMakeDamage(in DamageRequest damageRequest, ref Attacker attacker, ref Defender defender)
            {
                if (damageRequest.Ability != Orb.Ability) return;
                if (m_ReboundTimes > 0)
                {
                    attacker.DamagePower += ReboundAttackMultipleModifier;
                    DamageSystem.PushModify("撞墙减伤", attacker);
                }
            }
        }

        protected override void OnDisable()
        {
            GameManager.DamageSystem.DamageModifier -= m_DamageModifier;
        }

        public override void Handle(Orb orb, RaycastHit hit)
        {
            if (m_ReboundTimes < ReboundTimes)
            {
                m_ReboundTimes++;
                Vector3 forward          = orb.transform.forward.IgnoreY();
                Vector3 reflectDirection = Vector3.Reflect(forward, hit.normal);
                orb.Mode.Rotate(hit.point, reflectDirection);
            }
            else
            {
                orb.transform.position = hit.point;
                GameManager.OrbSystem.Destroy(orb);
            }
        }
    }
}