using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态扇形范围")]
    public sealed class StaticSectorArea : DynamicArea
    {
        [Description("原点偏移")] 
        public Vector2 CenterOffset;

        [Description("内圆半径")] [Min(0f)]
        public float InsideRadius = 0f;

        [Description("外圆半径")] [Min(0f)]
        public float OuterRadius = 1f;

        [Description("起始角度", "正前方为0度，左负右正")] [Range(-180f, 180f)]
        public float StartAngle = -30;

        [Description("结束角度", "正前方为0度，左负右正")] [Range(-180f, 180f)]
        public float EndAngle = 30f;
    
        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation, float normalizedTime)
        {
            Vector3     center         = position + new Vector3(CenterOffset.x, 0f, CenterOffset.y);
            Sphere      sphere         = new Sphere(center, OuterRadius);
            CrashResult crashResult    = CrashCaster.Overlay(sphere);
            float       insideRadius2  = InsideRadius * InsideRadius;
            Vector3     startDirection = rotation     * Quaternion.Euler(0f, StartAngle, 0f) * Vector3.forward;
            Vector3     endDirection   = rotation     * Quaternion.Euler(0f, EndAngle,   0f) * Vector3.forward;
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