using System.Collections.Generic;
using RogueGods.Utility;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 属性
    /// </summary>
    public sealed partial class AttributeProperty
    {
        private static IReadOnlyList<AttributeType> AttributeTypes => EnumCache<AttributeType>.Values;
        private static int                          AttributeCount => EnumCache<AttributeType>.ValueCount;

        public AttributeProperty()
        {
            // 排除第一个None
            m_Values = new AttributeValue[AttributeCount - 1];
            for (int i = 0; i < AttributeCount - 1; i++)
            {
                m_Values[i] = new AttributeValue(AttributeTypes[i + 1]);
            }
        }

        public AttributeProperty(AttributeProperty other) : this()
        {
            Overwrite(other);
        }

        private AttributeValue[] m_Values;

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="attribute"></param>
        private AttributeValue GetAttribute(AttributeType attribute)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                if (m_Values[i].Type == attribute)
                {
                    return m_Values[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 覆写属性
        /// </summary>
        /// <param name="other"></param>
        public void Overwrite(AttributeProperty other)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                m_Values[i].SetBaseValueWithoutNotify(other.m_Values[i].Value);
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="attribute"></param>
        public float this[AttributeType attribute] => GetValue(attribute);

        /// <summary>
        /// 最终值
        /// </summary>
        public float GetValue(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return 0f;
            return GetAttribute(attribute).Value;
        }

        /// <summary>
        /// 基础值
        /// </summary>
        public float GetBaseValue(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return 0f;
            return GetAttribute(attribute).BaseValue;
        }
        
        /// <summary>
        /// 加成值(不包括倍率)
        /// </summary>
        public float GetAdditionValue(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return 0f;
            return GetAttribute(attribute).AdditionValue;
        }

        /// <summary>
        /// 加成值(包括倍率)
        /// </summary>
        public float GetAdditionWithMultipleValue(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return 0f;
            return GetAttribute(attribute).AdditionWithMultipleValue;
        }

        /// <summary>
        /// 倍率值
        /// </summary>
        public float GetMultiplyValue(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return 0f;
            return GetAttribute(attribute).MultiplyValue;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        public void SetBaseValueWithoutNotify(AttributeType attribute, float value)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).SetBaseValueWithoutNotify(value);
        }

        /// <summary>
        /// 修改基础值 正加负减
        /// </summary>
        public void ModifyBaseValue(AttributeType attribute, float modifier)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).ModifyBaseValue(modifier);
        }

        /// <summary>
        /// 修改加成值 正加负减
        /// </summary>
        public void ModifyAdditionValue(AttributeType attribute, float modifier)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).ModifyAdditionValue(modifier);
        }

        /// <summary>
        /// 修改倍率值 正加负减
        /// </summary>
        public void ModifyMultiplyValue(AttributeType attribute, float modifier)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).ModifyMultiplyValue(modifier);
        }

        /// <summary>
        /// 修改属性值
        /// </summary>
        public void Modify(AttributeType attribute, bool isMultiplier, float modifier)
        {
            if (isMultiplier)
            {
                ModifyMultiplyValue(attribute, modifier);
            }
            else
            {
                ModifyAdditionValue(attribute, modifier);
            }
        }

        public void AddOnValueChanged(AttributeType attribute, AttributeChangeDelegate onValueChanged)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).OnValueChanged += onValueChanged;
        }

        public void RemoveOnValueChanged(AttributeType attribute, AttributeChangeDelegate onValueChanged)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).OnValueChanged -= onValueChanged;
        }

        /// <summary>
        /// 重置某属性值
        /// </summary>
        public void Reset(AttributeType attribute)
        {
            if (attribute == AttributeType.None) return;
            GetAttribute(attribute).Reset();
        }

        /// <summary>
        /// 重置全部属性值
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                m_Values[i].Reset();
            }
        }
    }
}