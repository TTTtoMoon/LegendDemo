using System.Diagnostics;
using RogueGods.Gameplay.AbilityDriven;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 伤害事件监听者
    /// </summary>
    public delegate void DamageListener(DamageResponse damageInfo);

    /// <summary>
    /// 伤害修改器
    /// </summary>
    public delegate void DamageModifier(in DamageRequest damageRequest, ref Attacker attacker, ref Defender defender);

    public class DamageSystem : GameSystem
    {
        public DamageSettings Settings { get; private set; }
        public event DamageModifier    DamageModifier;

        public override void Awake()
        {
            base.Awake();
            Settings = Resources.Load<DamageSettings>(nameof(DamageSettings));
        }

        public bool ApplyDamage(DamageRequest request, DamageListener onMadeDamage = null)
        {
            if (request.DamageMaker                        == null) return false;
            if (request.DamageTaker                        == null) return false;
            if (request.DamageMaker.CanMakeDamage(request) == false) return false;
            if (request.DamageTaker.CanTakeDamage(request) == false) return false;

#if UNITY_EDITOR
            DamageSampler.BeginSample(request);
#endif
            Attacker attacker = Attacker.Create();
            Defender defender = Defender.Create();
            request.DamageMaker.PrepareMakeDamage(request, ref attacker);
            request.DamageTaker.PrepareTakeDamage(request, ref defender);
            DamageModifier?.Invoke(request, ref attacker, ref defender);

            DamageResponse response = new DamageResponse(request, attacker, defender);
#if UNITY_EDITOR
            DamageSampler.EndSample(response);
#endif
            request.DamageMaker.MadeDamage(response);
            onMadeDamage?.Invoke(response); // 先执行造成伤害回调，再触发受到伤害，否则可能导致受击者受伤死亡而出现错误
            PlayDamageEffect(request);
            request.DamageTaker.TakeDamage(response);
            return true;
        }

        private void PlayDamageEffect(in DamageRequest descriptor)
        {
            // Dot伤不造成任何受击反馈
            if (descriptor.Ability?.Tag == AbilityTagDefine.Dot)
            {
                return;
            }

            BodyType            takerBodyType = descriptor.DamageTaker.BodyType;
            DamageSettings      settings      = GameManager.DamageSystem.Settings;
            DamageImpactSetting impactSetting = settings.GetDamageImpactSetting(descriptor.DamageMaterial, descriptor.DamageTaker.Material, takerBodyType);
            if (impactSetting.Audio != null)
            {
                AudioSource.PlayClipAtPoint(impactSetting.Audio, descriptor.SourcePosition);
            }

            if (descriptor.HitSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSetting.Audio, descriptor.DamageTaker.HitPosition);
            }

            Vector3 hitPosition = descriptor.DamageTaker.HitPosition;
            if (impactSetting.VFX != null)
            {
                Vector3 scale = impactSetting.VFXScale * Vector3.one;
                if (impactSetting.VFXFollow)
                {
                    Transform takerTransform = descriptor.DamageTaker.transform;
                    TransformFollower.Params @params = new TransformFollower.Params()
                    {
                        LocalPosition = hitPosition - takerTransform.position,
                        LocalRotation = Quaternion.identity,
                        LocalScale    = scale,
                        UpdateMode    = TransformFollower.Mode.Position,
                    };
                    GameManager.VFXSystem.CreateInstance(impactSetting.VFX, takerTransform, @params);
                }
                else
                {
                    GameManager.VFXSystem.CreateInstance(impactSetting.VFX, hitPosition, Quaternion.identity, impactSetting.VFXScale);
                }
            }

            if (descriptor.HitVFX != null)
            {
                GameManager.VFXSystem.CreateInstance(descriptor.HitVFX, hitPosition, Quaternion.identity);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushModify(string description, in Attacker attacker)
        {
#if UNITY_EDITOR
            DamageSampler.PushModify(description, attacker);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushModify(string description, in Defender defender)
        {
#if UNITY_EDITOR
            DamageSampler.PushModify(description, defender);
#endif
        }
    }
}