using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态圆形范围内随机目标")]
    public sealed class StaticCircleAreaFindRandom : StaticCircleArea
    {
        [Description("目标数量")]
        public int TargetCount;
        
        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            using (CrashResult result = Collect(origin.Position, origin.Rotation))
            {
                List<IAbilityTarget> validTargets = ListPool<IAbilityTarget>.Get();
                for (int i = 0; i < result.Count; i++)
                {
                    var target = result[i];
                    if (((ReferenceEquals(target, origin) && IncludeOrigin) || !ReferenceEquals(target, origin)) && Filter.Verify(target))
                    {
                        validTargets.Add(target);
                    }
                }

                if (validTargets.Count <= TargetCount)
                {
                    for (int i = 0; i < validTargets.Count; i++)
                    {
                        collector.Append(validTargets[i]);
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
                
                ListPool<IAbilityTarget>.Release(validTargets);
            }
        }
    }
}