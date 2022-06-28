using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("修改属性")]
    public sealed class ModifyAttribute : PassivityFeatureEffect
    {
        [Description("目标属性")] 
        public AttributeType TargetAttribute;

        [Description("修改值(正加负减)")] 
        public AbilityVariable AttributeModifier = new AbilityVariable(0f);

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override EffectRevocation Invoke(IAbilityTarget target)
        {
            if (target is Actor actor)
            {
                actor.Attribute.ModifyAdditionValue(TargetAttribute, AttributeModifier.Value);
                AttributeModifier.OnValueChanged += OnAttributeModifierChanged;

                return () =>
                {
                    AttributeModifier.OnValueChanged -= OnAttributeModifierChanged;
                    actor.Attribute.ModifyAdditionValue(TargetAttribute, -AttributeModifier.Value);
                };

                void OnAttributeModifierChanged(float oldValue, float newValue)
                {
                    actor.Attribute.ModifyAdditionValue(TargetAttribute, newValue - oldValue);
                }
            }

            return null;
        }
    }
}