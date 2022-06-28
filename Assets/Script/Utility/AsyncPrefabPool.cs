using System.Collections.Generic;
using UnityEngine;

namespace RogueGods.Utility
{
    public class AsyncPrefabPool<T> where T : Component
    {
        private string     m_Name;
        private GameObject m_Root;
        private T          m_Prefab;
        private Stack<T>   m_Pool;

        public readonly NiceDelegate<T> OnCreate = new NiceDelegate<T>();
        public readonly NiceDelegate<T> OnPop    = new NiceDelegate<T>();
        public readonly NiceDelegate<T> OnPush   = new NiceDelegate<T>();

        public string    Name => m_Name;
        public Transform Root => m_Root.transform;

        public AsyncPrefabPool(GameObject prefab, GameObject root = null)
        {
            m_Prefab = prefab.GetComponent<T>();
            if (m_Prefab == null)
            {
                Debug.LogWarning($"{prefab}缺少组件{typeof(T).Name}");
                return;
            }

            m_Name = m_Prefab.name;
            m_Root = root == null ? new GameObject($"PrefabPool_{m_Name}") : root;
            Object.DontDestroyOnLoad(m_Root);
            m_Pool = new Stack<T>();
        }

        public T Pop(bool autoActive = true)
        {
            if (m_Pool == null)
            {
                return null;
            }

            T instance;
            if (m_Pool.Count > 0)
            {
                instance = m_Pool.Pop();
            }
            else
            {
                instance = Object.Instantiate(m_Prefab, Root, false);
                OnCreate.Invoke(instance);
            }

            instance.name = m_Name;
            if (autoActive)
            {
                instance.gameObject.SetActive(true);
            }

            OnPop.Invoke(instance);
            return instance;
        }

        public void Push(T instance)
        {
            OnPush.Invoke(instance);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(Root);
            m_Pool.Push(instance);
        }

        public void Destroy()
        {
            m_Name = null;
            m_Pool.Clear();
            Object.DestroyImmediate(m_Root);
            m_Root   = null;
            m_Prefab = null;
        }
    }
}