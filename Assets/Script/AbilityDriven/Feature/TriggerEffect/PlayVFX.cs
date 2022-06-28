using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.VFX;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("在目标位置播放特效")]
    public sealed class PlayVFX : TriggerFeatureEffect
    {
        [Description("特效资源")]
        public GameObject VFX;

        [Description("创建位置")] 
        public TransformSlot Position;
        
        [Description("位置偏移")] 
        public Vector3 Offset;

        [Description("旋转偏移")] 
        public Vector3 Rotation;

        [Description("大小缩放")]
        public AbilityVariable VFXScale = new AbilityVariable(1f);
        
        [Description("跟随目标时长", "[<0永久] [0不跟随] [>0跟随n秒]")]
        public float FollowDuration;
        
        [Description("跟随模式")] [HideIf(nameof(FollowDuration), 0f)] 
        public TransformFollower.Mode FollowMode;

        [Description("失效时强制销毁？")]
        public bool DestroyWhenDisable;

        private List<VFXInstance> m_Instances;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            if (m_Instances != null)
            {
                for (int i = 0, length = m_Instances.Count; i < length; i++)
                {
                    GameManager.VFXSystem.DestroyInstance(m_Instances[i]);
                }
                
                ListPool<VFXInstance>.Release(m_Instances);
                m_Instances = null;
            }
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0, length = targets.Count; i < length; i++)
            {
                CreateVFX(targets[i]);
            }
        }

        private void CreateVFX(IAbilityTarget target)
        {
            Transform   transform     = target is Actor actor ? actor.SlotPoint.GetSlotTransform(Position) : target.transform;
            Quaternion  localRotation = Quaternion.Euler(Rotation);
            VFXInstance vfxInstance;
            if (FollowDuration >= 0 && FollowDuration < float.Epsilon)
            {
                Vector3    position = transform.position + transform.rotation * Offset;
                Quaternion rotation = transform.rotation * localRotation;
                vfxInstance = GameManager.VFXSystem.CreateInstance(VFX, position, rotation);
            }
            else
            {
                TransformFollower.Params @params = new TransformFollower.Params()
                {
                    LocalPosition = Offset,
                    LocalScale    = VFXScale * Vector3.one,
                    LocalRotation = localRotation,
                    UpdateMode    = FollowMode,
                    Duration      = FollowDuration,
                };

                vfxInstance = GameManager.VFXSystem.CreateInstance(VFX, transform, @params);
            }

            if (DestroyWhenDisable)
            {
                if (m_Instances == null)
                {
                    m_Instances = ListPool<VFXInstance>.Get();
                }
                
                m_Instances.Add(vfxInstance);
            }
        }
    }
}