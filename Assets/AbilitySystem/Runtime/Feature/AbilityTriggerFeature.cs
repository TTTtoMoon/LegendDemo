using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [Description("触发能力")]
    public sealed class AbilityTriggerFeature : IAbilityFeature
    {
        [SerializeField] [Description("生效条件")]
        public Condition Condition = new Condition();

        [SerializeReference] [ReferenceCollection] [Description("触发器")]
        public FeatureTrigger[] Triggers = new FeatureTrigger[0];

        [SerializeReference] [ReferenceCollection] [Description("目标选择")]
        public FeatureTarget[] Targets = new FeatureTarget[0];

        [SerializeReference] [ReferenceCollection] [Description("效果")]
        public TriggerFeatureEffect[] Effects = new TriggerFeatureEffect[0];

        internal Ability              m_Ability;
        internal List<IAbilityTarget> m_CacheTargets = new List<IAbilityTarget>();

        /// <summary>
        /// 启用功能
        /// </summary>
        /// <param name="ability">所属能力</param>
        void IAbilityFeature.OnEnable(Ability ability)
        {
            m_Ability = ability;
            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                Effects[i].Ability = ability;
                Effects[i].OnEnable();
            }

            for (int i = 0, length = Triggers.Length; i < length; i++)
            {
                Triggers[i].Ability        = ability;
                Triggers[i].TriggerFeature = this;
                Triggers[i].OnEnable();
            }
        }

        /// <summary>
        /// 禁用功能
        /// </summary>
        void IAbilityFeature.OnDisable()
        {
            for (int i = 0, length = Triggers.Length; i < length; i++)
            {
                Triggers[i].OnDisable();
            }

            for (int i = 0, length = Effects.Length; i < length; i++)
            {
                Effects[i].OnDisable();
            }
        }

        void IAbilityFeature.OnDispose()
        {
            for (int i = 0, length = Triggers.Length; i < length; i++)
            {
                Triggers[i].OnDispose();
                Triggers[i].TriggerFeature = null;
                Triggers[i].Ability        = null;
            }

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
            foreach (FeatureTrigger trigger in Triggers)
            {
                trigger.OnUpdate();
            }
        }

        /// <summary>
        /// 执行一次效果
        /// </summary>
        internal bool Invoke<TArg>(IAbilityTarget triggerTarget, in TArg arg) where TArg : struct, IFeatureTriggerArg
        {
            if (Condition.Verify(m_Ability) == false)
            {
                return false;
            }

            using (TargetCollector collector = TargetCollector.Collect(m_CacheTargets, m_Ability, Targets, triggerTarget, arg))
            {
                TargetCollection targetCollection = new TargetCollection(collector.NewTargetList, collector.OldTargetList);
                foreach (TriggerFeatureEffect effect in Effects)
                {
                    if (arg is EmptyArg == false && effect is TriggerFeatureEffect<TArg> effectWithArg)
                    {
                        effectWithArg.Invoke(arg, targetCollection);
                    }
                    else
                    {
                        effect.Invoke(targetCollection);
                    }
                }
            }

            return true;
        }
    }
}