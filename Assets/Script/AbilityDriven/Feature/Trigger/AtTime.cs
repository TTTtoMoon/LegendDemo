using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("定时触发")]
    public sealed class AtTime : FeatureTrigger
    {
        [Description("时间点")]
        public AbilityVariable TimePoint = new AbilityVariable(0f);
        
        private float m_PreviousTime;

        protected override void OnEnable()
        {
            m_PreviousTime = -1f;
        }

        protected override void OnDisable()
        {
        }

        protected override void OnUpdate()
        {
            if (m_PreviousTime < TimePoint && Ability.Time >= TimePoint)
            {
                InvokeEffect();
            }

            m_PreviousTime = Ability.Time;
        }
    }
}