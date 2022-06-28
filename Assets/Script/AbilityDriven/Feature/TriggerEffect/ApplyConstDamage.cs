using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("造成固定伤害")]
    public sealed class ApplyConstDamage : TriggerFeatureEffect
    {
        [Description("伤害比率")]
        public AbilityVariable DamageCoefficient = new AbilityVariable(1f);

        [Description("伤害材质")]
        public DamageMaterial DamageMaterial;

        [Description("打断等级")]
        public AbilityVariable HurtAttackLevel = new AbilityVariable(0f);

        [Description("击退等级")]
        public AbilityVariable RetreatAttackLevel = new AbilityVariable(0f);

        [Description("击退速度")]
        public AbilityVariable RetreatSpeed = new AbilityVariable(0f);

        [Description("击退朝向(绝对/相对)", "true:绝对 false:相对")]
        public bool RetreatAbsolute;

        [Description("击中音效")]
        public AudioClip HitSound;
        
        [Description("击中特效")]
        public GameObject HitVFX;

        [Description("忽略相同目标")]
        public bool IgnoreSameTarget;

        private HashSet<IAbilityTarget> m_Cache;

        protected override void OnEnable()
        {
            if (IgnoreSameTarget)
            {
                m_Cache = HashSetPool<IAbilityTarget>.Get();
            }
        }

        protected override void OnDisable()
        {
            if (IgnoreSameTarget)
            {
                HashSetPool<IAbilityTarget>.Release(m_Cache);
                m_Cache = null;
            }
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Apply(targets[i]);
            }
        }

        private void Apply(IAbilityTarget target)
        {
            if (target.IsValidAndActive() == false)
            {
                return;
            }

            if (IgnoreSameTarget && m_Cache.Add(target) == false)
            {
                return;
            }

            DamageRequest request = new DamageRequest()
            {
                Ability            = Ability,
                DamageMaker        = Ability.Source as IDamageMaker,
                DamageTaker        = target as IDamageTaker,
                DamageCoefficient  = DamageCoefficient,
                SourcePosition     = Ability.Owner.Position,
                DamageMaterial     = DamageMaterial,
                HurtAttackLevel    = HurtAttackLevel,
                RetreatAttackLevel = RetreatAttackLevel,
                RetreatSpeed       = RetreatSpeed,
                RetreatAbsolute    = RetreatAbsolute,
                HitSound           = HitSound,
                HitVFX             = HitVFX,
                Tag                = Ability.Tag,
            };

            GameManager.DamageSystem.ApplyDamage(request);
        }
    }
}