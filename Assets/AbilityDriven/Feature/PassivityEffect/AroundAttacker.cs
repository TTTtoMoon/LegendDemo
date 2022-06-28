using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("环绕攻击")]
    public class AroundAttacker : AroundEffect
    {
        [Description("目标筛选")]
        public Filter<IAbilityTarget> TargetFilter = new Filter<IAbilityTarget>();

        [Description("伤害范围")]
        public Vector3 TriggerSize = new Vector3(0.25f, 10f, 0.25f);

        [Description("伤害比率")]
        public AbilityVariable DamageCoefficient = new AbilityVariable(1f);

        [Description("伤害材质")]
        public DamageMaterial DamageMaterial;
        
        [Description("击中音效")]
        public AudioClip HitSound;
        
        [Description("击中特效")]
        public GameObject HitVFX;

        protected override AroundAgent CreateAgent(IAbilityTarget target)
        {
            return new Attacker(this);
        }

        private class Attacker : AroundAgent
        {
            public Attacker(AroundAttacker effect) : base(effect)
            {
                m_Attacker = effect;
                m_Cache    = HashSetPool<IAbilityTarget>.Get();
            }

            private AroundAttacker          m_Attacker;
            private HashSet<IAbilityTarget> m_Cache;

            protected override void OnDispose()
            {
                HashSetPool<IAbilityTarget>.Release(m_Cache);
                m_Cache = null;
            }

            public override void OnUpdate(Vector3[] prePositions)
            {
                HashSet<IAbilityTarget> temp = HashSetPool<IAbilityTarget>.Get();
                for (int i = 0, length = VFX.Length; i < length; i++)
                {
                    Transform transform = VFX[i].transform;
                    Cast(prePositions[i], transform.rotation, transform.position);
                }
                
                HashSetPool<IAbilityTarget>.Release(m_Cache);
                m_Cache = temp;

                void Cast(Vector3 startPosition, Quaternion rotation, Vector3 endPosition)
                {
                    using CrashResult result = CrashCaster.Cast(new Box(startPosition, rotation, m_Attacker.TriggerSize), endPosition - startPosition);
                    for (int i = 0; i < result.Count; i++)
                    {
                        IAbilityTarget target = result[i];
                        if (target.IsValidAndActive() == false ||
                            temp.Add(target)          == false ||
                            m_Cache.Contains(target)           ||
                            m_Attacker.TargetFilter.Verify(target) == false)
                        {
                            continue;
                        }

                        Apply(target);
                    }
                }
            }

            private void Apply(IAbilityTarget target)
            {
                DamageRequest request = new DamageRequest()
                {
                    Ability            = m_Attacker.Ability,
                    DamageMaker        = m_Attacker.Ability.Source as IDamageMaker,
                    DamageTaker        = target as IDamageTaker,
                    DamageCoefficient  = m_Attacker.DamageCoefficient,
                    SourcePosition     = m_Attacker.Ability.Owner.Position,
                    DamageMaterial     = m_Attacker.DamageMaterial,
                    HitSound           = m_Attacker.HitSound,
                    HitVFX             = m_Attacker.HitVFX,
                    Tag                = m_Attacker.Ability.Tag,
                };

                GameManager.DamageSystem.ApplyDamage(request);
            }
        }
    }
}