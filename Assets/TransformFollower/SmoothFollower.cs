using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay
{
    [DefaultExecutionOrder(300)]
    public class SmoothFollower : MonoBehaviour
    {
        private static List<SmoothFollower> m_ActiveFollowers = new List<SmoothFollower>();

        public static void SyncPositionImmediately()
        {
            for (int i = 0; i < m_ActiveFollowers.Count; i++)
            {
                m_ActiveFollowers[i].m_JustEnable = true;
            }
        }

        [LabelText("跟随目标")]           public Transform Target;
        [LabelText("缓动速度")] [Min(0f)] public float     MoveSmoothSpeed;
        [LabelText("缓动加速")] [Min(0f)] public float     Acceleration;
        [LabelText("跟随旋转")]           public bool      FollowRotation;
        [LabelText("缓动旋转")] [Min(0f)] public float     RotateSmoothSpeed;

        private float      m_CurrentSpeed;
        private Vector3    m_PrePosition;
        private Quaternion m_PreRotation;

        private bool m_JustEnable;

        private void OnEnable()
        {
            m_JustEnable = true;
            m_ActiveFollowers.Add(this);
        }

        private void OnDisable()
        {
            m_ActiveFollowers.Remove(this);
        }

        private void LateUpdate()
        {
            if (m_JustEnable)
            {
                transform.position = Target.position;
                transform.rotation = Target.rotation;
                m_PrePosition      = transform.position;
                m_PreRotation      = transform.rotation;
                m_JustEnable       = false;
            }

            if (Target == null) return;
            Vector3 offset   = Target.position - m_PrePosition;
            float   distance = offset.magnitude;
            if (Mathf.Approximately(distance, 0f))
            {
                m_CurrentSpeed = 0f;
                return;
            }

            m_CurrentSpeed = Mathf.MoveTowards(m_CurrentSpeed, MoveSmoothSpeed, Time.deltaTime * Acceleration);
            float   deltaSpeed = Time.deltaTime * m_CurrentSpeed;
            Vector3 direction  = offset         / distance;
            m_PrePosition      += Mathf.Min(distance, deltaSpeed) * direction;
            transform.position =  m_PrePosition;
            if (FollowRotation)
            {
                m_PreRotation      = Quaternion.RotateTowards(m_PreRotation, Target.rotation, Time.deltaTime * RotateSmoothSpeed);
                transform.rotation = m_PreRotation;
            }
        }
    }
}