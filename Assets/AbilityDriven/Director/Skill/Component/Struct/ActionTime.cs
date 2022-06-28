using System;
using Sirenix.OdinInspector;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Toggle("Enable")]
    public class ActionTime
    {
        public bool Enable;
        
        [LabelText("延时")]
        public float Delay;
    }
}