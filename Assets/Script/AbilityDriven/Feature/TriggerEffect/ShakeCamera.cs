using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("相机抖动")]
    public sealed class ShakeCamera : TriggerFeatureEffect
    {
        /// <summary>
        /// 抖动方式
        /// </summary>
        [Description("抖动方式")] 
        public CameraSystem.CameraOffsetType CameraOffsetType;

        /// <summary>
        /// 抖动曲线吗
        /// </summary>
        [Description("抖动曲线", "x:时间(秒) y:强度")]
        public AnimationCurve ShakeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            GameManager.CameraSystem.SetCameraDistance(ShakeCurve, CameraOffsetType);
        }
    }
}