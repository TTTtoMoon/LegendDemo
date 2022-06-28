namespace Abilities
{
    /// <summary>
    /// 能力功能
    /// 为了兼容热更，所以设计为接口，IFix仅支持热更代码实现原有的接口，而不支持继承原有类
    /// </summary>
    public interface IAbilityFeature
    {
        /// <summary>
        /// 当启用功能时
        /// 请勿手动调用
        /// </summary>
        /// <param name="ability"></param>
        void OnEnable(Ability ability);
        
        /// <summary>
        /// 当禁用功能时
        /// 请勿手动调用
        /// </summary>
        void OnDisable();
        
        /// <summary>
        /// 功能释放数据
        /// 请勿手动调用
        /// </summary>
        void OnDispose();
        
        /// <summary>
        /// 当更新功能时
        /// 请勿手动调用
        /// </summary>
        void OnUpdate();
    }
}