using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using Unity.Mathematics;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("动态圆形范围")]
    public sealed class DynamicCircleArea : DynamicArea
    {
        [Description("原点X偏移")]
        public AnimationCurve CenterOffsetX = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Description("原点Y偏移")]
        public AnimationCurve CenterOffsetY = AnimationCurve.Constant(0f, 1f, 0f);

        [Description("半径")] 
        public AnimationCurve Radius = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation, float normalizedTime)
        {
            Vector3 centerOffset = new Vector3(CenterOffsetX.Evaluate(normalizedTime), 0f, CenterOffsetY.Evaluate(normalizedTime));
            Sphere  sphere       = new Sphere(position + centerOffset, Radius.Evaluate(normalizedTime));
            return CrashCaster.Overlay(sphere);
        }
    }
}