using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("静态模式(不动)")]
    public class StaticMode : OrbMode
    {
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}