using UnityEngine;

namespace RogueGods.Gameplay
{
    public static class TransformFollowerExtension
    {
        /// <summary>
        /// 跟随目标
        /// </summary>
        /// <param name="follower">跟随者</param>
        /// <param name="target">跟随目标</param>
        /// <param name="params">跟随参数</param>
        public static TransformFollower Follow(this Transform follower, Transform target, TransformFollower.Params @params)
        {
            return TransformFollower.Begin(follower, target, @params);
        }

        /// <summary>
        /// 停止跟随目标
        /// </summary>
        /// <param name="follower"></param>
        public static void StopFollow(this Transform follower)
        {
            TransformFollower.Stop(follower);
        }
    }
}