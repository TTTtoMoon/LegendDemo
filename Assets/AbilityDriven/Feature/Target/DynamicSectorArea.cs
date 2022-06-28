using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("动态扇形范围")]
    public sealed class DynamicSectorArea : DynamicArea
    {
        [Description("原点X偏移")]
        public AnimationCurve CenterOffsetX = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("原点Y偏移")]
        public AnimationCurve CenterOffsetY = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("内圆半径")]
        public AnimationCurve InsideRadius = AnimationCurve.Constant(0f, 0f, 0f);
        
        [Description("外圆半径")]
        public AnimationCurve OuterRadius = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("起始角度", "正前方为0度，左负右正")]
        public AnimationCurve StartAngle = AnimationCurve.Constant(0f, 0f, 0f);
        
        [Description("结束角度", "正前方为0度，左负右正")]
        public AnimationCurve EndAngle = AnimationCurve.Constant(0f, 1f, 0f);
    
        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation, float normalizedTime)
        {
            Vector3     center        = position + new Vector3(CenterOffsetX.Evaluate(normalizedTime), 0f, CenterOffsetY.Evaluate(normalizedTime));
            Sphere      sphere        = new Sphere(center, OuterRadius.Evaluate(normalizedTime));
            CrashResult crashResult   = CrashCaster.Overlay(sphere);
            float       insideRadius2 = InsideRadius.Evaluate(normalizedTime);
            insideRadius2 *= insideRadius2;
            Vector3 startDirection = rotation * Quaternion.Euler(0f, StartAngle.Evaluate(normalizedTime), 0f) * Vector3.forward;
            Vector3 endDirection   = rotation * Quaternion.Euler(0f, EndAngle.Evaluate(normalizedTime),   0f) * Vector3.forward;
            for (int i = crashResult.Count - 1; i >= 0; i--)
            {
                IAbilityTarget target    = crashResult[i];
                Vector3        offset    = (target.Position - center).IgnoreY();
                float          distance2 = offset.sqrMagnitude;
                // 在内圈内，排除 || 在角度范围外
                if (distance2 < insideRadius2 || Vector3.Dot(startDirection, offset) * Vector3.Dot(offset, endDirection) > 0)
                {
                    crashResult.RemoveAt(i);
                }
            }
            
            return crashResult;
        }
    }
}