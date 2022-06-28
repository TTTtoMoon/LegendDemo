using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("球体效果器配置")]
    public sealed class SphereOrbInfo : OrbInfo
    {
        [Description("碰撞中心偏移")]
        public Vector3 Center;

        [Description("碰撞半径")]
        public float Radius;
        
        public override Collider CreateTrigger(GameObject orb)
        {
            SphereCollider trigger = orb.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.center    = Center;
            trigger.radius    = Radius;
            return trigger;
        }
    }
}