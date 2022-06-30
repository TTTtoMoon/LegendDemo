using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    public sealed class AbilitySystem : GameSystem, IAbilityFactory
    {
        private Dictionary<int, Pool> m_PoolMap = new Dictionary<int, Pool>();

        Ability IAbilityFactory.Allocate(int configurationID)
        {
            if (m_PoolMap.TryGetValue(configurationID, out Pool pool) == false)
            {
                pool = new Pool(configurationID);
                m_PoolMap.Add(configurationID, pool);
            }

            return pool.Get();
        }

        void IAbilityFactory.Recycle(Ability ability)
        {
            if (m_PoolMap.TryGetValue(ability.ConfigurationID, out Pool pool))
            {
                pool.Release(ability);
            }
        }

        public override void Awake()
        {
            base.Awake();
            this.SetUp();
        }

        public override void Update()
        {
            base.Update();
            Ability.Update();
        }

        /// <summary>
        /// 释放缓存
        /// </summary>
        public void DisposeCache()
        {
            foreach (KeyValuePair<int, Pool> kv in m_PoolMap)
            {
                kv.Value.Dispose();
            }

            m_PoolMap.Clear();
        }

        private class Pool : AbstractPool<Ability>, IDisposable
        {
            private readonly string m_Json;

            public Pool(int abilityID)
            {
                string path = Ability.GetAbilityName(abilityID);
                Ability ability = Resources.Load<Ability>($"Ability/{path}");
                m_Json = ability != null ? AbilitySerializer.ToJson(ability) : string.Empty;
                Resources.UnloadAsset(ability);
            }

            protected override Ability NewItem()
            {
                return AbilitySerializer.DeSerialize(m_Json);
            }

            public void Dispose()
            {
            }
        }
    }
}