using System;

namespace Abilities
{
    /// <summary>
    /// 效果撤销委托
    /// </summary>
    public delegate void EffectRevocation();

    /// <summary>
    /// 功能可撤销效果
    /// </summary>
    [Serializable]
    public abstract class PassivityFeatureEffect
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
        /// 效果生效
        /// </summary>
        /// <param name="target">效果目标</param>
        /// <returns>返回撤销效果的回调</returns>
        protected internal abstract EffectRevocation Invoke(IAbilityTarget target);
    }
}