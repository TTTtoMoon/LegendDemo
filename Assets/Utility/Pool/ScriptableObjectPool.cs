using UnityEngine;

namespace RogueGods.Utility
{
    public static class ScriptableObjectPool<T> where T : ScriptableObject
    {
        private class Pool : AbstractPool<T>
        {
            protected override T NewItem()
            {
                return ScriptableObject.CreateInstance<T>();
            }

            protected override void OnClear(T item)
            {
                Object.DestroyImmediate(item);
            }
        }

        private static readonly Pool m_Pool = new Pool();

        public static T Get(T prefab)
        {
            T instance = m_Pool.Get();
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(prefab), instance);
            instance.name = prefab.name;
            return instance;
        }

        public static void Release(T item)
        {
            m_Pool.Release(item);
        }

        public static void Clear()
        {
            m_Pool.Clear();
        }
    }
}