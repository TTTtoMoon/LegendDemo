using System;

namespace Abilities
{
    /// <summary>
    /// 用于FeatureEffect，表示该他可不需要Target
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EffectWithoutTargetAttribute : Attribute
    {
    }
}