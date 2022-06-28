using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    [Serializable]
    [Description("静态圆形范围内距离优先目标")]
    public sealed class StaticCircleAreaFindByDistance : StaticCircleArea
    {
        [Description("目标数量")]
        public int TargetCount;

        protected override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
            int counter = 0;
            using (CrashResult result = Collect(origin.Position, origin.Rotation))
            {
                for (int i = 0; i < result.Count; i++)
                {
                    IAbilityTarget target = result[i];
                    if (((target == origin && IncludeOrigin) || target != origin) && Filter.Verify(target))
                    {
                        collector.Append(target);
                        counter++;
                        if (counter >= TargetCount)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}