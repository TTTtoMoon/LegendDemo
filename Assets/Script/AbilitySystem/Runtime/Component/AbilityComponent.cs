using System;

namespace Abilities
{
    /// <summary>
    /// 能力组件
    /// </summary>
    [Serializable]
    public abstract class AbilityComponent
    {
        /// <summary>
        /// 所属能力
        /// </summary>
        protected internal Ability Ability { get; internal set; }

        /// <summary>
        /// 当能力启用时
        /// </summary>
        protected internal abstract void OnEnable();
        
        /// <summary>
        /// 当能力禁用时
        /// </summary>
        protected internal abstract void OnDisable();

        /// <summary>
        /// 释放能力数据
        /// </summary>
        protected internal virtual void OnDispose()
        {
        }
    }
}