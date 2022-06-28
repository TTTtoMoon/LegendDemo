using System;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public readonly struct AnimationDefinition
    {
        public readonly struct State
        {
            public static readonly AnimationProperty Locomotion            = new AnimationProperty(nameof(Locomotion));
            public static readonly AnimationProperty Hurt                  = new AnimationProperty(nameof(Hurt));
            public static readonly AnimationProperty Shake                 = new AnimationProperty(nameof(Shake));
            public static readonly AnimationProperty Birth                 = new AnimationProperty(nameof(Birth));
            public static readonly AnimationProperty Death                 = new AnimationProperty(nameof(Death));
            public static readonly AnimationProperty Relive                = new AnimationProperty(nameof(Relive));
            public static readonly AnimationProperty Enter                 = new AnimationProperty(nameof(Enter));
            public static readonly AnimationProperty Exit                  = new AnimationProperty(nameof(Exit));
            public static readonly AnimationProperty ShowItem              = new AnimationProperty(nameof(ShowItem));
            public static readonly AnimationProperty Close                 = new AnimationProperty(nameof(Close));
            public static readonly AnimationProperty Inactive              = new AnimationProperty(nameof(Inactive));
            public static readonly AnimationProperty Active                = new AnimationProperty(nameof(Active));
            public static readonly AnimationProperty ActiveStandby         = new AnimationProperty(nameof(ActiveStandby));
            public static readonly AnimationProperty Finish                = new AnimationProperty(nameof(Finish));
            public static readonly AnimationProperty RouletteTrans         = new AnimationProperty("Trans");
            public static readonly AnimationProperty RouletteSelected = new AnimationProperty("EF_UI_RoulettePanel_Winglow_Anim");
            public static readonly AnimationProperty Up = new AnimationProperty("Up");
        }

        public readonly struct Parameter
        {
            public static readonly AnimationProperty MovementSpeed = new AnimationProperty(nameof(MovementSpeed));
            public static readonly AnimationProperty Disappear     = new AnimationProperty(nameof(Disappear));
        }

        public readonly struct Layer
        {
            public static readonly int BaseLayer     = 0;
            public static readonly int OverrideLayer = 1;
        }
    }

    [Serializable]
    public struct AnimationProperty : ISerializationCallbackReceiver
    {
        public AnimationProperty(string name)
        {
            m_Name = name;
            m_Hash = Animator.StringToHash(name);
        }

        [SerializeField] private string m_Name;
        [NonSerialized]  private int    m_Hash;

        public string Name => m_Name;
        public int    Hash => m_Hash;

        public bool IsName(string name)
        {
            return m_Name == name;
        }

        public static implicit operator AnimationProperty(string name)
        {
            return new AnimationProperty(name);
        }

        public static implicit operator int(AnimationProperty property)
        {
            return property.Hash;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            m_Hash = Animator.StringToHash(m_Name);
        }
    }
}
