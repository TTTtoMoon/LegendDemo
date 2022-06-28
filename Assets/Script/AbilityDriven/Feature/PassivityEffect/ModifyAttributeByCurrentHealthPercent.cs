using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("根据当前生命值百分比修改属性")]
    public class ModifyAttributeByCurrentHealthPercent : PassivityFeatureEffect
    {
        [Description("加成最大时的血量百分比")] [Range(0f, 1f)]
        public float MaxBound = 0f;

        [Description("加成为0时的血量百分比")] [Range(0f, 1f)]
        public float MinBound = 1f;

        [Description("每多少比率生命转换一次")] public AbilityVariable EveryHealthPercent = new AbilityVariable(0.01f);

        [Description("目标属性")] public AttributeType TargetAttribute;

        [Description("按比率修改属性?")] public bool ModifyAttributeByMultiple;

        [Description("每次获得的属性数值")] public AbilityVariable AttributeConversionRote = new AbilityVariable(0.01f);

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
                float modifier = 0f;
                ApplyModify();

                EveryHealthPercent.OnValueChanged      += OnChanged;
                AttributeConversionRote.OnValueChanged += OnChanged;
                actor.OnCurrentHealthChanged           += OnChanged;

                return () =>
                {
                    EveryHealthPercent.OnValueChanged      -= OnChanged;
                    AttributeConversionRote.OnValueChanged -= OnChanged;
                    actor.OnCurrentHealthChanged           -= OnChanged;
                };

                void OnChanged(float oldValue, float newValue)
                {
                    ApplyModify();
                }

                void ApplyModify()
                {
                    float newModifier = GetModify();
                    if (ModifyAttributeByMultiple)
                    {
                        actor.Attribute.ModifyMultiplyValue(TargetAttribute, newModifier - modifier);
                    }
                    else
                    {
                        actor.Attribute.ModifyAdditionValue(TargetAttribute, newModifier - modifier);
                    }

                    modifier = newModifier;
                }

                float GetModify()
                {
                    float healthPercent = actor.CurrentHealth / actor.Attribute[AttributeType.MaxHealth];
                    int   convertTimes;
                    if (MaxBound > MinBound)
                    {
                        healthPercent = Mathf.Clamp(healthPercent, MinBound, MaxBound);
                        convertTimes  = (int)((healthPercent - MinBound) / EveryHealthPercent.Value);
                    }
                    else
                    {
                        healthPercent = Mathf.Clamp(healthPercent, MaxBound, MinBound);
                        convertTimes  = (int)((MinBound - healthPercent) / EveryHealthPercent.Value);
                    }

                    return AttributeConversionRote.Value * convertTimes;
                }
            }

            return null;
        }
    }
}