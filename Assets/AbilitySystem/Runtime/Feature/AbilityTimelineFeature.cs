using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [Description("时间轴")]
    public sealed class AbilityTimelineFeature : IAbilityFeature
    {
        [SerializeField] [LabelText("关键帧")] 
        [ListDrawerSettings(DraggableItems = false, AlwaysAddDefaultValue = true, ListElementLabelName = "Label", Expanded = true)]
        public Frame[] Frames = new Frame[0];
        
        internal Ability              m_Ability;
        internal float                m_PreviousTime;
        internal List<IAbilityTarget> m_CacheTargets = new List<IAbilityTarget>();
        
        void IAbilityFeature.OnEnable(Ability ability)
        {
            m_Ability      = ability;
            m_PreviousTime = -1f;
            
            for (int i = 0, length = Frames.Length; i < length; i++)
            {
                for (int j = 0; j < Frames[i].Effects.Length; j++)
                {
                    Frames[i].Effects[j].Ability = ability;
                    Frames[i].Effects[j].OnEnable();
                }
            }
        }

        void IAbilityFeature.OnDisable()
        {
            for (int i = 0, length = Frames.Length; i < length; i++)
            {
                for (int j = 0; j < Frames[i].Effects.Length; j++)
                {
                    Frames[i].Effects[j].OnDisable();
                }
            }
        }

        void IAbilityFeature.OnDispose()
        {
            for (int i = 0, length = Frames.Length; i < length; i++)
            {
                for (int j = 0; j < Frames[i].Effects.Length; j++)
                {
                    Frames[i].Effects[j].OnDispose();
                    Frames[i].Effects[j].Ability = null;
                }
            }
            
            m_CacheTargets.Clear();
            m_Ability = null;
        }

        void IAbilityFeature.OnUpdate()
        {
            for (int i = 0, length = Frames.Length; i < length; i++)
            {
                ref Frame frame = ref Frames[i];
                if (m_PreviousTime >= frame.Time || m_Ability.Time < frame.Time)
                {
                    continue;
                }

                m_CacheTargets.Clear();// 每次都视为新执行的目标
                using (TargetCollector collector = TargetCollector.Collect(m_CacheTargets, m_Ability, frame.Targets, null, EmptyArg.Empty))
                {
                    TargetCollection targetCollection = new TargetCollection(collector.NewTargetList, collector.OldTargetList);
                    foreach (TriggerFeatureEffect effect in frame.Effects)
                    {
                        effect.Invoke(targetCollection);
                    }
                }
            }

            m_PreviousTime = m_Ability.Time;
        }

        [Serializable]
        public class Frame
        {
#if UNITY_EDITOR
            internal string Label => $"在{Time}秒时";
#endif
            
            [Description("时间点")] [Min(0f)]
            public float Time;

            [SerializeReference] [ReferenceCollection] [Description("目标选择")]
            public FeatureTarget[] Targets = new FeatureTarget[0];

            [SerializeReference] [ReferenceCollection] [Description("效果")]
            public TriggerFeatureEffect[] Effects = new TriggerFeatureEffect[0];
        }
    }
}