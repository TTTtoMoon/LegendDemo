using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 跟随者
    /// </summary>
    public partial class TransformFollower
    {
        public const Mode  PositionAndRotation  = Mode.Position | Mode.Rotation;
        public const float AlwaysFollowDuration = -1f;

        private int        m_ID;
        private Transform  m_Follower, m_Target;
        private Vector3    m_LocalPosition;
        private Quaternion m_LocalRotation;
        private Vector3    m_LocalScale;
        private Rate       m_Rate;
        private Mode       m_Mode;
        private float      m_Duration;

        /// <summary>
        /// 唯一标识
        /// </summary>
        public int ID => m_ID;
        
        /// <summary>
        /// 是否正在跟随
        /// </summary>
        public bool IsFollowing => Following.ContainsKey(m_ID);

        /// <summary>
        /// 跟随者
        /// </summary>
        public Transform Follower => m_Follower;

        /// <summary>
        /// 目标
        /// </summary>
        public Transform Target
        {
            get => m_Target;
            set => m_Target = value;
        }

        /// <summary>
        /// 更新频率
        /// </summary>
        public Rate UpdateRate
        {
            get => m_Rate;
            set
            {
                Stop();
                m_Rate = value;
                Begin();
            }
        }

        /// <summary>
        /// 更新方式
        /// </summary>
        public Mode UpdateMode
        {
            get => m_Mode;
            set => m_Mode = value;
        }

        /// <summary>
        /// 本地坐标
        /// </summary>
        public Vector3 LocalPosition
        {
            get => m_LocalPosition;
            set => m_LocalPosition = value;
        }

        /// <summary>
        /// 本地旋转
        /// </summary>
        public Quaternion LocalRotation
        {
            get => m_LocalRotation;
            set => m_LocalRotation = value;
        }

        /// <summary>
        /// 本地缩放
        /// </summary>
        public Vector3 LocalScale
        {
            get => m_LocalScale;
            set => m_LocalScale = value;
        }

        /// <summary>
        /// 开始跟随
        /// </summary>
        public void Begin()
        {
            if (m_Follower == null || m_Target == null)
            {
                Debugger.LogError("跟随者或目标为空，无法进行跟随。");
                ObjectPool<TransformFollower>.Release(this);
                return;
            }

            Following[m_ID] = this;
            switch (m_Rate)
            {
                case Rate.Update:
                    UpdateFollowing.Add(this);
                    break;
                case Rate.FixedUpdate:
                    FixedUpdateFollowing.Add(this);
                    break;
                case Rate.LateUpdate:
                    LateUpdateFollowing.Add(this);
                    break;
            }

            Vector3    position = (m_Mode & Mode.Position) == Mode.Position ? m_Target.position + m_Target.rotation * m_LocalPosition : m_LocalPosition;
            Quaternion rotation = (m_Mode & Mode.Rotation) == Mode.Rotation ? m_Target.rotation * m_LocalRotation : m_LocalRotation;
            Vector3    scale    = m_LocalScale;
            if ((m_Mode & Mode.Scale) == Mode.Scale)
            {
                Vector3 lossyScale = m_Target.lossyScale;
                scale.x *= lossyScale.x;
                scale.y *= lossyScale.y;
                scale.z *= lossyScale.z;
            }

            m_Follower.SetPositionAndRotation(position, rotation);
            m_Follower.localScale = scale;
        }

        /// <summary>
        /// 停止跟随
        /// </summary>
        public void Stop()
        {
            if (Following.Remove(m_ID) == false) return;
        
            switch (m_Rate)
            {
                case Rate.Update:
                    UpdateFollowing.Remove(this);
                    break;
                case Rate.FixedUpdate:
                    FixedUpdateFollowing.Remove(this);
                    break;
                case Rate.LateUpdate:
                    LateUpdateFollowing.Remove(this);
                    break;
            }

            ObjectPool<TransformFollower>.Release(this);
        }

        /// <summary>
        /// 更新transform
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateTransform(float deltaTime)
        {
            if (m_Follower == null || m_Target == null)
            {
                Stop();
                return;
            }

            if (m_Duration > 0f)
            {
                m_Duration -= deltaTime;
                if (m_Duration <= 0f)
                {
                    Stop();
                    return;
                }
            }

            if ((m_Mode & Mode.Position) == Mode.Position)
            {
                Vector3 position = m_Target.position + m_Target.rotation * m_LocalPosition;
                if (m_Follower.position != position)
                    m_Follower.position = position;
            }

            if ((m_Mode & Mode.Rotation) == Mode.Rotation)
            {
                Quaternion rotation = m_Target.rotation * m_LocalRotation;
                if (m_Follower.rotation != rotation)
                    m_Follower.rotation = rotation;
            }

            if ((m_Mode & Mode.Scale) == Mode.Scale)
            {
                Vector3 scale      = m_LocalScale;
                Vector3 lossyScale = m_Target.lossyScale;
                scale.x *= lossyScale.x;
                scale.y *= lossyScale.y;
                scale.z *= lossyScale.z;
                if (m_Follower.localScale != scale)
                    m_Follower.localScale = scale;
            }
        }
    }
}