using Sirenix.OdinInspector;

namespace RogueGods.Gameplay
{
    public enum DamageMaterial
    {
        [LabelText("无")] None,
        
        [LabelText("钝器")] Blunt,
        
        [LabelText("利器")] Steel,
        
        [LabelText("击退")] Retreat,

        [LabelText("投射物")] Projectile,
    }
}