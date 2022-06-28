namespace Abilities
{
    /// <summary>
    /// 能力持有者
    /// </summary>
    public interface IAbilityOwner : IAbilityTarget
    {
        AbilityDirector AbilityDirector { get; }
    }
}