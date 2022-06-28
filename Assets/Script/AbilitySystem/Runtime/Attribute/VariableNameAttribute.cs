using System;

namespace Abilities
{
    /// <summary>
    /// 自定义变量名，代替字段名称
    /// </summary>
    public sealed class VariableNameAttribute : Attribute
    {
        public VariableNameAttribute(string name)
        {
            Name = name;
        }

        public readonly string Name;
    }
}