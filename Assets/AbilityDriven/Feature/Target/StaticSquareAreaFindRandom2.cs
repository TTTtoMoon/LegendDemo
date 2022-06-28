using System;
using Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态矩形范围内随机目标，无目标则在另一个范围内随机点")]
    public sealed class StaticSquareAreaFindRandom2 : StaticSquareAreaFindRandom
    {
        [Description("无目标原点偏移")]
        public Vector2 NoTargetCenterOffset = Vector2.zero;
        
        [Description("无目标角度偏移")]
        public float NoTargetAngleOffset = 0f;
        
        [Description("无目标长")] 
        public float NoTargetLength = 0f;
        
        [Description("无目标宽")] 
        public float NoTargetWidth = 0f;

        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            base.Collect(origin, collector);
            for (int i = collector.Count; i < TargetCount; i++)
            {
                Vector3 point = new Vector3(Random.Range(-NoTargetLength, NoTargetLength), 0f, Random.Range(-NoTargetWidth, NoTargetWidth));
                point.x += NoTargetCenterOffset.x;
                point.y += NoTargetCenterOffset.y;
                Quaternion rotation = origin.Rotation * Quaternion.Euler(0f, NoTargetAngleOffset, 0f);
                point =  rotation * point;
                point += origin.Position;
                collector.Append(new PointTarget(point, rotation));
            }
        }
    }
}