using System;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 效果器模式
    /// </summary>
    [Serializable]
    public abstract class OrbMode
    {
        public Orb Orb { get; private set; }

        public void Enable(Orb orb)
        {
            if (Orb != null) return;
            Orb = orb;
            OnEnable();
        }

        public void Disable()
        {
            if (Orb == null) return;
            OnDisable();
            Orb = null;
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();
        
        protected Vector3? m_RotateAtPosition;
        protected Vector3? m_RotateToDirection;

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMove()
        {
            if(Orb != null && Orb.Enable)
                OnDisable();
        }

        /// <summary>
        /// 旋转方向
        /// </summary>
        /// <param name="atPosition"></param>
        /// <param name="toDirection"></param>
        public void Rotate(Vector3 atPosition, Vector3 toDirection)
        {
            m_RotateAtPosition  = atPosition;
            m_RotateToDirection = toDirection;
        }

        /// <summary>
        /// 进行旋转
        /// </summary>
        /// <returns>是否进行了旋转</returns>
        protected bool ApplyRotate()
        {
            if (m_RotateAtPosition != null && m_RotateToDirection != null)
            {
                Orb.transform.position = m_RotateAtPosition.Value;
                Orb.transform.forward  = m_RotateToDirection.Value;
                m_RotateAtPosition  = null;
                m_RotateToDirection = null;
                Orb.TranslatePosition(Orb.transform.forward * Orb.Length);
                return true;
            }

            return false;
        }

        public OrbMode Clone()
        {
            return (OrbMode)MemberwiseClone();
        }
    }
}