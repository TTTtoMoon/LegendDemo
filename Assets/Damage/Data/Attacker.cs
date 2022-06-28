namespace RogueGods.Gameplay
{
    /// <summary>
    /// 攻击者
    /// </summary>
    public struct Attacker
    {
        /// <summary>
        /// 创建初始值
        /// </summary>
        /// <returns></returns>
        public static Attacker Create()
        {
            return new Attacker()
            {
                Attack                    = 0f,
                AttackMultipleModifier    = 0f,
                DamageAddition            = 0f,
                WeaponPower               = 1f,
                DamagePower               = 0f,
                CriticalChance            = 0f,
                CriticalPowerAddition     = 0f,
            };
        }

        /// <summary>
        /// 攻击力（角色面板攻击力）
        /// </summary>
        public float Attack;

        /// <summary>
        /// 攻击力倍率修正值（局内技能攻击力加成、各种箭减少等）
        /// </summary>
        public float AttackMultipleModifier;

        /// <summary>
        /// 伤害数值加成（如戒指的克制伤害）
        /// </summary>
        public float DamageAddition;

        /// <summary>
        /// 武器倍率
        /// </summary>
        public float WeaponPower;

        /// <summary>
        /// 伤害额外加成（如愤怒）
        /// </summary>
        public float DamagePower;

        /// <summary>
        /// 暴击几率(0.0 ~ 1.0)
        /// </summary>
        public float CriticalChance;

        /// <summary>
        /// 暴击倍率加成
        /// </summary>
        public float CriticalPowerAddition;
    }
}