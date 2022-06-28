using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.VFX
{
    public class VFXSystem : GameSystem
    {
        private GameObject m_Root;

        public static VFXInstance Empty { get; private set; }

        private Dictionary<GameObject, AsyncPrefabPool<VFXInstance>> m_PoolMap = new Dictionary<GameObject, AsyncPrefabPool<VFXInstance>>();

        private List<VFXInstance> m_PooledVFXs = new List<VFXInstance>(256);

        public override void Awake()
        {
            base.Awake();

            m_Root           = new GameObject("VFX_POOL");
            m_Root.hideFlags = HideFlags.DontSave;
            Object.DontDestroyOnLoad(m_Root);

            Empty = new GameObject("EmptyVFX").AddComponent<VFXInstance>();
            Empty.transform.SetParent(m_Root.transform);
            Empty.gameObject.SetActive(false);
            Empty.LifeTime = 0f;
            GameManager.Instance.RegisterUpdate(UpdateVFXState);
        }

        public VFXInstance CreateInstance(GameObject vfx, bool autoActive = true, bool autoDestroy = true)
        {
            if (vfx == null) return Empty;
            AsyncPrefabPool<VFXInstance> pool;
            if (m_PoolMap.TryGetValue(vfx, out pool) == false)
            {
                pool = NewPool();
                m_PoolMap.Add(vfx, pool);
            }

            VFXInstance instance = pool.Pop(autoActive);
            if (instance == null)
            {
                return Empty;
            }

            instance.AutoDestroy  = autoDestroy;
            instance.State        = VFXState.Playing;
            instance.ActiveAtTime = Time.time;
            return instance;

            AsyncPrefabPool<VFXInstance> NewPool()
            {
                AsyncPrefabPool<VFXInstance> newPool = new AsyncPrefabPool<VFXInstance>(vfx, m_Root);
                newPool.OnCreate.AddListener(Initialize);
                newPool.OnPush.AddListener(Dispose);
                return newPool;

                void Initialize(VFXInstance x)
                {
                    x.Pool = newPool;
                    m_PooledVFXs.Add(x);
                }

                void Dispose(VFXInstance x)
                {
                    x.State = VFXState.InActive;
                    x.transform.StopFollow();
                }
            }
        }

        public VFXInstance CreateInstance(GameObject vfx, Vector3 position, Quaternion rotation, float scale = 1f, bool autoDestroy = true)
        {
            VFXInstance instance = CreateInstance(vfx, false, autoDestroy);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.localScale = scale * Vector3.one;
            instance.gameObject.SetActive(true);
            return instance;
        }

        public VFXInstance CreateInstance(GameObject vfx, Transform followTarget, TransformFollower.Params followParams, bool autoDestroy = true)
        {
            VFXInstance instance = CreateInstance(vfx, false, autoDestroy);
            instance.transform.Follow(followTarget, followParams);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void DestroyInstance(VFXInstance instance)
        {
            if (instance == null || instance == Empty)
            {
                return;
            }

            if (instance.DestroyTime > 0f)
            {
                Destroy(instance);
            }
            else
            {
                instance.Pool.Push(instance);
            }
        }

        private void Destroy(VFXInstance instance)
        {
            instance.AutoDestroy = true;
            instance.transform.StopFollow();
            if (instance.transform.parent != instance.Pool.Root)
            {
                instance.transform.SetParent(instance.Pool.Root, true);
            }

            instance.State = VFXState.Destroying;
            instance.PlayDestroyAnimation();
        }

        private void UpdateVFXState()
        {
            float now = Time.time;
            for (int i = m_PooledVFXs.Count - 1; i >= 0; i--)
            {
                VFXInstance instance = m_PooledVFXs[i];

                switch (instance.State)
                {
                    case VFXState.Playing:
                        if (instance.AutoDestroy == false || instance.LifeTime < 0f)
                        {
                            continue;
                        }

                        if (now >= instance.ActiveAtTime + instance.LifeTime)
                        {
                            Destroy(instance);
                        }
                        else
                        {
                            PlayAudios(instance.ActiveAtTime, instance, instance.ActiveAudios);
                        }

                        break;

                    case VFXState.Destroying:
                        float lifeTime = instance.LifeTime < 0f ? 0f : instance.LifeTime;
                        if (now >= instance.ActiveAtTime + lifeTime + instance.DestroyTime)
                        {
                            instance.Pool.Push(instance);
                        }
                        else
                        {
                            PlayAudios(instance.ActiveAtTime + lifeTime, instance, instance.DestroyAudios);
                        }

                        break;
                }
            }

            void PlayAudios(float startTime, VFXInstance instance, VFXInstance.AudioInfo[] audios)
            {
                for (int i = 0, length = audios.Length; i < length; i++)
                {
                    ref VFXInstance.AudioInfo audioInfo = ref audios[i];
                    if (now < startTime + audioInfo.Delay)
                    {
                        continue;
                    }

                    AudioSource.PlayClipAtPoint(audioInfo.Audio, instance.transform.position, audioInfo.Volume);
                }
            }
        }
    }
}