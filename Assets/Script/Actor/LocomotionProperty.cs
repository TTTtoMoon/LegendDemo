using System;
using UnityEngine;

namespace RogueGods.Gameplay
{
    [Serializable]
    public sealed class LocomotionProperty
    {
        [SerializeField] private float m_RotationSpeed        = 3600f;
        [SerializeField] private float m_MovementSpeed        = 7.8f;
        [SerializeField] private float m_MovementAcceleration = 50f;

        public float RotationSpeed        => m_RotationSpeed;
        public float MovementSpeed        => m_MovementSpeed;
        public float MovementAcceleration => m_MovementAcceleration;
    }
}