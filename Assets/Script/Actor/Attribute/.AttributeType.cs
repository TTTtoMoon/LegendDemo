using UnityEngine;

namespace RogueGods.Gameplay
{
    public enum AttributeType
    {
        [InspectorName("生命值上限")]      Health                             = 1,
        [InspectorName("红心恢复固定值加成")]  RedHeartRecover                    = 2,
        [InspectorName("红心恢复系数加成")]   RedHeartRecoverCoefficientAddition = 3,
        [InspectorName("大招能量最大值")]    Energy                             = 4,
        [InspectorName("大招能量积攒系数加成")] EnergyGainCoefficientAddition      = 5,
        [InspectorName("经验获取率")]      ExperienceGainRate                 = 6,
        [InspectorName("金币获取率")]      GoldGainRate                       = 7,
        [InspectorName("移动速度修正系数")]   MoveSpeedCoefficientModifier       = 8,

        [InspectorName("攻击力")]      Attack                   = 101,
        [InspectorName("攻击强度修正系数")] AttackMultiplierModifier = 102,
        [InspectorName("攻击速度修正值")]  AttackSpeedModifier      = 103,
        [InspectorName("暴击率")]      CriticalChance           = 104,
        [InspectorName("暴击伤害加成")]   CriticalPowerAddition    = 105,
        [InspectorName("碰撞伤害系数")]   CrashDamageCoefficient   = 106,

        [InspectorName("闪避率(0~1)")] DodgeProbability      = 201,
        [InspectorName("受击免疫等级")]   DefenseLevelOfHurt    = 202,
        [InspectorName("击退免疫等级")]   DefenseLevelOfRetreat = 203,

        [InspectorName("增伤固定值：全部")] DamageAdditionOfAll       = 301,
        [InspectorName("增伤固定值：地面")] DamageAdditionOfGround    = 302,
        [InspectorName("增伤固定值：飞行")] DamageAdditionOfFly       = 303,
        [InspectorName("增伤固定值：近战")] DamageAdditionOfMeleeUnit = 304,
        [InspectorName("增伤固定值：远程")] DamageAdditionOfRangeUnit = 305,
        [InspectorName("增伤固定值：小怪")] DamageAdditionOfMonster   = 306,
        [InspectorName("增伤固定值：首领")] DamageAdditionOfBoss      = 307,

        [InspectorName("减伤固定值：全部")] DamageReductionsOfAll        = 401,
        [InspectorName("减伤固定值：静止")] DamageReductionsOfStatic     = 402,
        [InspectorName("减伤固定值：前方")] DamageReductionsOfForward    = 403,
        [InspectorName("减伤固定值：后方")] DamageReductionsOfBackward   = 404,
        [InspectorName("减伤固定值：碰撞")] DamageReductionsOfCrash      = 405,
        [InspectorName("减伤固定值：投射")] DamageReductionsOfProjectile = 406,

        [InspectorName("减伤比率(0~1)：全部")] DamageReductionsRatioOfAll        = 501,
        [InspectorName("减伤比率(0~1)：静止")] DamageReductionsRatioOfStatic     = 502,
        [InspectorName("减伤比率(0~1)：前方")] DamageReductionsRatioOfForward    = 503,
        [InspectorName("减伤比率(0~1)：后方")] DamageReductionsRatioOfBackward   = 504,
        [InspectorName("减伤比率(0~1)：碰撞")] DamageReductionsRatioOfCrash      = 505,
        [InspectorName("减伤比率(0~1)：投射")] DamageReductionsRatioOfProjectile = 506,
    }
}