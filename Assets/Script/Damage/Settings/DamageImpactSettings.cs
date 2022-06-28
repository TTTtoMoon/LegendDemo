using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay
{
    [Serializable]
    public struct DamageImpactConfig
    {
#if UNITY_EDITOR
        private string Name => $"{DamageMaterial} => {TakerMaterial}";
#endif

        [Serializable]
        public struct VFXScale
        {
            public BodyType Body;
            public float    Scale;
        }

        [LabelText("伤害材质")]
        public DamageMaterial DamageMaterial;

        [LabelText("受击材质")]
        public BodyMaterial TakerMaterial;

        [LabelText("击中音效")] 
        public AudioClip Audio;

        [LabelText("击中特效")] 
        public GameObject VFX;

        [LabelText("特效跟随?")] 
        public bool VFXFollow;
    }

    public readonly struct DamageImpactSetting
    {
        public DamageImpactSetting(AudioClip audio, GameObject vfx, bool vfxFollow, float vfxScale)
        {
            Audio     = audio;
            VFX       = vfx;
            VFXFollow = vfxFollow;
            VFXScale  = vfxScale;
        }

        public readonly AudioClip  Audio;
        public readonly GameObject VFX;
        public readonly bool       VFXFollow;
        public readonly float      VFXScale;
    }
}