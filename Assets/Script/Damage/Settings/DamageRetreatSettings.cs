using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay
{
    [Serializable]
    public struct DamageRetreatSetting
    {
        public static readonly DamageRetreatSetting Default = new DamageRetreatSetting()
        {
            BodyType             = BodyType.Thin,
            Curve                = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f),
        };

        [VerticalGroup("体型")] [HideLabel] 
        public BodyType BodyType;

        [VerticalGroup("速度曲线")] [HideLabel] 
        public AnimationCurve Curve;
    }
}