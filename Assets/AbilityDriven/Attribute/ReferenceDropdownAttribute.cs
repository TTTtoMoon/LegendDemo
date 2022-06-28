using System;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 引用对象下拉菜单
    /// 配合SerializeReference
    /// </summary>
    public sealed class ReferenceDropdownAttribute : Attribute
    {
        public readonly bool CanBeNull;

        public ReferenceDropdownAttribute(bool canBeNull = false)
        {
            CanBeNull = canBeNull;
        }
    }
}