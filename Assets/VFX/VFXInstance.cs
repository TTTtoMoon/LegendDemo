using System;
using System.Collections.Generic;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.VFX
{
    public class VFXInstance : MonoBehaviour
    {
        [Serializable]
        [InlineProperty]
        public class RandomAngle
        {
            [HorizontalGroup] [HideLabel] 
            public bool Enable;

            [HorizontalGroup] [HideLabel] [EnableIf("Enable")] [MinMaxSlider(-180f, 180f, true)]
            public Vector2 Range = new Vector2(-180f, 180f);

            public float Random()
            {
                return Enable ? UnityEngine.Random.Range(Range.x, Range.y) : 0f;
            }
        }

        [Serializable]
        [InlineProperty]
        public struct AudioInfo
        {
            [LabelText("延时(单位秒)")] [MinValue(0f)] 
            public float Delay;

            [LabelText("音频")]
            public AudioClip Audio;

            [LabelText("音量")] [Range(0f, 1f)] 
            public float Volume;
        }

#if UNITY_EDITOR
        private bool HaveDestroyTime => DestroyTime > 0f;
#endif

        #region Config Data
        
        [LabelText("持续时间")] 
        public float LifeTime = -1f;

        [LabelText("销毁时间")] [MinValue(0f)] 
        public float DestroyTime = 0f;

        [LabelText("使用Animator来进行消失动画？临时方案")] [ShowIf("HaveDestroyTime")]
        public bool UseAnimator = false;

        [LabelText("特效根节点")] 
        public Transform EffectRoot;

        [LabelText("阴影根节点")]
        public Transform ShadowRoot;

        [TitleGroup("随机初始角度")] [LabelText("X")] [LabelWidth(30F)]
        public RandomAngle RandomX = new RandomAngle();

        [TitleGroup("随机初始角度")] [LabelText("Y")] [LabelWidth(30F)]
        public RandomAngle RandomY = new RandomAngle();

        [TitleGroup("随机初始角度")] [LabelText("Z")] [LabelWidth(30F)]
        public RandomAngle RandomZ = new RandomAngle();

        [LabelText("激活时的音效")] 
        public AudioInfo[] ActiveAudios = new AudioInfo[0];

        [LabelText("消散时的音效")] [ShowIf("HaveDestroyTime")]
        public AudioInfo[] DestroyAudios = new AudioInfo[0];
        
        #endregion

        #region Runtime Data

        /// <summary>
        /// 特效所属的池子
        /// </summary>
        public AsyncPrefabPool<VFXInstance> Pool { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public VFXState State { get; set; }

        /// <summary>
        /// 是否自动销毁
        /// </summary>
        public bool AutoDestroy { get; set; }

        /// <summary>
        /// 在什么时候激活的
        /// </summary>
        public float ActiveAtTime { get; set; }

        #endregion

        private Animator[]       m_Animators;
        private TrailRenderer[]  m_TrailRenderers;
        private ParticleSystem[] m_ParticleSystems;
        private bool[]           m_ParticleSystemEmission;

        public void PlayDestroyAnimation()
        {
            if (UseAnimator)
            {
                foreach (var animator in m_Animators)
                {
                    animator.SetTrigger(AnimationDefinition.Parameter.Disappear);
                }
            }
            else
            {
                for (int i = 0, length = m_ParticleSystems.Length; i < length; i++)
                {
                    ParticleSystem.EmissionModule emission = m_ParticleSystems[i].emission;
                    emission.enabled = false;
                }
            }
        }

        private void Awake()
        {
            if (EffectRoot == null)
            {
                EffectRoot = transform.Find("Effect");
            }

            if (ShadowRoot == null)
            {
                ShadowRoot = transform.Find("Shadow");
            }

            m_Animators              = GetComponentsInChildren<Animator>(true);
            m_TrailRenderers         = GetComponentsInChildren<TrailRenderer>(true);
            m_ParticleSystems        = GetComponentsInChildren<ParticleSystem>(true);
            m_ParticleSystemEmission = new bool[m_ParticleSystems.Length];
            for (int i = 0, length = m_ParticleSystems.Length; i < length; i++)
            {
                m_ParticleSystemEmission[i] = m_ParticleSystems[i].emission.enabled;
            }
        }

        private void OnEnable()
        {
            for (int i = 0, length = m_ParticleSystems.Length; i < length; i++)
            {
                ParticleSystem.EmissionModule emission = m_ParticleSystems[i].emission;
                emission.enabled = m_ParticleSystemEmission[i];
            }

            Vector3 randomRotation = new Vector3(RandomX.Random(), RandomY.Random(), RandomZ.Random());
            transform.rotation *= Quaternion.Euler(randomRotation);
        }

        private void OnDisable()
        {
            foreach (var animator in m_Animators)
            {
                animator.ResetTrigger(AnimationDefinition.Parameter.Disappear);
            }

            for (int i = 0, length = m_TrailRenderers.Length; i < length; i++)
            {
                m_TrailRenderers[i].Clear();
            }
        }
    }
}