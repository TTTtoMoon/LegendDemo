using System;
using System.Collections.Generic;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    public class BuffDirector
    {
        public BuffDirector(AbilityDirector abilityDirector)
        {
            m_AbilityDirector         =  abilityDirector;
            AbilityDirector.OnDisable += OnAbilityDisable;
        }

        private AbilityDirector      m_AbilityDirector;
        private List<BuffDescriptor> m_Buffs = new List<BuffDescriptor>();

        public IReadOnlyList<BuffDescriptor> Buffs => m_Buffs;

        public event Action<BuffDescriptor> OnAcquire, OnDeprive;

        /// <summary>
        /// 添加buff
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="giver"></param>
        public void Acquire(BuffDescriptor buff, Ability giver = null)
        {
            Ability ability = Ability.Allocate(buff);
            if (TryFind(buff.AbilityID, out BuffDescriptor existBuff))
            {
                switch (existBuff.OverlayType)
                {
                    case BuffOverlayType.Restart:
                        Restart();
                        break;
                    case BuffOverlayType.OverlayDuration:
                        OverlayDuration();
                        break;
                    case BuffOverlayType.Independent:
                        Independent();
                        break;
                    default:
                        Ability.Recycle(ability);
                        break;
                }

                return;
            }

            EnableBuff();

            void Restart()
            {
                m_AbilityDirector.Disable(existBuff.Ability);
                m_Buffs.Remove(existBuff);
                EnableBuff();
            }

            void OverlayDuration()
            {
                existBuff.Ability.Duration += ability.Duration;
                Ability.Recycle(ability);
            }

            void Independent()
            {
                if (ability.TryGetComponent(out BuffIndependentOverlay independentOverlay) &&
                    GetLayer(buff.AbilityID) < independentOverlay.MaxOverlayLayer)
                {
                    EnableBuff();
                }
            }

            void EnableBuff()
            {
                buff.Ability = ability;
                m_AbilityDirector.Enable(ability, null, giver);
                m_Buffs.Add(buff);
                OnAcquire?.Invoke(buff);
            }
        }

        public void Deprive(BuffDescriptor buff)
        {
            if (buff != null && buff.Ability != null)
            {
                m_AbilityDirector.Disable(buff.Ability);
            }
        }

        /// <summary>
        /// 尝试查找buff
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buff"></param>
        /// <returns></returns>
        public bool TryFind(int id, out BuffDescriptor buff)
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                if (m_Buffs[i].AbilityID != id) continue;
                buff = m_Buffs[i];
                return true;
            }

            buff = null;
            return false;
        }

        /// <summary>
        /// 获取buff层数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetLayer(int id)
        {
            int layer = 0;
            for (int i = 0, length = m_Buffs.Count; i < length; i++)
            {
                if (m_Buffs[i].AbilityID == id)
                {
                    switch (m_Buffs[i].OverlayType)
                    {
                        case BuffOverlayType.DontOverlay:
                            layer = 1;
                            break;
                        case BuffOverlayType.Restart:
                            layer = 1;
                            break;
                        case BuffOverlayType.OverlayDuration:
                            layer = 1;
                            break;
                        case BuffOverlayType.Independent:
                            layer++;
                            break;
                    }
                }
            }

            return layer;
        }

        /// <summary>
        /// 清空buff
        /// </summary>
        public void Clear()
        {
            while (m_Buffs.Count > 0)
            {
                m_AbilityDirector.Disable(m_Buffs[m_Buffs.Count - 1].Ability);
            }
        }

        private void OnAbilityDisable(Ability ability)
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                BuffDescriptor buff = m_Buffs[i];
                if (buff.Ability == ability)
                {
                    Ability.Recycle(ability);
                    buff.Ability = null;
                    m_Buffs.RemoveAt(i);
                    OnDeprive?.Invoke(buff);
                    break;
                }
            }
        }
    }
}