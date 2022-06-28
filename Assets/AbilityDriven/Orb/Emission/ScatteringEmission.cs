using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.AbilityDriven.TriggerEffect;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [EffectWithoutTarget()]
    [Description("发射效果器：散射", "不配置目标时将朝前方发射，有目标时会对每个目标都进行一次全量发射。")]
    public sealed class ScatteringEmission : OrbEmission
    {
        [Description("散射方向")] 
        public EmitDirection Direction;

        [Description("散射起点")]
        public FeatureTargetOrigin Origin;

        [Description("始终使用当前位置进行发射？")]
        public bool FollowEmit;

        [Description("角度范围(度)")] [Range(0f, 360f)]
        public float AngleRange;

        [Description("散射中心偏移(度)")] [MinMaxSlider(-180f, 180f, true)]
        public Vector2 AngleOffset;

        [Description("中间的子弹角度偏移")] [MinMaxSlider(-180f, 180f, true)]
        public Vector2 CenterAngleOffset;
        
        [Description("发射点高度偏移(米)")]
        public float EmitOffsetY;
        
        [Description("发射点距离偏移(米)")]
        public float EmitOffsetZ;

        [Description("发射数量")] 
        public AbilityVariable EmitCount = new AbilityVariable(1);

        [Description("发射间隔(秒)")] [Min(0f)] 
        public float EmitSpan;

        [Description("多重数量区间", "在原本一次散射的基础上额外发射多次")]
        public Vector2Int MultipleRange = new Vector2Int(0, 0);

        [Description("多重间隔(秒)")]
        public float MultipleSpan;

        [Description("随机目标点距离")]
        public Vector2 RandomDestinationOffset;

        [Description("为每颗子弹随机目标距离")]
        public bool RandomDestinationForEveryOne;

        [Description("目标点随机范围半径")] [Min(0f)]
        public float RandomDestinationInRange = 0f;

        protected override void Invoke(TargetCollection targets)
        {
            if (targets.Count == 0)
            {
                ScatterFromTo(Ability.Owner, Ability.Owner);
                return;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                switch (Origin)
                {
                    case FeatureTargetOrigin.Owner:
                        ScatterFromTo(Ability.Owner, targets[i]);
                        break;
                    case FeatureTargetOrigin.Source:
                        ScatterFromTo(Ability.Giver == null ? null : Ability.Giver.Owner, targets[i]);
                        break;
                    case FeatureTargetOrigin.TriggerTarget:
                        ScatterFromTo(targets[i], targets[i]);
                        break;
                    case FeatureTargetOrigin.SearchedTarget:
                        ScatterFromTo(Ability.Target, targets[i]);
                        break;
                }
            }
        }

        public override void PreviewTrajectory(List<PreviewData> previewList)
        {
            Quaternion direction = Ability.Owner.Rotation;
            Vector3    position  = Ability.Owner.Position;
            position.y += EmitOffsetY;
            Vector3 forwardOffset = new Vector3(0f, 0f, EmitOffsetZ);
            float   angleOffset   = Random.Range(AngleOffset.x, AngleOffset.y);

            switch (Direction)
            {
                case EmitDirection.LeftToRight:
                    ScatterUniformly(-AngleRange / 2f + angleOffset, AngleRange / 2f + angleOffset);
                    break;
                case EmitDirection.RightToLeft:
                    ScatterUniformly(AngleRange / 2f + angleOffset, -AngleRange / 2f + angleOffset);
                    break;
                case EmitDirection.FullyRandom:
                    Debugger.LogError($"{Ability.ConfigurationID}随机朝向模式不支持预览。");
                    break;
                default:
                    Debugger.LogError($"{Ability.ConfigurationID}的散射模式无法识别。");
                    break;
            }

            void ScatterUniformly(float startAngle, float endAngle)
            {
                if (EmitCount > 1)
                {
                    for (int i = 0, length = EmitCount.IntValue - 1; i <= length; i++)
                    {
                        float angle = Mathf.Lerp(startAngle, endAngle, (float)i / length);
                        if (i > 0 && i + 1 < EmitCount.IntValue)
                        {
                            angle += Random.Range(CenterAngleOffset.x, CenterAngleOffset.y);
                        }

                        Quaternion rotation = direction * Quaternion.Euler(0f, angle, 0f);
                        Collect(rotation);
                    }
                }
                else
                {
                    Collect(direction);
                }
            }

            void Collect(Quaternion shotDirection)
            {
                Vector3 startPosition = position + shotDirection * forwardOffset;
                previewList.Add(new PreviewData(startPosition, shotDirection));
            }
        }

        private void ScatterFromTo(IAbilityTarget origin, IAbilityTarget target)
        {
            if (origin == null)
            {
                origin = Ability.Owner;
            }

            Quaternion direction;
            float      distance;
            if (ReferenceEquals(origin, target))
            {
                direction = origin.Rotation;
                distance  = Ability.Target != null ? (Ability.Target.Position - origin.Position).magnitude : 0f;
            }
            else
            {
                Vector3 offset = target.Position - origin.Position;
                direction = offset.ToRotation();
                distance  = offset.magnitude;
            }

            int multipleTimes = 1 + Random.Range(MultipleRange.x, MultipleRange.y + 1);
            Scatter(target, direction, distance, multipleTimes);
        }

        private void Scatter(IAbilityTarget target, Quaternion targetDirection, float distance, int multipleTimes)
        {
            Vector3 startPosition   = Ability.Owner.Position;
            Vector3 forwardOffset   = new Vector3(0f, 0f, EmitOffsetZ);
            float   angleOffset     = Random.Range(AngleOffset.x, AngleOffset.y);
            if (RandomDestinationInRange > float.Epsilon)
            {
                Vector2 randomDestination = RandomDestinationInRange * Random.insideUnitCircle;
                Vector3 destination       = startPosition + targetDirection * Vector3.forward * distance;
                destination.x += randomDestination.x;
                destination.z += randomDestination.y;
                Vector3 offset = destination - startPosition;
                targetDirection = Quaternion.LookRotation(offset);
                distance        = offset.magnitude;
            }

            switch (Direction)
            {
                case EmitDirection.LeftToRight:
                    StartEmitTask(ScatterUniformly(-AngleRange / 2f + angleOffset, AngleRange / 2f + angleOffset));
                    break;
                case EmitDirection.RightToLeft:
                    StartEmitTask(ScatterUniformly(AngleRange / 2f + angleOffset, -AngleRange / 2f + angleOffset));
                    break;
                case EmitDirection.FullyRandom:
                    StartEmitTask(ScatterRandomly());
                    break;
                default:
                    Debugger.LogError($"{Ability.ConfigurationID}的散射模式无法识别。");
                    break;
            }

            IEnumerator ScatterUniformly(float startAngle, float endAngle)
            {
                for (int i = 0; i < multipleTimes; i++)
                {
                    yield return ScatterOnce();
                    if (MultipleSpan > 0)
                    {
                        yield return new WaitForSeconds(MultipleSpan);
                    }
                }

                IEnumerator ScatterOnce()
                {
                    float firstRandomDistance = RandomDistance();
                    if (EmitCount > 1)
                    {
                        for (int i = 0, length = EmitCount.IntValue - 1; i <= length; i++)
                        {
                            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / length);
                            if (i > 0 && i + 1 < EmitCount.IntValue)
                            {
                                angle += Random.Range(CenterAngleOffset.x, CenterAngleOffset.y);
                            }

                            Quaternion rotation = targetDirection * Quaternion.Euler(0f, angle, 0f);
                            ScatterEmit(rotation, RandomDestinationForEveryOne ? RandomDistance() : firstRandomDistance);
                            if (EmitSpan > float.Epsilon)
                            {
                                yield return new WaitForSeconds(EmitSpan);
                            }
                        }
                    }
                    else
                    {
                        float      angle    = startAngle + Random.Range(CenterAngleOffset.x, CenterAngleOffset.y);
                        Quaternion rotation = targetDirection * Quaternion.Euler(0f, angle, 0f);
                        ScatterEmit(rotation, firstRandomDistance);
                    }
                }
            }

            IEnumerator ScatterRandomly()
            {
                for (int i = 0; i < multipleTimes; i++)
                {
                    yield return ScatterOnce();
                    if (MultipleSpan > 0)
                    {
                        yield return new WaitForSeconds(MultipleSpan);
                    }
                }

                IEnumerator ScatterOnce()
                {
                    float firstRandomDistance = RandomDistance();
                    for (int i = 0; i < EmitCount; i++)
                    {
                        float randomAngle = Random.Range(angleOffset - AngleRange / 2f, angleOffset + AngleRange / 2f);
                        if (i > 0 && i + 1 < EmitCount)
                        {
                            randomAngle += Random.Range(CenterAngleOffset.x, CenterAngleOffset.y);
                        }

                        Quaternion rotation = targetDirection * Quaternion.Euler(0f, randomAngle, 0f);
                        ScatterEmit(rotation, RandomDestinationForEveryOne ? RandomDistance() : firstRandomDistance);
                        if (EmitSpan > float.Epsilon)
                        {
                            yield return new WaitForSeconds(EmitSpan);
                        }
                    }
                }
            }

            float RandomDistance()
            {
                return distance + Random.Range(RandomDestinationOffset.x, RandomDestinationOffset.y);
            }

            void ScatterEmit(Quaternion direction, float targetDistance)
            {
                Vector3 position = FollowEmit ? Ability.Owner.Position : startPosition;
                position.y += EmitOffsetY;
                Vector3 emitPosition   = position     + direction * forwardOffset;
                Vector3 targetPosition = emitPosition + direction * Vector3.forward * targetDistance;
                Emit(emitPosition, direction, target, targetPosition);
            }
        }

        public enum EmitDirection
        {
            [InspectorName("从左往右")] LeftToRight,
            [InspectorName("从右往左")] RightToLeft,
            [InspectorName("完全随机")] FullyRandom,
        }
    }
}