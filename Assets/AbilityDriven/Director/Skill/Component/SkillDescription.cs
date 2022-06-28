using System;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    public abstract partial class SkillDescription : AbilityComponent
    {
        [Description("动画")] [CustomValueDrawer("DrawState")]
        public AnimationProperty State;

        [Description("启动动画位移")]
        public bool EnableRootMotion;
        
        [Description("效果时间段")]
        public Vector2 ActingTime;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}