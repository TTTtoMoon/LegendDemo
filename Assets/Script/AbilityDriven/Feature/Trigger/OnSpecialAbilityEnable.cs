using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当特定能力生效时")]
    public class OnSpecialAbilityEnable : FeatureTrigger
    {
        [Description("筛选")]
        public Filter<Ability> Filter = new Filter<Ability>();

        protected override void OnEnable()
        {
            AbilityDirector.OnEnable += OnEnableAbility;
        }

        protected override void OnDisable()
        {
            AbilityDirector.OnEnable -= OnEnableAbility;
        }

        protected override void OnUpdate()
        {
        }

        private void OnEnableAbility(Ability ability)
        {
            if (ability.Source == Ability.Owner && Filter.Verify(ability))
            {
                InvokeEffect(ability.Owner);
            }
        }
    }
}