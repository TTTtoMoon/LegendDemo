using System;
using System.Collections;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("延迟禁用自身效果")]
    public sealed class DelayDisableSelf : TriggerFeatureEffect
    {
        [Description("延迟时间")]
        public float DelayTime;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (DelayTime > 0)
            {
                GameManager.Instance.StartCoroutine(Delay());

                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(DelayTime);
                    Ability.Director.Disable(Ability);
                }
            }
            else
            {
                Ability.Director.Disable(Ability);
            }
        }
    }
}