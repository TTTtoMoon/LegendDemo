namespace Abilities
{
    /// <summary>
    /// 变量修改器
    /// </summary>
    public interface IAbilityVariableModifier
    {
        /// <summary>
        /// 应用修改
        /// </summary>
        /// <param name="sourceValue">原变量值</param>
        /// <param name="value">最终值</param>
        void Apply(float sourceValue, ref float value);
    }
}