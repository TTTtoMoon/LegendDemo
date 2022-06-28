using System;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 能力标签
    /// 最多63种
    /// </summary>
    [Serializable]
    public struct AbilityTag : IEquatable<AbilityTag>
    {
        public AbilityTag(long tag)
        {
            Tag = tag;
        }

        internal const int TagLength = 64;

        [SerializeField] internal long Tag;

        /// <summary>
        /// 是否包含全部目标标签
        /// </summary>
        /// <param name="tag">目标标签</param>
        /// <returns>当前标签包含所有目标标签则返回true，否则返回false</returns>
        public readonly bool ContainsAll(in AbilityTag tag)
        {
            return (tag.Tag & Tag) == tag.Tag;
        }

        /// <summary>
        /// 是否包含任意目标标签
        /// </summary>
        /// <param name="tag">目标标签</param>
        /// <returns>当前标签包含任意一个目标标签则返回true，否则返回false</returns>
        public readonly bool ContainsAny(in AbilityTag tag)
        {
            return (tag.Tag & Tag) != 0;
        }

        public readonly bool Equals(AbilityTag other)
        {
            return Tag == other.Tag;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is AbilityTag other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Tag.GetHashCode();
        }

        public static AbilityTag operator |(in AbilityTag left, in AbilityTag right)
        {
            return new AbilityTag(left.Tag | right.Tag);
        }

        public static AbilityTag operator &(in AbilityTag left, in AbilityTag right)
        {
            return new AbilityTag(left.Tag & right.Tag);
        }

        public static AbilityTag operator ~(in AbilityTag tag)
        {
            return new AbilityTag(~tag.Tag);
        }

        public static bool operator ==(in AbilityTag left, in AbilityTag right)
        {
            return left.Tag == right.Tag;
        }

        public static bool operator !=(in AbilityTag left, in AbilityTag right)
        {
            return left.Tag != right.Tag;
        }

        public static explicit operator long(AbilityTag tag)
        {
            return tag.Tag;
        }
    }
}