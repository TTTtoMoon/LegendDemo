using System;

namespace RogueGods.Gameplay.LocalPlayer
{
    [Flags]
    public enum InputState
    {
        None = 0,

        Down = 1 << 0,
        Hold = 1 << 1,
        Up   = 1 << 2,

        All = ~0,
    }
}