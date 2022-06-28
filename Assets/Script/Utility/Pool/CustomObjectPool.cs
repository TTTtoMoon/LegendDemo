using System;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public class CustomObjectPool<TItem>
    {
        private LinkedList<TItem> m_Pool = new LinkedList<TItem>();
        protected Func<TItem> m_Create;
        protected Action<TItem> m_OnGet, m_OnRelease;

        public CustomObjectPool(Func<TItem> create, Action<TItem> onGet = null, Action<TItem> onRelease = null)
        {
            m_Create = create;
            m_OnGet = onGet;
            m_OnRelease = onRelease;
        }

        public TItem Get()
        {
            LinkedListNode<TItem> cacheNode = m_Pool.Last;
            TItem item;
            if (cacheNode == null)
            {
                item = m_Create.Invoke();
            }
            else
            {
                item = cacheNode.Value;
                m_Pool.RemoveLast();
            }

            m_OnGet?.Invoke(item);
            return item;
        }

        public void Release(TItem item)
        {
            if (item == null) return;
            if (m_Pool.Contains(item))
            {
                Debugger.LogError("ObjectPool释放了重复的对象，这可能造成非常严重的错误，已自动终止Release。");
                return;
            }

            if (item is IPoolDisposable disposable)
            {
                disposable.Dispose();
            }

            m_Pool.AddLast(item);
            m_OnRelease?.Invoke(item);
        }

        public void Clear()
        {
            m_Pool.Clear();
        }
    }
}