using System;
using System.Collections;
using Abilities;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("发射法球：散射")]
    public sealed class Scattering : OrbEmission
    {
        [Description("发射器特效")] [AssetAddressSelector(AssetAddressSelector.AssetType.VFX)]
        public string EmissionVFX;

        [Description("散射半径")]
        public float Radius = 0f;

        [Description("散射朝向(与正前方的角度差值)")] [Range(-180f, 180f)]
        public float Direction = 0f;

        [Description("散射角度")] [Range(5f, 360f)]
        public float Angle = 60f;

        [Description("散射时间间隔")] [Min(0f)]
        public float Timespan = 0f;

        [Description("散射顺序")] [ValueDropdown("GetScatteringSeq")]
        public bool Sequence = false;

        [Description("散射数量")]
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

            GetPositionAndRotationByAngle(Direction, out Vector3 vfxPosition, out _);
            SystemController.VFXSystem.CreateInstance(EmissionVFX, vfxPosition, Quaternion.identity);
            if (EmitDelay > 0f)
            {
                yield return TimeDefine.GetSeconds(EmitDelay);
            }

            if (EmitOrbCount > 1)
            {
                float startAngle;
                float offsetAngle;
                if (Sequence)
                {
                    startAngle  = Direction + Angle / 2f;
                    offsetAngle = -Angle / EmitOrbCount;
                }
                else
                {
                    startAngle  = Direction - Angle / 2f;
                    offsetAngle = Angle / EmitOrbCount;
                }

                for (int i = 0, length = EmitOrbCount; i < length; i++)
                {
                    GetPositionAndRotationByAngle(startAngle + i * offsetAngle, out Vector3 orbPosition, out Quaternion orbRotation);
                    Emit(target, orbPosition, orbRotation);
                    if (Timespan > 0f)
                    {
                        yield return TimeDefine.GetSeconds(Timespan);
                    }
                }
            }
            else
            {
                GetPositionAndRotationByAngle(Direction, out Vector3 orbPosition, out Quaternion orbRotation);
                Emit(target, orbPosition, orbRotation);
            }
            
            void GetPositionAndRotationByAngle(float _Angle, out Vector3 _Position, out Quaternion _Rotation)
            {
                _Position =  EmitAtNewestPosition ? target.Position : sourcePosition;
                _Rotation =  EmitAtNewestPosition ? target.Rotation : sourceRotation;
                _Rotation *= Quaternion.AngleAxis(_Angle, Vector3.up);
                _Position += _Rotation * Vector3.forward * Radius + EmitPosition.GetPosition(target);
            }
        }
        
#if UNITY_EDITOR

        private ValueDropdownList<bool> GetScatteringSeq
        {
            get
            {
                ValueDropdownList<bool> list = new ValueDropdownList<bool>();
                list.Add("从左往右", false);
                list.Add("从右往左", true);
                return list;
            }
        }

#endif
    }
}