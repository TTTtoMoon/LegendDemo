using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当子弹销毁时")]
    public class OnOrbDisable : FeatureTrigger
    {
        [Description("筛选")]
        public Filter<Ability> Filter = new Filter<Ability>();

        protected override void OnEnable()
        {
            GameManager.OrbSystem.OnDestroyOrb += OnDestroyOrb;
        }

        protected override void OnDisable()
        {
            GameManager.OrbSystem.OnDestroyOrb -= OnDestroyOrb;
        }

        protected override void OnUpdate()
        {
        }

        private void OnDestroyOrb(Orb orb)
        {
            if (ReferenceEquals(orb.Ability.Source, Ability.Owner) && Filter.Verify(orb.Ability))
            {
                InvokeEffect(orb);
            }
        }
    }
}