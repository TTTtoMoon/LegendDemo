using System;
using UnityEditor;

namespace RogueGods.Utility
{
    public abstract class AbstractNiceDelegate<TDelegate> where TDelegate : Delegate
    {
        protected readonly PriorityList<TDelegate> m_Delegates = new PriorityList<TDelegate>();
        protected readonly PriorityList<TDelegate> m_ToAdd     = new PriorityList<TDelegate>();

        private int m_EnumeratorCounter;

        public int Count => m_Delegates.Count;

        public void AddListener(TDelegate listener, int priority = 0)
        {
            if (listener == null || m_Delegates.Contains(listener)) return;
            if (m_EnumeratorCounter > 0)
            {
                m_ToAdd.Enqueue(listener, priority);
            }
            else
            {
                m_Delegates.Enqueue(listener, priority);
            }
        }

        public void RemoveListener(TDelegate listener)
        {
            if (listener == null) return;
            if (m_EnumeratorCounter > 0)
            {
                m_Delegates.ResetItem(listener, null);
            }
            else
            {
                m_Delegates.Remove(listener);
            }
        }

        public void Clear()
        {
            if (m_EnumeratorCounter > 0)
            {
                foreach (PriorityNode<TDelegate> node in m_Delegates)
                {
                    node.Item = null;
                }
            }
            else
            {
                m_Delegates.Clear();
            }
        }

        protected void BeginInvoke()
        {
            m_EnumeratorCounter++;
            foreach (PriorityNode<TDelegate> node in m_ToAdd)
            {
                m_Delegates.Enqueue(node.Item, node.Priority);
            }
            
            m_ToAdd.Clear();
        }

        protected void EndInvoke()
        {
            m_EnumeratorCounter--;
            if (m_EnumeratorCounter == 0) m_Delegates.RemoveAll((TDelegate)null);
        }

        protected static T AddListener<T>(T niceDelegate, TDelegate listener)
            where T : AbstractNiceDelegate<TDelegate>, new()
        {
            if (niceDelegate == null) niceDelegate = new T();
            niceDelegate.AddListener(listener);
            return niceDelegate;
        }

        protected static T RemoveListener<T>(T niceDelegate, TDelegate listener)
            where T : AbstractNiceDelegate<TDelegate>, new()
        {
            niceDelegate?.RemoveListener(listener);
            return niceDelegate;
        }
    }
}