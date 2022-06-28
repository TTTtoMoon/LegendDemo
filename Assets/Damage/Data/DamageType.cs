using Sirenix.OdinInspector;

namespace RogueGods.Gameplay
{
    public enum DamageType
    {
        [LabelText("非法伤害(逻辑强杀)")] None = 0,

        [LabelText("普通攻击")] NormalAttack = 1,

        [LabelText("技能")] SpecialAttack = 2,

        [LabelText("降临")] Godlike = 3,

        [LabelText("冲刺普攻")] DashNormalAttack = 4,
        
        [LabelText("极限冲刺")] UltimateDodge = 5,

        [LabelText("陷阱")] Trap = 6,

        [LabelText("反射")] Reflect = 7,

        [LabelText("击退")] Retreat = 8,

        [LabelText("祝福")] GodWish = 9,
    }
}