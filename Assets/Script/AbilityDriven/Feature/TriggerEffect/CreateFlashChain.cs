using System;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("在目标位置创建闪电链")]
    public sealed class CreateFlashChain : TriggerFeatureEffect
    {
        private const float Height = 1f;

        [Description("链接特效")]
        public GameObject Chain;

        [Description("链接范围")]
        public AbilityVariable ChainRange = new AbilityVariable(10f);

        [Description("链接间隔")] [MinValue(0f)] 
        public float ChainInterval = 0.05f;
        
        [Description("闪电持续时间")] [MinValue(0f)] 
        public float ChainDuration = 0.1f;
        
        [Description("闪电最大击中个数")]
        public AbilityVariable ChainSpreadCount = new AbilityVariable(0);

        [Description("伤害比率")]
        public AbilityVariable DamageCoefficient = new AbilityVariable(1f);

        [Description("伤害材质")]
        public DamageMaterial DamageMaterial;

        [Description("打断等级")]
        public AbilityVariable HurtAttackLevel = new AbilityVariable(0f);

        [Description("击退等级")]
        public AbilityVariable RetreatAttackLevel = new AbilityVariable(0f);

        [Description("击退速度")]
        public AbilityVariable RetreatSpeed = new AbilityVariable(0f);

        [Description("击退朝向(绝对/相对)", "true:绝对 false:相对")]
        public bool RetreatAbsolute;

        [Description("击中音效")]
        public AudioClip HitSound;
        
        [Description("击中特效")]
        public GameObject HitVFX;

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
                CreateChain(targets[i]);
            }
        }

        private void CreateChain(IAbilityTarget target)
        {
            Vector3 position = target.Position;
            position.y = Height;
            GameManager.FlashChainSystem.AddLightChain(Chain, position, ChainRange.Value, ChainInterval, ChainDuration, ChainSpreadCount.IntValue, MakeDamage);

            void MakeDamage(Actor actor)
            {
                if (Ability == null) return;
                DamageRequest damageRequest = new DamageRequest()
                {
                    Ability            = Ability,
                    DamageMaker        = Ability.Source as IDamageMaker,
                    DamageTaker        = actor,
                    DamageCoefficient  = DamageCoefficient,
                    SourcePosition     = position,
                    DamageMaterial     = DamageMaterial,
                    HurtAttackLevel    = HurtAttackLevel,
                    RetreatAttackLevel = RetreatAttackLevel,
                    RetreatSpeed       = RetreatSpeed,
                    RetreatAbsolute    = RetreatAbsolute,
                    HitSound           = HitSound,
                    HitVFX             = HitVFX,
                    Tag                = Ability.Tag,
                };
                
                GameManager.DamageSystem.ApplyDamage(damageRequest);
            }
        }
    }
}