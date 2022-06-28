using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay
{
    public partial class DashAgent
    {
        private int?    m_ID;
        private Actor   m_Actor;
        private float   m_Duration;
        private Vector3 m_StartPosition, m_EndPosition;

        private float m_Timer;

        private void Initialize(int id,
            Actor                   actor,
            Vector3                 direction,
            float                   distance,
            float                   duration)
        {
            if (m_ID == null)
            {
                actor.DisableRootMotion();
                m_DashAgents.Add(this);
            }

            direction.y = 0f;
            direction.Normalize();
            m_ID            = id;
            m_Actor         = actor;
            m_StartPosition = actor.Position;
            m_Duration      = duration;
            m_EndPosition   = CalculateEndPosition(actor, direction, distance);
            m_Timer         = 0f;
        }

        private void UpdatePosition()
        {
            if (m_Actor == null || m_Actor.isActiveAndEnabled == false)
            {
                Stop();
                return;
            }

            m_Timer += Time.deltaTime;
            float   percent      = m_Timer / m_Duration;
            Vector3 nextPosition = Vector3.Lerp(m_StartPosition, m_EndPosition, percent);
            m_Actor.NavMeshAgent.Warp(nextPosition);
            if (m_Timer >= m_Duration)
            {
                Stop();
            }
        }

        private void Stop()
        {
            if (m_Actor)
            {
                m_Actor.EnableRootMotion();
            }

            m_ID    = null;
            m_Actor = null;
            if (m_DashAgents.Remove(this))
            {
                ObjectPool<DashAgent>.Release(this);
            }
        }

        public static Vector3 CalculateEndPosition(Actor actor, Vector3 direction, float distance)
        {
            Vector3 startPosition    = actor.Position;
            Vector3 basicEndPosition = startPosition + distance * direction;
            basicEndPosition.y = 0f;
            Vector3 endPosition = basicEndPosition;

            if (Physics.Linecast(startPosition, basicEndPosition, out RaycastHit hitInfo))
            {
                endPosition = hitInfo.point;
            }
            else
            {
                Vector3 clampedPosition = NavMesh.SamplePosition(basicEndPosition, out NavMeshHit hit, 5f, -1) ? hit.position : startPosition;
                endPosition = clampedPosition;
            }

            return endPosition;
        }
    }
}