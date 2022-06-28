using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 属性值
    /// </summary>
    public sealed class AttributeValue
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        private AttributeType m_Type;
        
        /// <summary>
        /// 基础值
        /// </summary>
        private float m_BaseValue;

        /// <summary>
        /// 加成值
        /// </summary>
        private float m_AdditionValue;
        
        /// <summary>
        /// 倍率值
        /// </summary>
        private float m_MultiplyValue;

        /// <summary>
        /// 数值变化事件
        /// </summary>
        public event AttributeChangeDelegate OnValueChanged;

        public AttributeValue(AttributeType type)
        {
            m_Type = type;
        }

        /// <summary>
        /// 属性类型
        /// </summary>
        public AttributeType Type => m_Type;

        /// <summary>
        /// 基础值
        /// </summary>
        public float BaseValue => m_Type.Fix(m_BaseValue);
        
        /// <summary>
        /// 加成值(不包括倍率)
        /// </summary>
        public float AdditionValue => m_Type.Fix(m_AdditionValue);

        /// <summary>
        /// 加成值(包括倍率)
        /// </summary>
        public float AdditionWithMultipleValue => m_Type.Fix(m_AdditionValue + (m_BaseValue + m_AdditionValue) * m_MultiplyValue);

        /// <summary>
        /// 倍率值
        /// </summary>
        public float MultiplyValue => m_MultiplyValue;
        
        /// <summary>
        /// 最终值
        /// </summary>
        public float Value => m_Type.CalculateValue(m_BaseValue,  m_AdditionValue, m_MultiplyValue);
        
        /// <summary>
        /// 初始化值
        /// </summary>
        public void SetBaseValueWithoutNotify(float value)
        {
            m_BaseValue = m_Type.Fix(value);
        }

        /// <summary>
        /// 修改基础值 正加负减
        /// </summary>
        /// <param name="modifier">基础值修改量</param>
        public void ModifyBaseValue(float modifier)
        {
            ApplyModify(ref m_BaseValue, modifier);
        }

        /// <summary>
        /// 修改加成值 正加负减
        /// </summary>
        /// <param name="modifier">加成值修改量</param>
        public void ModifyAdditionValue(float modifier)
        {
            ApplyModify(ref m_AdditionValue, modifier);
        }

        /// <summary>
        /// 修改倍率值 正加负减
        /// </summary>
        /// <param name="modifier">倍率值修改量</param>
        public void ModifyMultiplyValue(float modifier)
        {
            ApplyModify(ref m_MultiplyValue, modifier);
        }

        /// <summary>
        /// 重置数值为0
        /// </summary>
        public void Reset()
        {
            m_BaseValue     = 0f;
            m_AdditionValue = 0f;
            m_MultiplyValue = 0f;
        }

        private void ApplyModify(ref float value, float modifier)
        {
            float oldValue = Value;
            value += modifier;
            float newValue = Value;
            if (Mathf.Approximately(oldValue, newValue))
            {
                return;
            }
                
            OnValueChanged?.Invoke(oldValue, newValue);
        }

        public static implicit operator float(AttributeValue value)
        {
            return value.Value;
        }
        
        public static implicit operator int(AttributeValue value)
        {
            return Mathf.RoundToInt(value.Value);
        }
    }
}