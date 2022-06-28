using System;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public static class EnumCache<TEnum> where TEnum : Enum
    {
        static EnumCache()
        {
            m_Values = Enum.GetValues(typeof(TEnum)) as TEnum[];
        }

        private static readonly TEnum[] m_Values;

        public static IReadOnlyList<TEnum> Values     => m_Values;
        public static int                  ValueCount => Values.Count;

        public static int IndexOf(TEnum target)
        {
            return Array.IndexOf(m_Values, target);
        }
    }
}