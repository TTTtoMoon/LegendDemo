namespace Abilities
{
    /// <summary>
    /// 变量表
    /// </summary>
    public interface IAbilityVariableTable
    {
        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="uniqueName">变量名</param>
        /// <param name="value">变量值</param>
        /// <returns>返回是否存在变量</returns>
        bool GetVariableValue(string uniqueName, out float value);
    }
}