namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 技能阶段
    /// </summary>
    public enum SkillPhase
    {
        /// <summary>
        /// 未使用技能
        /// </summary>
        NoSkill,

        /// <summary>
        /// 前摇节段
        /// </summary>
        Preparing,

        /// <summary>
        /// 效果阶段
        /// </summary>
        Acting,

        /// <summary>
        /// 后摇阶段
        /// </summary>
        Finishing,
    }
}