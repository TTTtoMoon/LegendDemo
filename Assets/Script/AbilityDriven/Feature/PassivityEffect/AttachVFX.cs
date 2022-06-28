using System;
using Abilities;
using RogueGods.Gameplay.VFX;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("附加特效")]
    public sealed class AttachVFX : PassivityFeatureEffect
    {
        [Description("特效资源")]
        public GameObject VFX;

        [Description("位置偏移")] 
        public Vector3 Offset;

        [Description("大小缩放")] 
        public float VFXScale = 1;
        
        [Description("跟随挂点")] 
        public TransformSlot Slot;

        [Description("跟随模式")] 
        public TransformFollower.Mode FollowMode = TransformFollower.Mode.All;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override EffectRevocation Invoke(IAbilityTarget target)
        {
            VFXInstance vfxInstance = GameManager.VFXSystem.CreateInstance(VFX, false, false);
            Transform   slot        = target.transform;

            if (target is Actor actor)
            {
                slot = actor.SlotPoint.GetSlotTransform(Slot);
                if (slot == null) slot = target.transform;
            }

            if (Ability.Owner is Orb orb && orb.Mode is ParabolaMode)
            {
                Vector3 localPosition = (FollowMode & TransformFollower.Mode.Position) != TransformFollower.Mode.None ? Offset : Vector3.zero;
                Vector3 localScale    = (FollowMode & TransformFollower.Mode.Scale)    != TransformFollower.Mode.None ? VFXScale * Vector3.one : Vector3.one;
                orb.AddVFX(vfxInstance, localPosition, localScale);
                vfxInstance.gameObject.SetActive(true);
            }
            else
            {
                vfxInstance.transform.Follow(slot, new TransformFollower.Params()
                {
                    LocalPosition = Offset,
                    LocalScale    = VFXScale * Vector3.one,
                    UpdateMode    = FollowMode
                });
                vfxInstance.gameObject.SetActive(true);
            }

            return () => { GameManager.VFXSystem.DestroyInstance(vfxInstance); };
        }
    }
}