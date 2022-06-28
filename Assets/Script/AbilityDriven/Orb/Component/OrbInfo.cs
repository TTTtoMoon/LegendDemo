using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    public abstract class OrbInfo : AbilityComponent
    {
        [Description("销毁特效")]
        public GameObject DestroyVFX;

        [Description("销毁特效偏移位置")]
        public Vector3 DestroyVFXOffset;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        public abstract Collider CreateTrigger(GameObject orb);
    }
}