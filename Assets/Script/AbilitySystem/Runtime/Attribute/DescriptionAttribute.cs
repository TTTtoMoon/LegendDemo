using System;

namespace Abilities
{
    /// <summary>
    /// 对类或字段填写描述性语句
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string name, string toolTip = null)
        {
            Name    = name;
            ToolTip = toolTip;
        }

        public readonly string Name;
        public readonly string ToolTip;
    }
}