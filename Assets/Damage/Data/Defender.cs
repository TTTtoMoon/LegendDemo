using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 伤害减免
    /// </summary>
    public struct Defender
    {
        /// <summary>
        /// 创建初始值
        /// </summary>
        /// <returns></returns>
        public static Defender Create()
        {
            return new Defender()
            {
                DamageReductions = 0,
                DamageTakeRatio  = 1f,
            };
        }

        /// <summary>
        /// 伤害减免固定值
        /// </summary>
        public float DamageReductions;

        /// <summary>
        /// 承伤比(0f ~ 1f)
        /// </summary>
        public float DamageTakeRatio { get; private set; }

        /// <summary>
        /// 减伤
        /// </summary>
        /// <param name="reductionsRatio">伤害减少率(0.0 ~ 1.0)</param>
        public void DeductDamageByRatio(float reductionsRatio)
        {
            DamageTakeRatio *= Mathf.Clamp01(1f - reductionsRatio);
        }
    }
}