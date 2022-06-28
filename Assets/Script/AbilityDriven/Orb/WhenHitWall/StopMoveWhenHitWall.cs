using System;
using Abilities;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("效果器撞墙时：停止移动")]
    public sealed class StopMoveWhenHitWall : OrbWhenHitWall
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
        
        public override void Handle(Orb orb, RaycastHit hit)
        {
            orb.transform.position = hit.point;
            orb.Mode.StopMove();
        }
    }
}