using RogueGods.Utility;
using UnityEngine;

namespace RogueGods
{
    /// <summary>
    ///     状态机基类
    /// </summary>
    public abstract class StateMachine<TOwner, TMachineSelf> : MonoBehaviour, IStateMachine where TMachineSelf : StateMachine<TOwner, TMachineSelf>
    {
        //当前状态
        public State<TOwner, TMachineSelf> CurrentState;

        //所属
        [HideInInspector] public TOwner Owner;

        protected abstract State<TOwner, TMachineSelf> GetDefaultState();

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            CurrentState?.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            CurrentState?.OnLateUpdate();
        }

        protected virtual void Init()
        {
        }

        protected virtual void CustomDestroy()
        {
        }

        public void SetOwner(object owner)
        {
            if (owner is TOwner o)
            {
                Owner = o;
            }
            else
            {
                Debugger.LogError("owner 类型不对");
            }

            Init();
            CurrentState = GetDefaultState();
            CurrentState.Enter(this as TMachineSelf);
        }

        public void SetOwner(TOwner owner)
        {
            Owner = owner;
            Init();
            CurrentState = GetDefaultState();
            CurrentState.Enter(this as TMachineSelf);
        }

        /// <summary>
        ///     手动跳转状态
        /// </summary>
        /// <param name="newState"></param>
        public virtual bool ChangeState<TState>(TState newState = null) where TState : State<TOwner, TMachineSelf>, new()
        {
            if (newState == null)
            {
                newState = ObjectPool<TState>.Get();
            }

            if (CurrentState != null)
            {
                CurrentState.Exit();
            }
            else
            {
                Debug.LogError("current state is null, fail to exit state!");
            }

            CurrentState = newState;

            newState.Enter(this as TMachineSelf);
            return true;
        }

        public void Destroy()
        {
            CurrentState.Exit();
            CustomDestroy();
            CurrentState = null;
        }
    }
}