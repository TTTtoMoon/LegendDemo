using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [Description("被动（光环）能力")]
    public sealed class AbilityPassivityFeature : IAbilityFeature
    {
        [SerializeField] [Description("生效条件")]
        public Condition Condition = new Condition();

        [SerializeReference] [ReferenceCollection] [Description("目标选择")]
        public FeatureTarget[] Targets = new FeatureTarget[0];

        [SerializeReference] [ReferenceCollection] [Description("效果")]
        public PassivityFeatureEffect[] Effects = new PassivityFeatureEffect[0];

        internal Ability              m_Ability;
        internal List<IAbilityTarget> m_CacheTargets  = new List<IAbilityTarget>();
        private  List<EffectTarget>   m_EffectTargets = new List<EffectTarget>();

        void IAbilityFeature.OnEnable(Ability ability)
        {
            m_Ability      = ability;
            
            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                Effects[i].Ability = ability;
                Effects[i].OnEnable();
            }

            if (Condition.Verify(m_Ability))
            {
                Active();
            }
        }

        void IAbilityFeature.OnDisable()
        {
            DeActive();

            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                Effects[i].OnDisable();
            }
        }

        void IAbilityFeature.OnDispose()
        {
            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                Effects[i].OnDispose();
                Effects[i].Ability = null;
            }

            m_CacheTargets.Clear();
            m_Ability = null;
        }

        void IAbilityFeature.OnUpdate()
        {
            if (Condition.Verify(m_Ability))
            {
                Active();
            }
            else
            {
                DeActive();
            }
        }

        private void Active()
        {
            using (TargetCollector collector = TargetCollector.Collect(m_CacheTargets, m_Ability, Targets, null, EmptyArg.Empty))
            {
                for (int i = 0, length = collector.MissingTargetList.Count; i < length; i++)
                {
                    DeActive(collector.MissingTargetList[i]);
                }
                
                for (int i = 0, length = collector.NewTargetList.Count; i < length; i++)
                {
                    Active(collector.NewTargetList[i]);
                }
            }
        }

        private void Active(IAbilityTarget target)
        {
            EffectTarget effectTarget = new EffectTarget();
            effectTarget.Target = target;
            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                effectTarget.Revocation += Effects[i].Invoke(target);
            }

            m_EffectTargets.Add(effectTarget);
        }

        private void DeActive()
        {
            for (int i = m_CacheTargets.Count - 1; i >= 0; i--)
            {
                DeActive(m_CacheTargets[i]);
            }
        }
        
        private void DeActive(IAbilityTarget target)
        {
            for (int i = m_EffectTargets.Count - 1; i >= 0; i--)
            {
                if (m_EffectTargets[i].Target == target)
                {
                    m_EffectTargets[i].Revocation?.Invoke();
                    m_EffectTargets.RemoveAt(i);
                    break;
                }
            }
        }

        private struct EffectTarget
        {
            public IAbilityTarget   Target;
            public EffectRevocation Revocation;
        }
    }
}