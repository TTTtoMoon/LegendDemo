using UnityEngine;

namespace RogueGods.Gameplay.Animation
{
    public class StateCallback : StateMachineBehaviour
    {
        public interface IListener
        {
            void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex);
            void OnStateExit(AnimatorStateInfo  stateInfo, int layerIndex);
        }

        private IListener m_Listener;

        public static void Initialize(Animator animator, IListener listener)
        {
            StateCallback[] callbacks = animator.GetBehaviours<StateCallback>();
            for (int i = 0, length = callbacks.Length; i < length; i++)
            {
                callbacks[i].m_Listener = listener;
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            m_Listener?.OnStateEnter(stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            m_Listener?.OnStateExit(stateInfo, layerIndex);
        }
    }
}