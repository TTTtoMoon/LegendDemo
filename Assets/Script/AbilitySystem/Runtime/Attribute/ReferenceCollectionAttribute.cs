using System;

namespace Abilities
{
    /// <summary>
    /// 表示其为引用类型集合
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReferenceCollectionAttribute : Attribute
    {
    }
}