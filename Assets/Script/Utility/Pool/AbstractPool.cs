using System.Collections.Generic;
using UnityEngine;

namespace RogueGods.Utility
{
    public abstract class AbstractPool<TItem> where TItem : class
    {
        private Stack<TItem> m_Pool = new Stack<TItem>();

        protected abstract TItem NewItem();

        protected virtual void OnGet(TItem item)
        {
        }

        protected virtual void OnRelease(TItem item)
        {
        }

        protected virtual void OnClear(TItem item)
        {
        }

        public TItem Get()
        {
            TItem item = null;
            while (m_Pool.Count > 0 && item == null)
            {
                item = m_Pool.Pop();
            }

            if (item == null)
            {
                item = NewItem();
            }

            OnGet(item);
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

            OnRelease(item);
            if (item is IPoolDisposable disposable) disposable.Dispose();
            m_Pool.Push(item);
        }

        public void Clear()
        {
            while (m_Pool.Count > 0)
            {
                OnClear(m_Pool.Pop());
            }
        }
    }

    public class ComponentPool<TComponent> : AbstractPool<TComponent> where TComponent : Component
    {
        private static readonly string m_ItemName = "PoolItem->" + typeof(TComponent).Name;

        private Transform m_Root;

        public ComponentPool(string rootName)
        {
            GameObject root = new GameObject(rootName);
            m_Root = root.transform;
            Object.DontDestroyOnLoad(root);
        }

        protected override TComponent NewItem()
        {
            GameObject item = new GameObject(m_ItemName);
            return item.AddComponent<TComponent>();
        }
    }
}