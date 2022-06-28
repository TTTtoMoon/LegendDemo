using System;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("世界位置目标点")]
    public class WorldPoint : FeatureTarget
    {
        [Description("世界坐标")] [ListDrawerSettings(Expanded = true)]
        public Pair[] Pairs = new Pair[0];
        
        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            for (int i = 0, length = Pairs.Length; i < length; i++)
            {
                Pair pair = Pairs[i];
                collector.Append(new PointTarget(pair.WorldPosition, Quaternion.Euler(0f, pair.WorldRotation, 0f)));
            }
        }

        [Serializable]
        [InlineProperty]
        public struct Pair
        {
            [Description("世界坐标")] 
            public Vector3 WorldPosition;

            [Description("朝向")] [PropertyRange(-180f, 180f)]
            public float WorldRotation;
        }
    }
}