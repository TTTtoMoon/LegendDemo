using System;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public static class ObjectPool<TItem> where TItem : new()
    {
        private static Stack<TItem> m_Pool = new Stack<TItem>();

        public static TItem Get()
        {
            if (m_Pool.Count > 0)
            {
                TItem item = m_Pool.Pop();
                if (item != null)
                {
                    return item;
                }
            }

            return new TItem();
        }

        public static void Release(TItem item)
        {
            if (item == null) return;
            if (m_Pool.Contains(item))
            {
                Debugger.LogError("ObjectPool释放了重复的对象，这可能造成非常严重的错误，已自动终止Release。");
                return;
            }

            if (item is IPoolDisposable disposable) disposable.Dispose();
            m_Pool.Push(item);
        }

        public static void Dispose()
        {
            m_Pool.Clear();
        }
    }

    public class ObjectPoolInstance<TItem> : IDisposable where TItem : new()
    {
        private Stack<TItem> m_Pool = new Stack<TItem>();

        public TItem Get()
        {
            return ObjectPool<TItem>.Get();
        }

        public void Release(TItem item)
        {
            m_Pool.Push(item);
        }

        public void Dispose()
        {
            for (int i = m_Pool.Count - 1; i >= 0; i--)
            {
                ObjectPool<TItem>.Release(m_Pool.Pop());
            }

            m_Pool.Clear();
        }
    }
}