using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    public class CommonAbilityDescriptor : IAbilityDescriptor
    {
        public static CommonAbilityDescriptor CreateOrbAbility(int id, IAbilityVariableTable variableTable)
        {
            return new CommonAbilityDescriptor(id, variableTable);
        }

        private CommonAbilityDescriptor(int abilityID, IAbilityVariableTable variableTable)
        {
            AbilityID     = abilityID;
            VariableTable = variableTable;
        }

        public int                   AbilityID     { get; }
        public IAbilityVariableTable VariableTable { get; }
    }
}