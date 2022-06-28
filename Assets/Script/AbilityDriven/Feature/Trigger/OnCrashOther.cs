using System;
using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当碰撞到其他目标时(仅限Orb)")]
    public class OnCrashOther : FeatureTrigger
    {
        private struct CrashedTarget
        {
            public IAbilityTarget Target;
            public float          Time;
        }

        [Description("目标筛选")]
        public Filter<IAbilityTarget> Filter;

        [Description("同一目标仅触发一次？")]
        public bool IgnoreSameTarget;

        [Description("同一目标触发间隔")] [HideIf("IgnoreSameTarget")]
        public float SameTargetTriggerTimeSpan = 0.2f;

        private List<CrashedTarget> m_CachedTarget = new List<CrashedTarget>();

        protected override void OnEnable()
        {
            if (Ability.Owner is ICanCastCrash canCrash)
            {
                canCrash.OnCrash.AddListener(OnCrash);
            }
        }

        protected override void OnDisable()
        {
            if (Ability.Owner is ICanCastCrash canCrash)
            {
                canCrash.OnCrash.RemoveListener(OnCrash);
            }
            
            m_CachedTarget.Clear();
        }

        protected override void OnUpdate()
        {
        }

        private bool OnCrash(IAbilityTarget target)
        {
            float now        = Time.time;
            if (Filter.Verify(target))
            {
                if (IgnoreSameTarget)
                {
                    for (int i = 0; i < m_CachedTarget.Count; i++)
                    {
                        if (m_CachedTarget[i].Target == target)
                        {
                            return false;
                        }
                    }
                }
                else if(SameTargetTriggerTimeSpan > float.Epsilon)
                {
                    for (int i = 0; i < m_CachedTarget.Count; i++)
                    {
                        if (m_CachedTarget[i].Target == target)
                        {
                            if (m_CachedTarget[i].Time + SameTargetTriggerTimeSpan > now)
                            {
                                return false;
                            }

                            m_CachedTarget.RemoveAt(i);
                            break;
                        }
                    }
                }
                
                InvokeEffect(target);
                m_CachedTarget.Add(new CrashedTarget()
                {
                    Target = target,
                    Time   = now,
                });
                return true;
            }

            return false;
        }
    }
}