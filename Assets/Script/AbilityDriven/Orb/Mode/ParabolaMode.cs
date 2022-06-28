using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.VFX;
using RogueGods.Utility;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("抛物线模式(落地时自动销毁)")]
    public sealed class ParabolaMode : OrbMode
    {
        [Description("移动速度")] 
        public AbilityVariable MoveSpeed = new AbilityVariable(10f);

        [Description("缩放模板")] [Min(0)] 
        public int ParabolaScaleLevel;

        [Description("高度模板")] [Min(0)] 
        public int ParabolaHeightLevel;

        [Description("沿切线旋转？")] 
        public bool RotateWithTangent;

        private AnimationCurve m_ScaleCurve;
        private AnimationCurve m_HeightCurve;
        private Vector3        m_DefaultScale;
        private Vector3        m_DestinationPosition;
        private Vector3        m_DestinationDirection;
        private float          m_DestinationDistance;
        private float          m_MovedDistance;

        protected override void OnEnable()
        {
            m_ScaleCurve           =  MiscConfiguration.GetParabolaScaleCurve(ParabolaScaleLevel);
            m_HeightCurve          =  MiscConfiguration.GetParabolaHeightCurve(ParabolaHeightLevel);
            m_DefaultScale         =  Orb.transform.localScale;
            m_DestinationPosition  =  Orb.TargetPosition;
            m_DestinationDirection =  (m_DestinationPosition - Orb.Position).IgnoreY();
            m_DestinationDistance  =  m_DestinationDirection.magnitude;
            m_DestinationDirection /= m_DestinationDistance;
            m_MovedDistance        =  0f;
            if (m_DestinationDistance > 0f)
            {
                GameManager.Instance.RegisterUpdate(UpdateTransform, order: GameManager.UpdateMonoOrder.UpdateTransform);
            }
            else
            {
                Debugger.LogError($"由{Orb.Ability.Giver.ConfigurationID}创建的效果器目的地就在创建点，因此无法进行抛物线运动。");
            }
        }

        protected override void OnDisable()
        {
            GameManager.Instance.UnRegisterUpdate(UpdateTransform);
        }

        private void UpdateTransform()
        {
            float moveDistance = Mathf.Min(m_DestinationDistance - m_MovedDistance, Time.deltaTime * Orb.MoveSpeedMultiplier * MoveSpeed);
            m_MovedDistance += moveDistance;
            if (m_MovedDistance + 0.001f >= m_DestinationDistance)
            {
                GameManager.OrbSystem.Destroy(Orb);
                return;
            }

            float   percent       = m_MovedDistance / m_DestinationDistance;
            Vector3 deltaPosition = moveDistance    * m_DestinationDirection;
            Orb.transform.localScale = m_ScaleCurve.Evaluate(percent) * m_DefaultScale;
            Vector3 nextPosition = Orb.Position + deltaPosition;
            Orb.UpdateVFX(nextPosition, Orb.Rotation, Orb.transform.lossyScale, m_HeightCurve.Evaluate(percent), RotateWithTangent);
            Orb.TranslatePosition(deltaPosition);
        }
    }
}