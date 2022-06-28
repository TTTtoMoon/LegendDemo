using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    // TODO
    [Serializable]
    [Description("当拥有者技能释放结束时(包括打断)")]
    public sealed class OnOwnerSkillEnd : FeatureTrigger
    {
        [Description("技能筛选")] 
        public Filter<Ability> Filter = new Filter<Ability>();


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
                
            }
        }
    }
}