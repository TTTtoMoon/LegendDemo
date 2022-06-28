using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态矩形范围内全部目标")]
    public class StaticSquareArea : StaticArea
    {
        [Description("原点偏移")]
        public Vector2 CenterOffset = Vector2.zero;
        
        [Description("角度偏移")]
        public float AngleOffset = 0f;
        
        [Description("长")] 
        public float Length = 0f;
        
        [Description("宽")] 
        public float Width = 0f;

        protected override CrashResult Collect(in Vector3 position, in Quaternion rotation)
        {
            Vector3    centerOffset = new Vector3(CenterOffset.x, 0f, CenterOffset.y);
            Vector3    halfSize     = new Vector3(Width / 2f, 0f, Length / 2f);
            Quaternion angleOffset  = Quaternion.Euler(0f, AngleOffset, 0f);
            Box        box          = new Box(position + rotation * centerOffset, rotation * angleOffset, halfSize);
            return CrashCaster.Overlay(box);
        }
    }
}