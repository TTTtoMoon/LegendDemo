using System;
using Abilities;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("不做任何处理(即可穿墙)")]
    public class DoNothingWhenHitWall : OrbWhenHitWall
    {
        [Description("碰到地图外围墙也不销毁?")]
        public bool IncludeOuterWall;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        public override void Handle(Orb orb, RaycastHit hit)
        {
            if (IncludeOuterWall) return;
            Vector3 dir = hit.point                  - orb.Position;
            
            if (Physics.Linecast(orb.Position, hit.point + 0.05f * dir.normalized, out hit, LayerDefine.Wall.Mask))
            {
                orb.transform.position = hit.point;
                GameManager.OrbSystem.Destroy(orb);
            }
        }
    }
}