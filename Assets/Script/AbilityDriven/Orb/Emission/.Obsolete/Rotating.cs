using System;
using System.Collections;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("发射法球：旋转发射")]
    public sealed class Rotating : OrbEmission
    {
        [Description("发射半径")] 
        public float Radius;

        [Description("初始角度")] 
        public float StartAngle;

        [Description("结束角度")] 
        public float EndAngle;

        [Description("发射间隔时间")] [Min(0f)] 
        public float Timespan;

        [Description("发射数量")] 
        public AbilityVariable EmitOrbCount = new AbilityVariable(1);

        protected override void Invoke(TargetCollection targets)
        {
            if (EmitOrbCount < 1)
            {
                Debugger.LogError($"Ability({Ability.ConfigurationID})无法发射子弹，因为发射数量为0");
                return;
            }

            for (int i = 0, length = targets.Count; i < length; i++)
            {
                StartEmit(Emit(targets[i]));
            }
        }

        private IEnumerator Emit(IAbilityTarget target)
        {
            Vector3    sourcePosition = target.Position;
            Quaternion sourceRotation = target.Rotation;

            float angleOffset = (EndAngle - StartAngle) / EmitOrbCount;
            for (int i = 0, length = EmitOrbCount; i < length; i++)
            {
                EmitBtAngle(StartAngle + i * angleOffset);
                if (Timespan > 0f)
                {
                    yield return TimeDefine.GetSeconds(Timespan);
                }
            }

            void EmitBtAngle(float angle)
            {
                Vector3    position = EmitAtNewestPosition ? target.Position : sourcePosition;
                Quaternion rotation = EmitAtNewestPosition ? target.Rotation : sourceRotation;
                rotation *= Quaternion.AngleAxis(angle, Vector3.up);
                position += rotation * Vector3.forward * Radius + EmitPosition.GetPosition(target);
                Emit(target, position, rotation);
            }
        }
    }
}