using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 能力工具类
    /// </summary>
    public static class AbilityUtility
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static bool IsNull<T>(this T target) where T : class
        {
            return target is Object unityObject ? unityObject == null : target == null;
        }
        
        /// <summary>
        /// 装配
        /// </summary>
        /// <param name="factory">能力工厂</param>
        public static void SetUp(this IAbilityFactory factory)
        {
            Ability.s_Factory = factory;
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="variableTable">变量表</param>
        /// <param name="uniqueName">变量名</param>
        /// <param name="fallback">查找失败返回值</param>
        /// <returns>返回是否存在变量</returns>
        public static float GetVariableValue(this IAbilityVariableTable variableTable, string uniqueName, float fallback = default)
        {
            return variableTable.GetVariableValue(uniqueName, out float value) ? value : fallback;
        }
    }
}