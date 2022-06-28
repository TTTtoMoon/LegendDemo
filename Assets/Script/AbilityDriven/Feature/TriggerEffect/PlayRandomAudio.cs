using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("播放随机音效")]
    public sealed class PlayRandomAudio : TriggerFeatureEffect
    {
        [Description("音效资源")]
        public AudioClip[] Audios;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (Audios == null || Audios.Length == 0) return;
            for (int i = 0; i < targets.Count; i++)
            {
                AudioClip audio = Audios[UnityEngine.Random.Range(0, Audios.Length)];
                AudioSource.PlayClipAtPoint(audio, targets[i].Position);
            }
        }
    }
}