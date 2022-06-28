using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public class PriorityNode<TItem>
    {
        public PriorityNode(TItem item, int priority)
        {
            Item     = item;
            Priority = priority;
        }

        public TItem Item;
        public int   Priority;
    }

    public class PriorityList<TItem> : IEnumerable<PriorityNode<TItem>>
    {
        #region Cache

        private static LinkedList<PriorityNode<TItem>> Cache = new LinkedList<PriorityNode<TItem>>();

        private static LinkedListNode<PriorityNode<TItem>> GetNode(TItem item, int priority)
        {
            LinkedListNode<PriorityNode<TItem>> node      = Cache.Last;
            PriorityNode<TItem>                 nodeValue = new PriorityNode<TItem>(item, priority);
            if (node != null)
            {
                Cache.RemoveLast();
                node.Value = nodeValue;
            }
            else
            {
                node = new LinkedListNode<PriorityNode<TItem>>(nodeValue);
            }

            return node;
        }

        private static void ReleaseNode(LinkedListNode<PriorityNode<TItem>> node)
        {
            node.Value = default;
            Cache.AddLast(node);
        }

        #endregion

        private readonly LinkedList<PriorityNode<TItem>> m_List = new LinkedList<PriorityNode<TItem>>();

        public int Count => m_List.Count;

        public LinkedListNode<PriorityNode<TItem>> First => m_List.First;

        public LinkedListNode<PriorityNode<TItem>> Last => m_List.Last;

        public void Enqueue(TItem item, int priority)
        {
            LinkedListNode<PriorityNode<TItem>> node    = m_List.Last;
            LinkedListNode<PriorityNode<TItem>> newNode = GetNode(item, priority);
            if (node == null)
            {
                m_List.AddFirst(newNode);
                return;
            }

            while (node != null && node.Value.Priority > priority)
            {
                node = node.Previous;
            }

            if (node != null) m_List.AddAfter(node, newNode);
            else m_List.AddFirst(newNode);
        }

        public TItem Dequeue()
        {
            LinkedListNode<PriorityNode<TItem>> node = m_List.First;
            if (node != null)
            {
                TItem value = node.Value.Item;
                m_List.RemoveFirst();
                ReleaseNode(node);
                return value;
            }

            return default;
        }

        public TItem Peek()
        {
            LinkedListNode<PriorityNode<TItem>> node = m_List.First;
            return node != null ? node.Value.Item : default;
        }

        public bool Contains(TItem item)
        {
            LinkedListNode<PriorityNode<TItem>> node     = m_List.First;
            EqualityComparer<TItem>             equality = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (equality.Equals(priorityNode.Item, item))
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public void ResetItem(TItem oldItem, TItem newItem)
        {
            LinkedListNode<PriorityNode<TItem>> node     = m_List.First;
            EqualityComparer<TItem>             equality = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (equality.Equals(priorityNode.Item, oldItem))
                {
                    node.Value = new PriorityNode<TItem>(newItem, priorityNode.Priority);
                }

                node = node.Next;
            }
        }

        public bool Remove(TItem item, int priority)
        {
            LinkedListNode<PriorityNode<TItem>> node     = m_List.First;
            EqualityComparer<TItem>             equality = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (priorityNode.Priority == priority && equality.Equals(priorityNode.Item, item))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public bool Remove(TItem item)
        {
            LinkedListNode<PriorityNode<TItem>> node     = m_List.First;
            EqualityComparer<TItem>             equality = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (equality.Equals(priorityNode.Item, item))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public void Remove(LinkedListNode<PriorityNode<TItem>> node)
        {
            m_List.Remove(node);
        }

        public bool RemoveAll(Predicate<PriorityNode<TItem>> match)
        {
            bool                                removeAny = false;
            LinkedListNode<PriorityNode<TItem>> node      = m_List.First;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (match.Invoke(priorityNode))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    removeAny = true;
                }

                node = node.Next;
            }

            return removeAny;
        }

        public bool RemoveAll(Predicate<TItem> match)
        {
            bool                                removeAny = false;
            LinkedListNode<PriorityNode<TItem>> node      = m_List.First;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (match.Invoke(priorityNode.Item))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    removeAny = true;
                }

                node = node.Next;
            }

            return removeAny;
        }

        public bool RemoveAll(TItem item, int priority)
        {
            bool                                removeAny = false;
            LinkedListNode<PriorityNode<TItem>> node      = m_List.First;
            EqualityComparer<TItem>             equality  = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (priorityNode.Priority == priority && equality.Equals(priorityNode.Item, item))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    removeAny = true;
                }

                node = node.Next;
            }

            return removeAny;
        }

        public bool RemoveAll(TItem item)
        {
            bool                                removeAny = false;
            LinkedListNode<PriorityNode<TItem>> node      = m_List.First;
            EqualityComparer<TItem>             equality  = EqualityComparer<TItem>.Default;
            while (node != null)
            {
                PriorityNode<TItem> priorityNode = node.Value;
                if (equality.Equals(priorityNode.Item, item))
                {
                    m_List.Remove(node);
                    ReleaseNode(node);
                    removeAny = true;
                }

                node = node.Next;
            }

            return removeAny;
        }

        public void Clear()
        {
            LinkedListNode<PriorityNode<TItem>> node = m_List.First;
            while (node != null)
            {
                LinkedListNode<PriorityNode<TItem>> next = node.Next;
                m_List.Remove(node);
                ReleaseNode(node);
                node = next;
            }
        }

        public LinkedList<PriorityNode<TItem>>.Enumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator<PriorityNode<TItem>> IEnumerable<PriorityNode<TItem>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}