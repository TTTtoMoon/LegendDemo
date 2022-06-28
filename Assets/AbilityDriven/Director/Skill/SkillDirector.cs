using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 角色技能总控
    /// </summary>
    public sealed class SkillDirector
    {
        public SkillDirector(Actor actor, AbilityDirector abilityDirector)
        {
            m_Actor                   =  actor;
            m_AbilityDirector         =  abilityDirector;
            AbilityDirector.OnDisable += OnDisableAbility;
        }

        private Actor                 m_Actor;
        private AbilityDirector       m_AbilityDirector;
        private List<SkillDescriptor> m_OwnedSkills = new List<SkillDescriptor>();

        public IReadOnlyList<SkillDescriptor> Skills => m_OwnedSkills;

        /// <summary>
        /// 技能生效事件
        /// </summary>
        public readonly NiceDelegate<SkillDescriptor> OnAcquire = new NiceDelegate<SkillDescriptor>();

        /// <summary>
        /// 技能生效事件
        /// </summary>
        public readonly NiceDelegate<SkillDescriptor> OnDeprive = new NiceDelegate<SkillDescriptor>();

        /// <summary>
        /// 技能生效事件
        /// </summary>
        public readonly NiceDelegate<SkillDescriptor> OnBegin = new NiceDelegate<SkillDescriptor>();

        /// <summary>
        /// 技能结束事件
        /// </summary>
        public readonly NiceDelegate<SkillDescriptor> OnEnd = new NiceDelegate<SkillDescriptor>();

        /// <summary>
        /// 当前技能
        /// </summary>
        public SkillDescriptor CurrentSkill { get; private set; }

        /// <summary>
        /// 当前技能阶段
        /// </summary>
        public SkillPhase CurrentStage => CurrentSkill?.CurrentPhase ?? SkillPhase.NoSkill;

        /// <summary>
        /// 是否正在释放技能
        /// </summary>
        public bool IsExecutingSkill => CurrentStage != SkillPhase.NoSkill;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill"></param>
        public SkillDescriptor Acquire(SkillDescriptor skill)
        {
            m_OwnedSkills.Add(skill);
            OnAcquire.Invoke(skill);
            return skill;
        }

        public void Deprive(SkillDescriptor skill)
        {
            if (m_OwnedSkills.Remove(skill))
            {
                if (skill.Ability != null && skill.Ability.IsActive)
                {
                    m_AbilityDirector.Disable(skill.Ability);
                }

                OnDeprive.Invoke(skill);
                skill.Ability = null;
            }
        }

        /// <summary>
        /// 施放技能
        /// </summary>
        /// <param name="skill">技能</param>
        /// <param name="searchedTarget">索敌目标</param>
        /// <returns></returns>
        public bool Begin(SkillDescriptor skill, IAbilityTarget searchedTarget = default /*todo 索敌目标*/)
        {
            if (m_Actor.Tag.HasTag(ActorTag.SilenceAbility) || skill == null)
            {
                return false;
            }

            Interrupt();
            CurrentSkill = skill;
            CurrentSkill.Ability = Ability.Allocate(CurrentSkill);
            float speed = 1f + m_Actor.Attribute[AttributeType.AttackSpeedModifier];
            m_AbilityDirector.Enable(skill.Ability, searchedTarget, null, speed);
            if (skill.Ability != null && skill.Ability.TryGetComponent(out SkillDescription skillDescriptor))
            {
                m_Actor.Animator.Play(skillDescriptor.State);
                m_Actor.Animator.SetSpeed(speed);
                if(skillDescriptor.EnableRootMotion) m_Actor.EnableRootMotion();
            }

            OnBegin.Invoke(skill);
            return true;
        }

        /// <summary>
        /// 施放当前蓄力技能
        /// </summary>
        public void Cast()
        {
            // TODO 蓄力技能
        }

        /// <summary>
        /// 强制打断当前技能
        /// </summary>
        public void Interrupt()
        {
            if (CurrentSkill                  == null ||
                CurrentSkill.Ability          == null ||
                CurrentSkill.Ability.IsActive == false)
            {
                return;
            }
            
            m_AbilityDirector.Disable(CurrentSkill.Ability);
            CurrentSkill = null;
        }

        /// <summary>
        /// 清空所有技能
        /// </summary>
        public void Clear()
        {
            for (int i = m_OwnedSkills.Count - 1; i >= 0; i--)
            {
                Deprive(m_OwnedSkills[i]);
            }

            if (CurrentSkill != null && CurrentSkill.Ability != null)
            {
                m_AbilityDirector.Disable(CurrentSkill.Ability);
            }
        }

        private void OnDisableAbility(Ability ability)
        {
            if (CurrentSkill == null || CurrentSkill.Ability != ability)
            {
                return;
            }

            if (ability.TryGetComponent(out SkillDescription skillDescriptor) &&
                skillDescriptor.EnableRootMotion)
            {
                m_Actor.DisableRootMotion();
            }
            
            m_Actor.Animator.ResetSpeed();
            OnEnd.Invoke(CurrentSkill);
            CurrentSkill.Ability = null;
            CurrentSkill         = null;
            Ability.Recycle(ability);
        }
    }
}