namespace RogueGods.Gameplay
{
    public enum AttributeType
    {
        None = 0, // 无效值 
        MaxHealth = 1, // 生命值上限 
        Energy = 4, // 大招能量最大值 
        MoveSpeedCoefficientModifier = 8, // 移动速度修正系数 
        Attack = 101, // 攻击力 
        AttackMultiplierModifier = 102, // 攻击强度修正系数 
        AttackSpeedModifier = 103, // 攻击速度修正值 
        CriticalChance = 104, // 暴击率 
        CriticalPowerAddition = 105, // 暴击伤害加成 
        DodgeProbability = 201, // 闪避率(0~1) 
    }
}