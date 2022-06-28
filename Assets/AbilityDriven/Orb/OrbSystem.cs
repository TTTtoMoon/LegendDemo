using System;
using System.Collections.Generic;
using RogueGods.Utility;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RogueGods.Gameplay.AbilityDriven
{
    public interface IOrb : IPoolDisposable
    {
        void Initialize(OrbOrder order);
        void Enable();
        void Disable();
    }

    public sealed class OrbSystem : GameSystem
    {
        private Transform             m_Root;
        private CustomObjectPool<Orb> m_Pool;

        private List<ToDestroy> m_ToDestroyOrbs = new List<ToDestroy>(32);

        public event Action<Orb> OnCreateOrb;
        public event Action<Orb> OnDestroyOrb;

        public override void Awake()
        {
            m_Root = new GameObject("OrbRoot").transform;
            Object.DontDestroyOnLoad(m_Root.gameObject);
            m_Pool = new CustomObjectPool<Orb>(NewOrb);

            Orb NewOrb()
            {
                GameObject go = new GameObject();
                go.transform.SetParent(m_Root);
                return go.AddComponent<Orb>();
            }
        }

        public Orb Create(OrbOrder order)
        {
            Orb instance = m_Pool.Get();
            ((IOrb)instance).Initialize(order);
            ((IOrb)instance).Enable();
            OnCreateOrb?.Invoke(instance);
            return instance;
        }

        public void Destroy(Orb orb, OrbDestroyOption option = OrbDestroyOption.All)
        {
            if (orb == null || orb.Enable == false) return;
            for (int i = 0; i < m_ToDestroyOrbs.Count; i++)
            {
                if (m_ToDestroyOrbs[i].Orb == orb)
                {
                    return;
                }
            }

            m_ToDestroyOrbs.Add(new ToDestroy()
            {
                Frame = Time.frameCount + 1,
                Orb   = orb,
            });

            if ((option & OrbDestroyOption.PlayDestroyVFX) != 0 && orb.Ability.TryGetComponent(out OrbInfo orbInfo))
            {
                GameManager.VFXSystem.CreateInstance(orbInfo.DestroyVFX, orb.Position + orb.Rotation * orbInfo.DestroyVFXOffset, orb.Rotation);
            }

            if ((option & OrbDestroyOption.TriggerDestroyEvent) != 0)
            {
                OnDestroyOrb?.Invoke(orb);
            }

            ((IOrb)orb).Disable();
        }
        
        public override void LateUpdate()
        {
            int currentFrame = Time.frameCount;
            for (int i = m_ToDestroyOrbs.Count - 1; i >= 0; i--)
            {
                ToDestroy toDestroy = m_ToDestroyOrbs[i];
                if (toDestroy.Frame > currentFrame || toDestroy.Orb.Ability.ReferenceCount > 0)
                {
                    continue;
                }

                m_Pool.Release(toDestroy.Orb);
                m_ToDestroyOrbs.RemoveAt(i);
            }
        }

        private struct ToDestroy
        {
            public int Frame;
            public Orb Orb;
        }
    }
}