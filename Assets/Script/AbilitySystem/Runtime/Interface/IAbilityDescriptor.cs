namespace Abilities
{
    /// <summary>
    /// 能力描述符
    /// </summary>
    public interface IAbilityDescriptor
    {
        /// <summary>
        /// 能力ID
        /// </summary>
        int AbilityID { get; }

        /// <summary>
        /// 变量表
        /// </summary>
        IAbilityVariableTable VariableTable { get; }
    }
}