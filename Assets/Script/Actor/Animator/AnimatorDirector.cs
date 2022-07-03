using System;
using System.Collections.Generic;
using RogueGods.Gameplay.Animation;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public class AnimatorDirector : StateCallback.IListener
    {
        public AnimatorDirector(Animator animator)
        {
            m_Animator   = animator;
            m_LayerCount = animator.layerCount;
            Initialize();
        }

        private Dictionary<int, NiceDelegate> m_StateEnterCallbackMap = new Dictionary<int, NiceDelegate>();
        private Dictionary<int, NiceDelegate> m_StateExitCallbackMap  = new Dictionary<int, NiceDelegate>();
        private Dictionary<int, int>          m_CurrentState          = new Dictionary<int, int>();

        private int      m_LayerCount;
        private Animator m_Animator;
        public  Animator Animator => m_Animator;

        public void Initialize()
        {
            StateCallback.Initialize(m_Animator, this);
        }

        public bool Play(AnimationProperty state, int layer = -1, float normalizedTimeOffset = 0f)
        {
            if (CheckStateLayer(state, ref layer) == false)
            {
                //Debugger.LogWarning($"无法播放{m_Animator.runtimeAnimatorController.name}不存在状态的 => {state.Name}");
                return false;
            }

            int currentState = m_Animator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
            // 手动播动画时强行触发一次当前状态的退出事件，否则会因为事件延迟了一帧出现BUG
            if (m_StateExitCallbackMap.TryGetValue(currentState, out NiceDelegate exitCallback))
            {
                exitCallback.Invoke();
            }

            m_Animator.SetLayerWeight(layer, 1f);
            m_Animator.Play(state, layer, normalizedTimeOffset);
            // 手动触发进入事件，避免一帧的延迟
            if (m_StateEnterCallbackMap.TryGetValue(state, out NiceDelegate enterCallback))
            {
                enterCallback.Invoke();
            }

            m_CurrentState[layer] = state;
            return true;
        }

        public void SetSpeed(float speed)
        {
            Animator.speed = speed;
        }

        public void ResetSpeed()
        {
            Animator.speed = 1f;
        }

        public void SetFloat(int parameter, float value)
        {
            m_Animator.SetFloat(parameter, value);
        }

        public void EnableOverrideLayer()
        {
            if (m_LayerCount == 1) return;
            if (m_Animator.GetLayerWeight(AnimationDefinition.Layer.OverrideLayer) < 1f)
            {
                m_Animator.SetLayerWeight(AnimationDefinition.Layer.OverrideLayer, 1f);
            }
        }

        public void DisableOverrideLayer()
        {
            if (m_LayerCount == 1) return;
            if (m_Animator.GetLayerWeight(AnimationDefinition.Layer.OverrideLayer) > 0f)
            {
                m_Animator.SetLayerWeight(AnimationDefinition.Layer.OverrideLayer, 0f);
            }
        }

        public void AddStateEnterListener(int state, Action listener)
        {
            NiceDelegate callback;
            if (m_StateEnterCallbackMap.TryGetValue(state, out callback) == false)
            {
                callback = new NiceDelegate();
                m_StateEnterCallbackMap.Add(state, callback);
            }

            callback.AddListener(listener);
        }

        public void RemoveStateEnterListener(int state, Action listener)
        {
            if (m_StateEnterCallbackMap.TryGetValue(state, out NiceDelegate callback))
            {
                callback.RemoveListener(listener);
            }
        }

        public void AddStateExitListener(int state, Action listener)
        {
            NiceDelegate callback;
            if (m_StateExitCallbackMap.TryGetValue(state, out callback) == false)
            {
                callback = new NiceDelegate();
                m_StateExitCallbackMap.Add(state, callback);
            }

            callback.AddListener(listener);
        }

        public void RemoveStateExitListener(int state, Action listener)
        {
            if (m_StateExitCallbackMap.TryGetValue(state, out NiceDelegate callback))
            {
                callback.RemoveListener(listener);
            }
        }

        void StateCallback.IListener.OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 如果进入同一状态或已手动触发过进入事件，则不再触发进入事件
            int state = stateInfo.shortNameHash;
            if (m_CurrentState.TryGetValue(layerIndex, out int currentState) && currentState == state)
            {
                return;
            }

            m_CurrentState[layerIndex] = state;
            if (m_StateEnterCallbackMap.TryGetValue(state, out NiceDelegate callback))
            {
                callback.Invoke();
            }
        }

        void StateCallback.IListener.OnStateExit(AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 退出到同一状态或已手动触发退出事件，则不再不触发退出事件
            int state = stateInfo.shortNameHash;
            if (m_CurrentState.TryGetValue(layerIndex, out int currentState) && currentState != state ||
                m_Animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == state)
            {
                return;
            }

            if (m_StateExitCallbackMap.TryGetValue(stateInfo.shortNameHash, out NiceDelegate callback))
            {
                callback.Invoke();
            }
        }

        private bool CheckStateLayer(int state, ref int layer)
        {
            bool hasState = false;
            if (layer < 0)
            {
                for (int i = 0; i < m_LayerCount; i++)
                {
                    hasState |= m_Animator.HasState(i, state);
                    if (hasState)
                    {
                        layer = i;
                        break;
                    }
                }
            }
            else
            {
                hasState = m_Animator.HasState(layer, state);
            }

            return hasState;
        }

        public void Clear()
        {
        }
    }
}