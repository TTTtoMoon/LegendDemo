namespace RogueGods.Gameplay
{
    public enum AttributeType
    {
        None = 0, // 无效值 
        MaxHealth = 1, // 生命值上限 
        RedHeartRecover = 2, // 红心恢复固定值加成 
        RedHeartRecoverCoefficientAddition = 3, // 红心恢复系数加成 
        Energy = 4, // 大招能量最大值 
        EnergyGainCoefficientAddition = 5, // 大招能量积攒系数加成 
        ExperienceGainRate = 6, // 经验获取率 
        GoldGainRate = 7, // 金币获取率 
        MoveSpeedCoefficientModifier = 8, // 移动速度修正系数 
        HealthRecoverWhenLevelUp = 9, // 升级时生命恢复 
        SelectSkillAtBegin = 10, // 初始技能选择次数 
        EquipmentAttributeMultiplier = 11, // 装备属性效果倍率加成 
        RoleAttributeMultiplier = 12, // 角色属性效果倍率加成 
        RedHeartDropAddition = 13, // 红心掉落概率加成 
        EnergyCostReduceRatio = 14, // 大招消耗减少 
        Attack = 101, // 攻击力 
        AttackMultiplierModifier = 102, // 攻击强度修正系数 
        AttackSpeedModifier = 103, // 攻击速度修正值 
        CriticalChance = 104, // 暴击率 
        CriticalPowerAddition = 105, // 暴击伤害加成 
        CrashDamageCoefficient = 106, // 碰撞伤害系数 
        DodgeProbability = 201, // 闪避率(0~1) 
        DefenseLevelOfHurt = 202, // 受击免疫等级 
        DefenseLevelOfRetreat = 203, // 击退免疫等级 
        DamageAdditionOfAll = 301, // 增伤固定值：全部 
        DamageAdditionOfGround = 302, // 增伤固定值：地面 
        DamageAdditionOfFly = 303, // 增伤固定值：飞行 
        DamageAdditionOfMeleeUnit = 304, // 增伤固定值：近战 
        DamageAdditionOfRangeUnit = 305, // 增伤固定值：远程 
        DamageAdditionOfMonster = 306, // 增伤固定值：小怪 
        DamageAdditionOfBoss = 307, // 增伤固定值：首领 
        DamageReductionsOfAll = 401, // 减伤固定值：全部 
        DamageReductionsOfStatic = 402, // 减伤固定值：静止 
        DamageReductionsOfForward = 403, // 减伤固定值：前方 
        DamageReductionsOfBackward = 404, // 减伤固定值：后方 
        DamageReductionsOfCrash = 405, // 减伤固定值：碰撞 
        DamageReductionsOfProjectile = 406, // 减伤固定值：投射 
        DamageReductionsOfMonster = 407, // 减伤固定值：小怪 
        DamageReductionsOfBoss = 408, // 减伤固定值：首领 
        DamageReductionsRatioOfAll = 501, // 减伤比率(0~1)：全部 
        DamageReductionsRatioOfStatic = 502, // 减伤比率(0~1)：静止 
        DamageReductionsRatioOfForward = 503, // 减伤比率(0~1)：前方 
        DamageReductionsRatioOfBackward = 504, // 减伤比率(0~1)：后方 
        DamageReductionsRatioOfCrash = 505, // 减伤比率(0~1)：碰撞 
        DamageReductionsRatioOfProjectile = 506, // 减伤比率(0~1)：投射 
        DamageReductionsRatioOfMonster = 507, // 减伤比率(0~1)：小怪 
        DamageReductionsRatioOfBoss = 508, // 减伤比率(0~1)：首领 
    }
}