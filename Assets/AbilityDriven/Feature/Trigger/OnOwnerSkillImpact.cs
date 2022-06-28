using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当拥有者技能前摇结束时")]
    public sealed class OnOwnerSkillImpact : FeatureTrigger
    {
        [Description("技能筛选")] 
        public Filter<Ability> Filter = new Filter<Ability>();

        private bool m_Invoked = false;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnUpdate()
        {
            if (Ability.Owner is Actor actor)
            {
                if (actor.SkillDirector.CurrentStage == SkillPhase.Finishing ||
                    actor.SkillDirector.CurrentStage == SkillPhase.NoSkill)
                {
                    m_Invoked = false;
                    return;
                }

                if (m_Invoked)
                {
                    return;
                }

                if (actor.SkillDirector.CurrentStage == SkillPhase.Acting &&
                    Filter.Verify(actor.SkillDirector.CurrentSkill.Ability))
                {
                    m_Invoked = true;
                    InvokeEffect();
                }
            }
        }
    }
}