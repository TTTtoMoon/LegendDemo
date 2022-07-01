using UnityEngine;

namespace RogueGods.Gameplay
{
    public static class AttributeUtility
    {
        /// <summary>
        /// 数值修正
        /// </summary>
        public static float Fix(this AttributeType attribute, float value)
        {
            switch (attribute)
            {
                case AttributeType.MaxHealth:
                case AttributeType.Energy:
                case AttributeType.Attack:
                    return Mathf.Round(value); // 修正为整数
                default:
                    return value;
            }
        }

        /// <summary>
        /// 计算最终属性值
        /// </summary>
        public static float CalculateValue(this AttributeType attribute, float baseValue, float additionValue, float multipleValue)
        {
            return Fix(attribute, (baseValue + additionValue) * (1f + multipleValue));
        }

        /// <summary>
        /// 应用修改
        /// </summary>
        /// <param name="modifyType"></param>
        /// <param name="value"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static float Apply(this AttributeModifyType modifyType, float value, float modifier)
        {
            switch (modifyType)
            {
                case AttributeModifyType.DirectAddition:
                    return value + modifier;
                case AttributeModifyType.DirectSet:
                    return modifier;
                case AttributeModifyType.SetIfLess:
                    return Mathf.Max(value, modifier);
            }

            return value;
        }
    }
}