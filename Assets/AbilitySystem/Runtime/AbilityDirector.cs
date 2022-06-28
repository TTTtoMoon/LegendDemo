using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 能力驱动
    /// </summary>
    public sealed class AbilityDirector
    {
        private static int s_InstanceCounter = 0;

        public AbilityDirector(IAbilityOwner owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        private List<Ability> m_Abilities = new List<Ability>();

        /// <summary>
        /// 已生效能力集合
        /// </summary>
        public IReadOnlyList<Ability> Abilities => m_Abilities;

        /// <summary>
        /// 持有者
        /// </summary>
        public readonly IAbilityOwner Owner;

        /// <summary>
        /// 当获得能力时(能力生效前)
        /// </summary>
        public static event AbilityDelegate OnEnable;

        /// <summary>
        /// 当失去能力时(能力失效后)
        /// </summary>
        public static event AbilityDelegate OnDisable;

        /// <summary>
        /// 激活能力效果
        /// </summary>
        /// <param name="ability">能力实例</param>
        /// <param name="target">目标</param>
        /// <param name="giver">给与者</param>
        /// <param name="timeScale">时间缩放</param>
        public void Enable(Ability ability, IAbilityTarget target = null, Ability giver = null, float timeScale = 1f)
        {
            ability.m_InstanceID   = s_InstanceCounter++;
            ability.m_Director     = this;
            ability.m_Giver        = giver;
            ability.m_EnableAtTime = Time.time;
            ability.m_TimeScale    = timeScale > 0 ? timeScale : 1f;
            ability.m_Target       = target;
            ability.Reference();
            OnEnable?.Invoke(ability);

            for (int i = 0; i < ability.m_Components.Length; i++)
            {
                ability.m_Components[i].Ability = ability;
                ability.m_Components[i].OnEnable();
            }

            for (int i = 0; i < ability.m_Features.Length; i++)
            {
                ability.m_Features[i].OnEnable(ability);
            }

            Ability.s_ActiveAbilities.Add(ability);
            m_Abilities.Add(ability);
            if (Mathf.Approximately(ability.Duration, 0f))
            {
                Disable(ability);
            }
        }

        /// <summary>
        /// 终止能力效果
        /// 仅禁用能力效果，数据并不一定会立即清空，直到该能力没有任何引用
        /// </summary>
        /// <param name="ability">能力实例</param>
        public void Disable(Ability ability)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability), "Ability can't be null.");
            }

            if (m_Abilities.Remove(ability) == false)
            {
                return;
            }

            Ability.s_ActiveAbilities.Remove(ability);
            for (int i = 0; i < ability.m_Features.Length; i++)
            {
                ability.m_Features[i].OnDisable();
            }

            for (int i = 0; i < ability.m_Components.Length; i++)
            {
                ability.m_Components[i].OnDisable();
            }

            OnDisable?.Invoke(ability);
            ability.UnReference();
        }
    }
}