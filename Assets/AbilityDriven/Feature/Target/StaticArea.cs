using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    public abstract class StaticArea : FeatureTarget
    {
        [Description("是否包括起始目标")]
        public bool IncludeOrigin = true;
        
        [Description("目标筛选")]
        public Filter<IAbilityTarget> Filter = new Filter<IAbilityTarget>();

        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            if (IncludeOrigin)
            {
                collector.Append(origin);
            }

            using (CrashResult result = Collect(origin.Position, origin.Rotation))
            {
                for (int i = 0; i < result.Count; i++)
                {
                    IAbilityTarget target = result[i];
                    if (!ReferenceEquals(target, origin) && Filter.Verify(target))
                    {
                        collector.Append(target);
                    }
                }
            }
        }

        protected abstract CrashResult Collect(in Vector3 position, in Quaternion rotation);
    }
}