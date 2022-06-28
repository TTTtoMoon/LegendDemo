using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 碰撞检测结果
    /// </summary>
    public struct CrashResult : IDisposable
    {
        public CrashResult(int count, RaycastHit[] hits)
        {
            m_Count     = count;
            m_Targets   = ArrayPool<IAbilityTarget>.Get(hits.Length);
            for (int i = 0; i < count; i++)
            {
                m_Targets[i] = hits[i].transform.parent.GetComponent<IAbilityTarget>();
            }
        }

        public CrashResult(int count, Collider[] colliders)
        {
            m_Count     = count;
            m_Targets   = ArrayPool<IAbilityTarget>.Get(colliders.Length);
            for (int i = 0; i < count; i++)
            {
                m_Targets[i] = colliders[i].transform.parent.GetComponent<IAbilityTarget>();
            }
        }

        private int              m_Count;
        private IAbilityTarget[] m_Targets;

        public int Count => m_Count;
        public IAbilityTarget this[int index] => m_Targets[index];

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= m_Count)
            {
                return;
            }

            m_Count--;
            if (m_Count < 0)
            {
                return;
            }

            for (int i = index; i < m_Count; i++)
            {
                m_Targets[i] = m_Targets[i + 1];
            }
            
            m_Targets[m_Count] = null;
        }

        public void Dispose()
        {
            ArrayPool<IAbilityTarget>.Release(m_Targets);
            m_Targets = null;
        }
    }
}