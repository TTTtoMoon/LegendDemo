using System;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 功能目标
    /// </summary>
    [Serializable]
    public abstract class FeatureTarget
    {
        [SerializeField] [Description("起始目标")]
        internal FeatureTargetOrigin Origin;
        
        /// <summary>
        /// 收集目标
        /// </summary>
        /// <param name="origin">源点(必然非空)</param>
        /// <param name="collector">目标收集器</param>
        protected internal abstract void Collect(IAbilityTarget origin, TargetCollector collector);
    }

    /// <summary>
    /// 功能目标
    /// </summary>
    [Serializable]
    public abstract class FeatureTarget<TArg> : FeatureTarget where TArg : struct, IFeatureTriggerArg
    {
        [Obsolete("请勿使用该接口", true)]
        protected internal sealed override void Collect(IAbilityTarget origin, TargetCollector collector)
        {
        }

        /// <summary>
        /// 收集目标
        /// </summary>
        /// <param name="origin">源点(必然非空)</param>
        /// <param name="arg">参数</param>
        /// <param name="collector">目标收集器</param>
        protected internal abstract void Collect(IAbilityTarget origin, TArg arg, TargetCollector collector);
    }
}