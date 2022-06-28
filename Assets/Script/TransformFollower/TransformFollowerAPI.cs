using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public partial class TransformFollower
    {
        /// <summary>
        /// 已创建的跟随者，防止同一对象跟随不同对象。
        /// </summary>
        private static readonly Dictionary<int, TransformFollower> Following = new Dictionary<int, TransformFollower>();

        private static readonly List<TransformFollower> UpdateFollowing      = new List<TransformFollower>();
        private static readonly List<TransformFollower> FixedUpdateFollowing = new List<TransformFollower>();
        private static readonly List<TransformFollower> LateUpdateFollowing  = new List<TransformFollower>();

        static TransformFollower()
        {
            GameManager.Instance.RegisterUpdate(Update, order: GameManager.UpdateMonoOrder.Last);
            GameManager.Instance.RegisterFixedUpdate(FixedUpdate, order: GameManager.FixedOrder.Last);
            GameManager.Instance.RegisterLateUpdate(LateUpdate, order: GameManager.LateUpdateMonoOrder.Last);
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
        /// <param name="follower">跟随者</param>
        /// <param name="target">跟随目标</param>
        /// <param name="params">跟随参数</param>
        public static TransformFollower Begin(Transform follower, Transform target, Params @params)
        {
            if (follower == null || target == null || @params.UpdateMode == Mode.None) return null;

            int               id = follower.GetInstanceID();
            TransformFollower transformFollower;
            if (Following.TryGetValue(id, out transformFollower))
            {
                transformFollower.Stop();
            }

            transformFollower                 = ObjectPool<TransformFollower>.Get();
            transformFollower.m_ID            = id;
            transformFollower.m_Follower      = follower;
            transformFollower.m_Target        = target;
            transformFollower.m_LocalPosition = @params.LocalPosition ?? Vector3.zero;
            transformFollower.m_LocalRotation = @params.LocalRotation ?? Quaternion.identity;
            transformFollower.m_LocalScale    = @params.LocalScale    ?? Vector3.one;
            transformFollower.m_Rate          = @params.UpdateRate    ?? Rate.LateUpdate;
            transformFollower.m_Mode          = @params.UpdateMode    ?? Mode.All;
            transformFollower.m_Duration      = @params.Duration      ?? AlwaysFollowDuration;
            transformFollower.Begin();
            return transformFollower;
        }

        /// <summary>
        /// 停止跟随，如果在跟随中
        /// </summary>
        /// <param name="follower">跟随者</param>
        public static void Stop(Transform follower)
        {
            if (follower == null) return;
            if (Following.TryGetValue(follower.GetInstanceID(), out TransformFollower transformFollower))
            {
                transformFollower.Stop();
            }
        }

        private static void Update()
        {
            Update(UpdateFollowing, Time.deltaTime);
        }

        private static void FixedUpdate()
        {
            Update(FixedUpdateFollowing, Time.fixedDeltaTime);
        }

        private static void LateUpdate()
        {
            Update(LateUpdateFollowing, Time.deltaTime);
        }

        private static void Update(List<TransformFollower> followers, float deltaTime)
        {
            List<TransformFollower> temp = ListPool<TransformFollower>.Get();
            temp.AddRange(followers);
            for (int i = 0, length = temp.Count; i < length; i++)
            {
                temp[i].UpdateTransform(deltaTime);
            }

            ListPool<TransformFollower>.Release(temp);
        }
    }
}