using System;
using System.Diagnostics;

namespace Abilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [Conditional("UNITY_EDITOR")]
    public sealed class AbilityTagDefineAttribute : Attribute
    {
    }
}