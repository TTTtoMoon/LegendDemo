using System;
using Abilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("创建怪物")]
    public sealed class CreateMonster : TriggerFeatureEffect
    {
        [Description("怪物ID")]
        public Actor Monster;

        [Description("位置偏移")]
        public Vector3 PositionOffset;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            if (Monster == null) return;
            Object.Instantiate(Monster.gameObject, Ability.Owner.Position + PositionOffset, Quaternion.identity);
        }
    }
}