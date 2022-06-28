using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// Buff
    /// </summary>
    public class BuffDescriptor : IAbilityDescriptor
    {
        public BuffDescriptor(int abilityID, IAbilityVariableTable variableTable = null)
        {
            AbilityID     = abilityID;
            VariableTable = variableTable;
        }

        public int                   AbilityID     { get; }
        public IAbilityVariableTable VariableTable { get; }
        public Ability               Ability       { get; set; }
        public BuffOverlayType       OverlayType   => Ability != null && Ability.TryGetComponent(out BuffDescription buffDescription) ? buffDescription.OverlayType : BuffOverlayType.DontOverlay;
    }
}