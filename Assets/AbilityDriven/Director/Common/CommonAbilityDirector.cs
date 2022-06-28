using System.Collections.Generic;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 通用技能总控，用于可破坏物、环境怪物等
    /// </summary>
    public class CommonAbilityDirector
    {
        public CommonAbilityDirector(AbilityDirector abilityDirector)
        {
            m_AbilityDirector         =  abilityDirector;
            AbilityDirector.OnDisable += OnAbilityDisable;
        }

        private AbilityDirector m_AbilityDirector;

        /// <summary>
        /// 开始指定ID的技能
        /// </summary>
        public Ability Begin(CommonAbilityDescriptor descriptor, IAbilityTarget searchedTarget, Ability giver = null)
        {
            Ability ability = Ability.Allocate(descriptor);
            m_AbilityDirector.Enable(ability, searchedTarget, giver);
            return ability;
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        /// <param name="ability"></param>
        public void Interrupt(Ability ability)
        {
            m_AbilityDirector.Disable(ability);
        }

        /// <summary>
        /// 打断所有正在执行的技能
        /// </summary>
        public void InterruptAll()
        {
            IReadOnlyList<Ability> abilities = m_AbilityDirector.Abilities;
            while (abilities.Count > 0)
            {
                m_AbilityDirector.Disable(abilities[abilities.Count - 1]);
            }
        }

        private void OnAbilityDisable(Ability ability)
        {
            if (ability.Director == m_AbilityDirector)
            {
                Ability.Recycle(ability);
            }
        }
    }
}