using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("正弦波模式")]
    public class SineWaveMode : OrbMode
    {
        [Description("移动速度(米/秒)")]
        public AbilityVariable MoveSpeed = new AbilityVariable(10f);
        
        [Description("波峰")]
        public AbilityVariable WavePeak = new AbilityVariable(2.0f);
        
        [Description("频率")]
        public AbilityVariable PIDelta = new AbilityVariable(1.0f);
        
        private Vector3 m_OriginPosition;
        protected override void OnEnable()
        {
            GameManager.Instance.RegisterUpdate(UpdateTransform, order: GameManager.UpdateMonoOrder.UpdateTransform);
            Orb.OnCrash.AddListener(OnCrash);
            m_OriginPosition = Orb.transform.position;
            m_PIAccumulae = 0;
        }

        protected override void OnDisable()
        {
            GameManager.Instance.UnRegisterUpdate(UpdateTransform);    
            Orb.OnCrash.RemoveListener(OnCrash);
        }

        private float m_PIAccumulae = 0;

        private void UpdateTransform()
        {
            var orbTransform = Orb.transform;
            m_OriginPosition += orbTransform.forward * Time.deltaTime * MoveSpeed;
            m_PIAccumulae    += PIDelta              * Time.deltaTime;
            m_PIAccumulae    %= 6.28f;
            var sineWave = WavePeak * Mathf.Sin(m_PIAccumulae);
            Orb.TranslatePosition(m_OriginPosition + Orb.transform.right * sineWave - orbTransform.position);
        }
        
        private bool OnCrash(IAbilityTarget target)
        {
            GameManager.OrbSystem.Destroy(Orb, OrbDestroyOption.DestroyWhenHitActor);
            return false;
        }
    }
}