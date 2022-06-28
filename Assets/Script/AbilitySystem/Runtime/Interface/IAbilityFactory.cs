namespace Abilities
{
    /// <summary>
    /// 能力工厂
    /// 请使用实现该接口后的对象调用SetUp()，而不要直接使用该接口
    /// </summary>
    public interface IAbilityFactory
    {
        /// <summary>
        /// 分配实例
        /// </summary>
        /// <param name="configurationID">配置ID</param>
        /// <returns>返回实例</returns>
        Ability Allocate(int configurationID);

        /// <summary>
        /// 回收实例
        /// </summary>
        /// <param name="ability">实例</param>
        void Recycle(Ability ability);
    }
}