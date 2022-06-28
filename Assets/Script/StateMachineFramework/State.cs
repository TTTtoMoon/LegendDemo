using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueGods
{
    /// <summary>
    ///     状态基类<Owner>
    /// </summary>
    public abstract class State<TOwner, TMachine>  where TMachine : StateMachine<TOwner, TMachine>
    {
        protected TMachine _Machine;
        protected TOwner   _Owner;

        public void Enter(TMachine machine)
        {
            _Machine = machine;
            _Owner   = _Machine.Owner;
            OnEnter();
        }

        protected virtual void OnEnter() { }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnExit() { }
    
        public virtual void OnUpdate() { }

        public virtual void OnLateUpdate() { }

        public virtual void OnFixedUpdate() { }
    }
}
