using System;

namespace Abilities
{
    /// <summary>
    /// 功能效果
    /// </summary>
    [Serializable]
    public abstract class TriggerFeatureEffect
    {
        /// <summary>
        /// 所属能力
        /// </summary>
        protected internal Ability Ability { get; internal set; }

        /// <summary>
        /// 当Feature激活时
        /// </summary>
        protected internal abstract void OnEnable();
        
        /// <summary>
        /// 当Feature禁用时
        /// </summary>
        protected internal abstract void OnDisable();

        /// <summary>
        /// Feature释放数据
        /// </summary>
        protected internal virtual void OnDispose()
        {
        }
        
        /// <summary>
        /// 生效
        /// </summary>
        /// <param name="targets">目标集合</param>
        protected internal abstract void Invoke(TargetCollection targets);
    }
    
    /// <summary>
    /// 功能效果
    /// </summary>
    [Serializable]
    public abstract class TriggerFeatureEffect<TArg> : TriggerFeatureEffect where TArg : struct, IFeatureTriggerArg
    {
        protected internal sealed override void Invoke(TargetCollection targets)
        {
        }

        /// <summary>
        /// 生效
        /// </summary>
        /// <param name="arg">触发参数</param>
        /// <param name="targets">目标集合</param>
        protected internal abstract void Invoke(TArg arg, TargetCollection targets);
    }
}