using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    public class SkillDescriptor : IAbilityDescriptor
    {
        public SkillDescriptor(int abilityID) : this(abilityID, null, GetSkillVariableTable(abilityID))
        {
        }

        public SkillDescriptor(int abilityID, Ability giver, IAbilityVariableTable variableTable)
        {
            AbilityID     = abilityID;
            VariableTable = variableTable;
            Giver         = giver;
        }

        public int                   AbilityID     { get; }
        public IAbilityVariableTable VariableTable { get; }
        public Ability               Giver         { get; }
        public Ability               Ability       { get; set; }
        public Vector2               ActingTime    => Ability != null && Ability.TryGetComponent(out SkillDescription skillDescription) ? skillDescription.ActingTime : Vector2.zero;

        /// <summary>
        /// 当前阶段
        /// </summary>
        public SkillPhase CurrentPhase
        {
            get
            {
                if (Ability == null || Ability.IsActive == false)
                {
                    return SkillPhase.NoSkill;
                }

                if (Ability.TryGetComponent(out SkillDescription skillDescription))
                {
                    if (Ability.Time < skillDescription.ActingTime.x)
                    {
                        return SkillPhase.Preparing;
                    }

                    if (Ability.Time < skillDescription.ActingTime.y)
                    {
                        return SkillPhase.Acting;
                    }

                    return SkillPhase.Finishing;
                }

                Debugger.LogError($"技能{AbilityID}未配置{nameof(SkillDescription)}，因此无法识别当前技能阶段。");
                return SkillPhase.Acting;
            }
        }

        /// <summary>
        /// 读表填充数据表
        /// </summary>
        private static IAbilityVariableTable GetSkillVariableTable(int skillID)
        {
            AbilityVariableTable table = new AbilityVariableTable();
            return table;
        }
    }
}