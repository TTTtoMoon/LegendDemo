using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("技能预警")]
    public class SkillWarning : TriggerFeatureEffect
    {
        [Description("预警曲线", "x时间 y预警值")]
        public AnimationCurve WarningCurve;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
        }
    }
}