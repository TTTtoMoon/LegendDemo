using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当特定能力失效时")]
    public class OnSpecialAbilityDisable : FeatureTrigger
    {
        [Description("筛选")]
        public Filter<Ability> Filter = new Filter<Ability>();

        protected override void OnEnable()
        {
            AbilityDirector.OnDisable += OnDisableAbility;
        }

        protected override void OnDisable()
        {
            AbilityDirector.OnDisable -= OnDisableAbility;
        }

        protected override void OnUpdate()
        {
        }

        private void OnDisableAbility(Ability ability)
        {
            if (ability.Source == Ability.Owner && Filter.Verify(ability))
            {
                InvokeEffect(ability.Owner);
            }
        }
    }
}