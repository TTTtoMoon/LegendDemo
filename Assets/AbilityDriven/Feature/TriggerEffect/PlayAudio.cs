using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("播放音效")]
    public sealed class PlayAudio : TriggerFeatureEffect
    {
        [Description("音效资源")]
        public AudioClip Audio;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                AudioSource.PlayClipAtPoint(Audio, targets[i].Position);
            }
        }
    }
}