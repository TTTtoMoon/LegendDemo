using System;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public static class ArrayPool<TItem>
    {
        private static Dictionary<int, CustomObjectPool<TItem[]>> m_Pool = new Dictionary<int, CustomObjectPool<TItem[]>>();

        private static CustomObjectPool<TItem[]> GetPool(int length)
        {
            CustomObjectPool<TItem[]> pool;
            if (m_Pool.TryGetValue(length, out pool) == false)
            {
                pool = new CustomObjectPool<TItem[]>(() => new TItem[length], null, OnRelease);
                m_Pool.Add(length, pool);
            }

            return pool;
        }

        private static void OnRelease(TItem[] array)
        {
            for (int i = 0, end = array.Length; i < end; i++)
            {
                array[i] = default;
            }
        }

        public static TItem[] Get(int length)
        {
            return GetPool(length).Get();
        }

        public static void Release(TItem[] array)
        {
            GetPool(array.Length).Release(array);
        }

        public static void Clear()
        {
            m_Pool.Clear();
        }
    }

    public static class ArrayPoolRelease
    {
        public static void Release<T>(this T[] array)
        {
            ArrayPool<T>.Release(array);
        }
    }
}