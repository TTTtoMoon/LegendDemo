using System;

namespace RogueGods.Utility
{
    public class NiceDelegate : AbstractNiceDelegate<Action>
    {
        public void Invoke()
        {
            BeginInvoke();
            foreach (PriorityNode<Action> actionNode in m_Delegates)
            {
                Action action = actionNode.Item;
                if (action == null) continue;
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debugger.LogException(new NiceDelegateException(action.Target.GetType().Name, action.Method.Name, ex));
                }
            }

            EndInvoke();
        }

        public static NiceDelegate operator +(NiceDelegate niceDelegate, Action listener)
        {
            return AddListener(niceDelegate, listener);
        }

        public static NiceDelegate operator -(NiceDelegate niceDelegate, Action listener)
        {
            return RemoveListener(niceDelegate, listener);
        }
    }

    public class NiceDelegate<T0> : AbstractNiceDelegate<Action<T0>>
    {
        public void Invoke(T0 param0)
        {
            BeginInvoke();
            foreach (PriorityNode<Action<T0>> actionNode in m_Delegates)
            {
                Action<T0> action = actionNode.Item;
                if (action == null) continue;
                try
                {
                    action.Invoke(param0);
                }
                catch (Exception ex)
                {
                    Debugger.LogException(new NiceDelegateException(action.Target.GetType().Name, action.Method.Name, ex));
                }
            }

            EndInvoke();
        }

        public static NiceDelegate<T0> operator +(NiceDelegate<T0> niceDelegate, Action<T0> listener)
        {
            return AddListener(niceDelegate, listener);
        }

        public static NiceDelegate<T0> operator -(NiceDelegate<T0> niceDelegate, Action<T0> listener)
        {
            return RemoveListener(niceDelegate, listener);
        }
    }

    public class NiceDelegate<T0, T1> : AbstractNiceDelegate<Action<T0, T1>>
    {
        public void Invoke(T0 param0, T1 param1)
        {
            BeginInvoke();
            foreach (PriorityNode<Action<T0, T1>> actionNode in m_Delegates)
            {
                Action<T0, T1> action = actionNode.Item;
                if (action == null) continue;
                try
                {
                    action.Invoke(param0, param1);
                }
                catch (Exception ex)
                {
                    Debugger.LogException(new NiceDelegateException(action.Target.GetType().Name, action.Method.Name, ex));
                }
            }

            EndInvoke();
        }

        public static NiceDelegate<T0, T1> operator +(NiceDelegate<T0, T1> niceDelegate, Action<T0, T1> listener)
        {
            return AddListener(niceDelegate, listener);
        }

        public static NiceDelegate<T0, T1> operator -(NiceDelegate<T0, T1> niceDelegate, Action<T0, T1> listener)
        {
            return RemoveListener(niceDelegate, listener);
        }
    }

    public class NiceDelegate<T0, T1, T2> : AbstractNiceDelegate<Action<T0, T1, T2>>
    {
        public void Invoke(T0 param0, T1 param1, T2 param2)
        {
            BeginInvoke();
            foreach (PriorityNode<Action<T0, T1, T2>> actionNode in m_Delegates)
            {
                Action<T0, T1, T2> action = actionNode.Item;
                if (action == null) continue;
                try
                {
                    action.Invoke(param0, param1, param2);
                }
                catch (Exception ex)
                {
                    Debugger.LogException(new NiceDelegateException(action.Target.GetType().Name, action.Method.Name, ex));
                }
            }

            EndInvoke();
        }

        public static NiceDelegate<T0, T1, T2> operator +(NiceDelegate<T0, T1, T2> niceDelegate, Action<T0, T1, T2> listener)
        {
            return AddListener(niceDelegate, listener);
        }

        public static NiceDelegate<T0, T1, T2> operator -(NiceDelegate<T0, T1, T2> niceDelegate, Action<T0, T1, T2> listener)
        {
            return RemoveListener(niceDelegate, listener);
        }
    }
}