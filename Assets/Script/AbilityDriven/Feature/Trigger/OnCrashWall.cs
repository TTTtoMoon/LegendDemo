using System;
using Abilities;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当撞墙时(仅限Orb)")]
    public class OnCrashWall : FeatureTrigger
    {
        [Description("什么墙")] 
        public WallType WhatWall;

        private Action<RaycastHit> m_OnCrashWall;

        protected override void OnEnable()
        {
            if (Ability.Owner is Orb orb)
            {
                m_OnCrashWall      =  OnCrashWall;
                orb.CrashWallEvent += m_OnCrashWall;

                void OnCrashWall(RaycastHit hit)
                {
                    switch (WhatWall)
                    {
                        case WallType.OnlyOuterWall when hit.collider.gameObject.layer== LayerDefine.Wall.Index:
                            InvokeEffect();
                            break;
                        default:
                            InvokeEffect();
                            break;
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            if (Ability.Owner is Orb orb)
            {
                orb.CrashWallEvent -= m_OnCrashWall;
                m_OnCrashWall      =  null;
            }
        }

        protected override void OnUpdate()
        {
        }

        public enum WallType
        {
            [InspectorName("仅外墙")] OnlyOuterWall,
            [InspectorName("所有墙")] AllWall,
        }
    }
}