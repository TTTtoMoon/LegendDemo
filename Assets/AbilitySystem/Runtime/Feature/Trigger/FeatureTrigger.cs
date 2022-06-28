using System;

namespace Abilities
{
    /// <summary>
    /// 功能触发器
    /// </summary>
    [Serializable]
    public abstract class FeatureTrigger
    {
        /// <summary>
        /// 所属能力
        /// </summary>
        protected internal Ability Ability { get; internal set; }

        /// <summary>
        /// 所属功能
        /// </summary>
        internal AbilityTriggerFeature TriggerFeature { get; set; }

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
        /// 当Ability Update时
        /// </summary>
        protected internal abstract void OnUpdate();

        /// <summary>
        /// 执行效果(无触发目标)
        /// </summary>
        protected bool InvokeEffect()
            => InvokeEffect(null);

        /// <summary>
        /// 执行效果，带触发目标
        /// </summary>
        /// <param name="triggerTarget">触发目标</param>
        protected bool InvokeEffect(IAbilityTarget triggerTarget)
            => TriggerFeature.Invoke(triggerTarget, EmptyArg.Empty);
    }

    /// <summary>
    /// 带参数的功能触发器
    /// </summary>
    /// <typeparam name="TArg">参数类型</typeparam>
    [Serializable]
    public abstract class FeatureTrigger<TArg> : FeatureTrigger where TArg : struct, IFeatureTriggerArg
    {
        [Obsolete("请使用带参方法InvokeEffect(in TArg arg)", true)]
        protected new bool InvokeEffect()
            => InvokeEffect(null);

        [Obsolete("请使用带参方法InvokeEffect(IAbilityTarget triggerTarget, in TArg arg)", true)]
        protected new bool InvokeEffect(IAbilityTarget triggerTarget)
            => TriggerFeature.Invoke(triggerTarget, EmptyArg.Empty);
        
        /// <summary>
        /// 执行效果(无触发目标)
        /// </summary>
        /// <param name="arg">触发器参数</param>
        protected bool InvokeEffect(in TArg arg)
            => InvokeEffect(null, arg);

        /// <summary>
        /// 执行效果，带触发目标
        /// </summary>
        /// <param name="triggerTarget">触发目标</param>
        /// <param name="arg">触发器参数</param>
        protected bool InvokeEffect(IAbilityTarget triggerTarget, in TArg arg)
        {
            return TriggerFeature.Invoke(triggerTarget, arg);
        }
    }
}