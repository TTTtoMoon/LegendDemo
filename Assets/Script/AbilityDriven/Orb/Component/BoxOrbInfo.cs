using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("方体效果器数据")]
    public sealed class BoxOrbInfo : OrbInfo
    {
        [Description("碰撞中心偏移")] 
        public Vector3 Center;

        [Description("碰撞大小")] 
        public Vector3 Size;

        public override Collider CreateTrigger(GameObject orb)
        {
            BoxCollider trigger = orb.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center    = Center;
            trigger.size      = Size;
            return trigger;
        }
    }
}