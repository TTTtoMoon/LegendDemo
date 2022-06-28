using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("修改属性（基于属性）")]
    public sealed class ModifyAttributeByAttribute : PassivityFeatureEffect
    {
        [Description("目标属性")] 
        public AttributeType TargetAttribute;
        
        [Description("来源属性")] 
        public AttributeType BasedAttribute;
        
        [Description("来源属性转换率(正加负减)")] 
        public AbilityVariable AttributeConversionRote = new AbilityVariable(1f);

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
                if (TargetAttribute == BasedAttribute)
                {
                    actor.Attribute.ModifyMultiplyValue(TargetAttribute, AttributeConversionRote.Value);
                    AttributeConversionRote.OnValueChanged += OnVariableChanged;
                    return () =>
                    {
                        AttributeConversionRote.OnValueChanged -= OnVariableChanged;
                        actor.Attribute.ModifyMultiplyValue(TargetAttribute, -AttributeConversionRote.Value);
                    };

                    void OnVariableChanged(float oldValue, float newValue)
                    {
                        actor.Attribute.ModifyMultiplyValue(TargetAttribute, newValue - oldValue);
                    }
                }
                else
                {
                    actor.Attribute.ModifyAdditionValue(TargetAttribute, actor.Attribute[BasedAttribute] * AttributeConversionRote.Value);
                    actor.Attribute.AddOnValueChanged(BasedAttribute, OnAttributeChanged);
                    AttributeConversionRote.OnValueChanged += OnVariableChanged;
                    return () =>
                    {
                        AttributeConversionRote.OnValueChanged -= OnVariableChanged;
                        actor.Attribute.RemoveOnValueChanged(BasedAttribute, OnAttributeChanged);
                        actor.Attribute.ModifyMultiplyValue(TargetAttribute, -AttributeConversionRote.Value);
                    };

                    void OnAttributeChanged(float oldValue, float newValue)
                    {
                        actor.Attribute.ModifyAdditionValue(TargetAttribute, (newValue - oldValue) * AttributeConversionRote.Value);
                    }

                    void OnVariableChanged(float oldValue, float newValue)
                    {
                        actor.Attribute.ModifyAdditionValue(TargetAttribute, actor.Attribute[BasedAttribute] * (newValue - oldValue));
                    }
                }
            }

            return null;
        }
    }
}