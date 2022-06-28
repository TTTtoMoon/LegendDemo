using System;
using Abilities;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("销毁自己")]
    public class DestroyWhenHitWall : OrbWhenHitWall
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
            GameManager.OrbSystem.Destroy(orb);
        }
    }
}