using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("周期触发-参数" + nameof(TickerArg))]
    public sealed class Ticker : FeatureTrigger<TickerArg>
    {
        [Description("起始时间", "<=0表示生效时立即开始")]
        public AbilityVariable StartTime = new AbilityVariable(0f);
        
        [Description("持续时间", "<=0表示永久")]
        public AbilityVariable Duration = new AbilityVariable(0f);
        
        [Description("周期间隔", "<=0表示始终触发")]
        public AbilityVariable Interval = new AbilityVariable(0f);

        [Description("首帧是否执行")]
        public bool TickImmediately = true;

        private int   m_TickIndex;
        private float m_PreviousTime;

        private int TickIndexForCalculate => TickImmediately ? m_TickIndex + 1 : m_TickIndex;

        protected override void OnEnable()
        {
            m_TickIndex    = 0;
            m_PreviousTime = -1;
        }

        protected override void OnDisable()
        {
        }

        protected override void OnUpdate()
        {
            if (Ability.Time < StartTime || m_PreviousTime > StartTime + Duration)
            {
                return;
            }

            float now = Ability.Time < StartTime + Duration ? Ability.Time - StartTime : Duration;
            if (Interval > 0f)
            {
                while (TickIndexForCalculate * Interval < now)
                {
                    InvokeEffect(new TickerArg(now, m_TickIndex, Duration));
                    m_TickIndex++;
                }
            }
            else
            {
                InvokeEffect(new TickerArg(now, m_TickIndex, Duration));
                m_TickIndex++;
            }

            m_PreviousTime = Ability.Time;
        }
    }

    /// <summary>
    /// 周期参数
    /// </summary>
    public readonly struct TickerArg : IFeatureTriggerArg
    {
        public readonly float Time;
        public readonly float NormalizedTime;
        public readonly int   TickIndex;

        public TickerArg(float time, int tickIndex, float duration)
        {
            Time           = time;
            NormalizedTime = duration > 0 ? time / duration : 0f;
            TickIndex      = tickIndex;
        }
    }
}