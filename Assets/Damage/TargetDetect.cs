using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay
{
    [Flags]
    public enum TargetFilter
    {
        None = 0,

        [LabelText("玩家")] Player = 1 << 0,

        [LabelText("怪物")] Monster = 1 << 1,

        All = ~None,
    }

    public interface IFilterTarget
    {
        TargetFilter Type { get; }
    }
}