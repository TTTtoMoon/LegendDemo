using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("动态矩形范围")]
    public sealed class DynamicSquareArea : DynamicArea
    {
        [Description("原点X偏移")]
        public AnimationCurve CenterOffsetX = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("原点Y偏移")]
        public AnimationCurve CenterOffsetY = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("角度偏移")]
        public AnimationCurve AngleOffset = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("长")] 
        public AnimationCurve Length = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Description("宽")] 
        public AnimationCurve Width = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation, float normalizedTime)
        {
            Vector3    centerOffset = new Vector3(CenterOffsetX.Evaluate(normalizedTime), 0f, CenterOffsetY.Evaluate(normalizedTime));
            Vector3    halfSize     = new Vector3(Width.Evaluate(normalizedTime) / 2f,    0f, Length.Evaluate(normalizedTime) / 2f);
            Quaternion angleOffset  = Quaternion.Euler(0f, AngleOffset.Evaluate(normalizedTime), 0f);
            Box        box          = new Box(position + rotation * centerOffset, rotation * angleOffset, halfSize);
            return CrashCaster.Overlay(box);
        }
    }
}