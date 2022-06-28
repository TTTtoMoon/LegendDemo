using System;
using UnityEngine;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class PlayerInput
    {
        public static readonly int InputTypeCount = Enum.GetValues(typeof(InputType)).Length;

        private Vector3      m_Direction;
        private InputState[] m_InputStates = new InputState[InputTypeCount];

        public Vector3 Direction => m_Direction;

        public bool VerifyInput(InputType inputType, InputState inputState)
        {
            int index = (int)inputType;
            return (m_InputStates[index] & inputState) != InputState.None;
        }

        public void SetDirection(Vector2 direction)
        {
            m_Direction.x = direction.x;
            m_Direction.z = direction.y;
            m_Direction.Normalize();
        }

        public bool AnyInput()
        {
            for (int i = 0; i < InputTypeCount; i++)
            {
                if (m_InputStates[i] == InputState.Hold ||
                    m_InputStates[i] == InputState.Down)
                {
                    return true;
                }
            }

            return m_Direction != Vector3.zero;
        }

        public void EnqueueInput(InputType inputType, bool inputState)
        {
            int index = (int)inputType;
            m_InputStates[index] = inputState ? InputState.Down : InputState.Up;
        }

        public void ClearInput()
        {
            m_Direction = Vector3.zero;
            for (int i = 0; i < InputTypeCount; i++)
            {
                m_InputStates[i] = InputState.None;
            }
        }

        public void UpdateInputState()
        {
            for (int i = 0; i < InputTypeCount; i++)
            {
                switch (m_InputStates[i])
                {
                    case InputState.Down:
                        m_InputStates[i] = InputState.Hold;
                        break;
                    case InputState.Up:
                        m_InputStates[i] = InputState.None;
                        break;
                }
            }
        }
    }
}