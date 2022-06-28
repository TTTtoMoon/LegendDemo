using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.AbilityDriven.Feature;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    public abstract class DynamicArea : FeatureTarget<TickerArg>
    {
        [Description("是否包括起始目标")]
        public bool IncludeOrigin = true;
        
        [Description("目标筛选")]
        public Filter<IAbilityTarget> Filter = new Filter<IAbilityTarget>();

        protected sealed override void Collect(IAbilityTarget origin, TickerArg arg, TargetCollector collector)
        {
            if (IncludeOrigin)
            {
                collector.Append(origin);
            }

            using (CrashResult result = Collect(origin.Position, origin.Rotation, arg.NormalizedTime))
            {
                for (int i = 0; i < result.Count; i++)
                {
                    IAbilityTarget target = result[i];
                    if (target != origin && Filter.Verify(target))
                    {
                        collector.Append(target);
                    }
                }
            }
        }

        protected abstract CrashResult Collect(in Vector3 position, in Quaternion rotation, float normalizedTime);
    }
}