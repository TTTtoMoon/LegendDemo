using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态圆形范围内全部目标")]
    public class StaticCircleArea : StaticArea
    {
        [Description("原点偏移")]
        public Vector2 CenterOffset = Vector2.zero;
        
        [Description("半径")] 
        public float Radius = 0f;

        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation)
        {
            Vector3 centerOffset = new Vector3(CenterOffset.x, 0f, CenterOffset.y);
            Sphere  sphere       = new Sphere(position + centerOffset, Radius);
            return CrashCaster.Overlay(sphere);
        }
    }
}