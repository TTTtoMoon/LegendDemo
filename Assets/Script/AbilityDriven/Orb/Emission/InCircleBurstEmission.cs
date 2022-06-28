using System;
using System.Collections;
using Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("发射效果器：圆形范围内迸发n颗子弹")]
    public sealed class InCircleBurstEmission : OrbEmission
    {
        [Description("圆心偏移")]
        public Vector3 CenterOffset;

        [Description("发射点偏移")]
        public Vector2 PositionOffset;
        
        [Description("发射位置角度范围")] [Range(0f, 360f)]
        public float PositionAngle;

        [Description("发射朝向角度范围")] [Range(0f, 360f)]
        public float EmitAngle;
        
        [Description("发射朝向偏移")] [Range(-180f, 180f)]
        public float EmitAngleOffset;

        [Description("发射数量")]
        public AbilityVariable EmitCount = new AbilityVariable(1);
        
        [Description("发射间隔(秒)")] [Min(0f)] 
        public float EmitSpan;
        
        [Description("多重数量区间", "在原本一次散射的基础上额外发射多次")]
        public Vector2Int MultipleRange = new Vector2Int(0, 0);

        [Description("多重间隔(秒)")]
        public float MultipleSpan;

        [Description("飞行距离")]
        public Vector2 Distance;

        [Description("每颗子弹随机飞行距离？")]
        public bool RandomDistanceForEveryOne;
        
        protected override void Invoke(TargetCollection targets)
        {
            StartEmitTask(Multiple(1 + Random.Range(MultipleRange.x, MultipleRange.y + 1)));

            IEnumerator Multiple(int multipleTimes)
            {
                for (int i = 0; i < multipleTimes; i++)
                {
                    StartEmitTask(Burst());
                    if (MultipleSpan > 0)
                    {
                        yield return new WaitForSeconds(MultipleSpan);
                    }
                }
            }

            IEnumerator Burst()
            {
                float      firstRandomDistance = RandomDistance();
                Quaternion centerRotation      = Ability.Owner.Rotation * Quaternion.Euler(0f, EmitAngleOffset, 0f);
                Vector3    centerPosition      = Ability.Owner.Position;
                centerPosition += centerRotation * CenterOffset;
                for (int i = 0; i < EmitCount; i++)
                {
                    Quaternion posRotation    = centerRotation * Quaternion.Euler(0f, Random.Range(-PositionAngle / 2f, PositionAngle / 2f), 0f);
                    Vector3    emitPosition   = centerPosition + posRotation * Vector3.forward * Random.Range(PositionOffset.x, PositionOffset.y);
                    Quaternion emitRotation   = centerRotation * Quaternion.Euler(0f, Random.Range(-EmitAngle / 2f, EmitAngle / 2f), 0f);
                    Vector3    targetPosition = emitPosition + emitRotation * Vector3.forward * (RandomDistanceForEveryOne ? RandomDistance() : firstRandomDistance);
                    Emit(emitPosition, emitRotation, null, targetPosition);
                    if (EmitSpan > 0f)
                    {
                        yield return new WaitForSeconds(EmitSpan);
                    }
                }
            }

            float RandomDistance()
            {
                return Random.Range(Distance.x, Distance.y);
            }
        }
    }
}