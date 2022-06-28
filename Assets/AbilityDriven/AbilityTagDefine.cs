using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    [AbilityTagDefine]
    public static class AbilityTagDefine
    {
        [Description("碰撞")] 
        public static readonly AbilityTag Crash = new AbilityTag(1 << 0);

        [Description("子弹(投射物)")] 
        public static readonly AbilityTag Projectile = new AbilityTag(1 << 1);

        [Description("Dot伤")] 
        public static readonly AbilityTag Dot = new AbilityTag(1 << 2);

        [Description("主动技能")]
        public static readonly AbilityTag InitiativeSkill = new AbilityTag(1 << 3);

        [Description("火箭弹")]
        public static readonly AbilityTag RocketProjectile = new AbilityTag(1 << 4);

        [Description("普通子弹(可以被盾挡)")]
        public static readonly AbilityTag NormalBullet = new AbilityTag(1 << 5);
    }
}