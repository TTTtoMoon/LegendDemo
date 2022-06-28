using System;
using System.Reflection;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 伤害请求
    /// </summary>
    public struct DamageRequest
    {
        /// <summary>
        /// 伤害来源
        /// </summary>
        public Ability Ability;

        /// <summary>
        /// 伤害创建者
        /// </summary>
        public IDamageMaker DamageMaker;

        /// <summary>
        /// 伤害承受者
        /// </summary>
        public IDamageTaker DamageTaker;

        /// <summary>
        /// 伤害系数(技能的伤害系数)
        /// </summary>
        public float DamageCoefficient;
        
        /// <summary>
        /// 伤害来源位置
        /// </summary>
        public Vector3 SourcePosition;

        /// <summary>
        /// 伤害材质
        /// </summary>
        public DamageMaterial DamageMaterial;

        /// <summary>
        /// 打断等级
        /// </summary>
        public float HurtAttackLevel;
        
        /// <summary>
        /// 击退等级
        /// </summary>
        public float RetreatAttackLevel;

        /// <summary>
        /// 击退速度
        /// </summary>
        public float RetreatSpeed;

        /// <summary>
        /// 击退朝向(相对/绝对)
        /// </summary>
        public bool RetreatAbsolute;
                
        /// <summary>
        /// 是否触发连杀减速
        /// </summary>
        public bool TriggerComboKill;

        /// <summary>
        /// 击中音效
        /// </summary>
        public AudioClip HitSound;

        /// <summary>
        /// 击中特效
        /// </summary>
        public GameObject HitVFX;

        /// <summary>
        /// 能力标签
        /// </summary>
        public AbilityTag Tag;
        
        public override string ToString()
        {
            FieldInfo[] fileds = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            string      str    = "";
            for (int i = 0; i < fileds.Length; i++)
            {
                str += $"{fileds[i].Name} : {fileds[i].GetValue(this)}\n";
            }

            return str;
        }
    }
}