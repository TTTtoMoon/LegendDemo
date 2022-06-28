using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态矩形范围内随机目标")]
    public class StaticSquareAreaFindRandom : StaticSquareArea
    {
        [Description("目标数量")]
        public int TargetCount;

        [Description("新增目标优先")]
        public bool NewTargetFirst;

        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            using (CrashResult result = Collect(origin.Position, origin.Rotation))
            {
                int                  newTargetCount = 0;
                List<IAbilityTarget> validTargets   = ListPool<IAbilityTarget>.Get();
                for (int i = 0; i < result.Count; i++)
                {
                    var target = result[i];
                    if (((ReferenceEquals(target, origin) && IncludeOrigin) || !ReferenceEquals(target, origin)) && Filter.Verify(target))
                    {
                        if (collector.IsOldTarget(target))
                        {
                            validTargets.Add(target);
                        }
                        else
                        {
                            validTargets.Insert(0, target);
                            newTargetCount++;
                        }
                    }
                }

                // 有效目标数量少于需求目标数量，直接把所有目标加进去
                if (validTargets.Count <= TargetCount)
                {
                    for (int i = 0; i < validTargets.Count; i++)
                    {
                        collector.Append(validTargets[i]);
                    }
                }
                else
                {
                    if (NewTargetFirst)
                    {
                        // 新增目标数量多余目标数量，则只在新增目标里随机
                        if (newTargetCount >= TargetCount)
                        {
                            for (int i = 0; i < TargetCount; i++)
                            {
                                int index = Random.Range(0, newTargetCount);
                                collector.Append(validTargets[index]);
                                validTargets.RemoveAt(index);
                                newTargetCount--;
                            }
                        }
                        // 否则新增目标全append，剩余的目标在老目标里随机
                        else
                        {
                            for (int i = 0; i < newTargetCount; i++)
                            {
                                collector.Append(validTargets[i]);
                            }

                            for (int i = 0; i < TargetCount; i++)
                            {
                                int index = Random.Range(newTargetCount, validTargets.Count);
                                collector.Append(validTargets[index]);
                                validTargets.RemoveAt(index);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TargetCount; i++)
                        {
                            int index = Random.Range(0, validTargets.Count);
                            collector.Append(validTargets[index]);
                            validTargets.RemoveAt(index);
                        }
                    }
                }

                ListPool<IAbilityTarget>.Release(validTargets);
            }
        }
    }
}