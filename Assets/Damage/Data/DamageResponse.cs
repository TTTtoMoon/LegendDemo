using System;
using Abilities;
using RogueGods.Gameplay.AbilityDriven;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 伤害结果
    /// </summary>
    public struct DamageResponse
    {
        /// <summary>
        /// 伤害来源
        /// </summary>
        public Ability Ability;

        /// <summary>
        /// 伤害创建者
        /// </summary>
        public IDamageMaker DamageMaker;

        /// <summary>
        /// 伤害接收者
        /// </summary>
        public IDamageTaker DamageTaker;

        /// <summary>
        /// 伤害值
        /// </summary>
        public float Damage;

        /// <summary>
        /// 是否暴击了
        /// </summary>
        public bool IsCritical;

        /// <summary>
        /// 伤害来源位置
        /// </summary>
        public Vector3 SourcePosition;

        /// <summary>
        /// 打断等级
        /// </summary>
        public float HurtAttackLevel;

        /// <summary>
        /// 击退等级
        /// </summary>
        public float RetreatAttackLevel;

        /// <summary>
        /// 击退速度
        /// </summary>
        public float RetreatSpeed;

        /// <summary>
        /// 击退朝向(相对/绝对)
        /// </summary>
        public bool RetreatAbsolute;

        /// <summary>
        /// 是否触发连杀减速
        /// </summary>
        public bool TriggerComboKill;
        
        /// <summary>
        /// 能力标签
        /// </summary>
        public AbilityTag Tag => Ability != null ? Ability.Tag : default;

        /// <summary>
        /// 构建伤害结果
        /// </summary>
        /// <param name="request">伤害请求</param>
        public DamageResponse(in DamageRequest request, in Attacker attacker, in Defender defender)
        {
            Ability            = request.Ability;
            DamageMaker        = request.DamageMaker;
            DamageTaker        = request.DamageTaker;
            SourcePosition     = request.SourcePosition;
            HurtAttackLevel    = request.HurtAttackLevel;
            RetreatAttackLevel = request.RetreatAttackLevel;
            RetreatSpeed       = request.RetreatSpeed;
            RetreatAbsolute    = request.RetreatAbsolute;
            TriggerComboKill   = request.TriggerComboKill;

            // 伤害计算
            // 公式：BaseDamage        = Attack * (1 + AttackMultiplier) * DamageCoefficient + DamageAddition
            // 公式：CriticalPower     = BaseCriticalPower + CriticalPowerAddition
            // 公式：DamageTakeRatio   = (1 - DamageDeductionsRatio1) * (1 - DamageDeductionsRatio2) * (...)
            // 公式：FinalDamage       = (BaseDamage * WeaponPower * DamagePower * CriticalPower - DamageReductions) * DamageTakeRatio;
            // 计算基础伤害
            Damage = attacker.Attack * (1f + attacker.AttackMultipleModifier) * request.DamageCoefficient + attacker.DamageAddition;
            // 计算武器加成
            Damage *= attacker.WeaponPower;
            // 计算伤害加成
            Damage *= 1f + attacker.DamagePower;
            // 计算暴击伤害
            IsCritical = CalculateCritical(request, attacker, defender);
            if (IsCritical)
            {
                Damage *= GameManager.DamageSystem.Settings.CriticalPower + attacker.CriticalPowerAddition;
            }

            // 减少伤害减免固定值
            Damage = Mathf.Max(Damage - defender.DamageReductions, 0f);
            // 计算伤害减免
            float minTakeRatio = 1f - GameManager.DamageSystem.Settings.MaxDamageDeductions;
            Damage *= Mathf.Max(defender.DamageTakeRatio, minTakeRatio);
            // 伤害最低为1，且取整
            Damage = Mathf.Max(Mathf.RoundToInt(Damage), 1f);
        }

        /// <summary>
        /// 获取伤害趋势方向
        /// </summary>
        /// <param name="taker"></param>
        /// <returns></returns>
        public readonly Vector3 GetDamageDirection(Transform taker)
        {
            Vector3 direction;
            if (RetreatAbsolute)
            {
                direction = Ability != null ? Ability.Owner.transform.forward : -taker.forward;
            }
            else
            {
                direction = taker.position - SourcePosition;
                direction.Normalize();
            }

            return direction;
        }

        /// <summary>
        /// 计算暴击与否
        /// </summary>
        /// <param name="request"></param>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        private static bool CalculateCritical(in DamageRequest request, in Attacker attacker, in Defender defender)
        {
            // Dot伤不能暴击
            if (request.Tag.ContainsAny(AbilityTagDefine.Dot))
            {
                return false;
            }

            return RandomUtility.Probability(attacker.CriticalChance);
        }
    }
}