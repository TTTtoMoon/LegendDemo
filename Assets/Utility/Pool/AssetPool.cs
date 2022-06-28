using System.Collections.Generic;

namespace RogueGods.Utility
{
    public class AssetPool<TAsset> where TAsset : UnityEngine.Object
    {
        private TAsset m_Asset;
        private LinkedList<TAsset> m_Pool = new LinkedList<TAsset>();

        public readonly NiceDelegate<TAsset> GetListener = new NiceDelegate<TAsset>();
        public readonly NiceDelegate<TAsset> ReleaseListener = new NiceDelegate<TAsset>();
        public readonly NiceDelegate ClearListener = new NiceDelegate();

        public AssetPool(TAsset asset)
        {
            m_Asset = asset;
        }

        public TAsset Get()
        {
            LinkedListNode<TAsset> cacheNode = m_Pool.Last;
            TAsset asset;
            if (cacheNode == null)
            {
                asset = m_Asset == null ? null : UnityEngine.Object.Instantiate(m_Asset);
            }
            else
            {
                asset = cacheNode.Value;
                m_Pool.RemoveLast();
            }

            GetListener.Invoke(asset);
            OnGet(asset);
            return asset;
        }

        public void Release(TAsset asset)
        {
            if (asset == null) return;
            if (m_Pool.Contains(asset))
            {
                Debugger.LogError("ObjectPool释放了重复的对象，这可能造成非常严重的错误，已自动终止Release。");
                return;
            }

            m_Pool.AddLast(asset);
            ReleaseListener.Invoke(asset);
            OnRelease(asset);
        }

        public void Clear()
        {
            OnClear();
            m_Pool.Clear();
        }

        protected virtual void OnGet(TAsset asset)
        {
        }

        protected virtual void OnRelease(TAsset asset)
        {
        }

        protected virtual void OnClear()
        {
        }
    }
}