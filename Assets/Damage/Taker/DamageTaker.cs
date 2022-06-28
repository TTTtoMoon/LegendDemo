using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public class DamageTaker : MonoBehaviour
    {
        private static readonly List<DamageTaker>          m_Takers = new List<DamageTaker>(64);
        public static readonly  IReadOnlyList<DamageTaker> Takers   = m_Takers;

        private IDamageTaker m_Taker;
        public  IDamageTaker Taker => m_Taker ??= GetComponent<IDamageTaker>();

        private void OnEnable()
        {
            m_Takers.Add(this);
        }

        private void Start()
        {
            if (Taker == null)
            {
                Debugger.LogError($"{name}缺失IDamageTaker组件");
                DestroyImmediate(this);
            }
        }

        private void OnDisable()
        {
            m_Takers.Remove(this);
        }

        private void OnDestroy()
        {
            m_Takers.Remove(this);
        }
    }
}