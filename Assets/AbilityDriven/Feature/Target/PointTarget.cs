using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Target
{
    public class PointTarget : IAbilityTarget
    {
        public PointTarget(Vector3 position, Quaternion rotation)
        {
            m_Transform = new GameObject().transform;
            m_Position  = position;
            m_Rotation  = rotation;
        }

        private Transform  m_Transform;
        private Vector3    m_Position;
        private Quaternion m_Rotation;

        public bool IsValidAndActive()
        {
            return m_Transform != null;
        }

        Transform IAbilityTarget.transform => m_Transform;

        Vector3 IAbilityTarget.Position => m_Position;

        Quaternion IAbilityTarget.Rotation => m_Rotation;
    }
}