using System;

namespace Abilities
{
    /// <summary>
    /// 表示该类需要Ability拥有对应的Component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NeedComponentAttribute : Attribute
    {
        public NeedComponentAttribute(params Type[] components)
        {
            Components = components;
        }

        public readonly Type[] Components;
    }
}