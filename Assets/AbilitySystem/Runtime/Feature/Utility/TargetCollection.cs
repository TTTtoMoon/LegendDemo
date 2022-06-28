using System.Collections;
using System.Collections.Generic;

namespace Abilities
{
    /// <summary>
    /// 目标集合
    /// </summary>
    public struct TargetCollection : IReadOnlyList<IAbilityTarget>
    {
        private List<IAbilityTarget> m_Targets;

        internal TargetCollection(List<IAbilityTarget> targets, List<IAbilityTarget> append)
        {
            m_Targets = targets;
            m_Targets.AddRange(append);
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count => m_Targets?.Count ?? 0;

        /// <summary>
        /// 第index个目标
        /// </summary>
        /// <param name="index">索引</param>
        public IAbilityTarget this[int index] => m_Targets[index];

        public List<IAbilityTarget>.Enumerator GetEnumerator()
        {
            return m_Targets.GetEnumerator();
        }

        IEnumerator<IAbilityTarget> IEnumerable<IAbilityTarget>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}