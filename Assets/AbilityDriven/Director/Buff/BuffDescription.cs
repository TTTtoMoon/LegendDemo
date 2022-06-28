using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("Buff数据")]
    public class BuffDescription : AbilityComponent
    {
        [Description("叠加方式")]
        public BuffOverlayType OverlayType;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}