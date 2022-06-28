using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 能力目标
    /// </summary>
    public interface IAbilityTarget
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValidAndActive();
        
        /// <summary>
        /// 小写开头，匹配MonoBehaviour.transform
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// 当前位置
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// 当前旋转角度
        /// </summary>
        Quaternion Rotation { get; }
    }
}