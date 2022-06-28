using System;
using System.Collections.Generic;

namespace RogueGods.Utility
{
    public class PrefixTree<TValue>
    {
        #region Node

        public class Node
        {
            private string m_Name;
            private Node m_Parent;
            private LinkedList<Node> m_Children;

            public string Name => m_Name;
            public Node Parent => m_Parent;
            public TValue Value;

            public Node(string name, Node paren)
            {
                m_Name = name;
                m_Parent = paren;
                m_Children = new LinkedList<Node>();
            }

            public bool TryGetChild(string childName, out Node child)
            {
                for (LinkedListNode<Node> node = m_Children.First; node != null; node = node.Next)
                {
                    child = node.Value;
                    if (child.m_Name == childName)
                    {
                        return true;
                    }
                }

                child = null;
                return false;
            }

            public void AddChild(string childName, out Node child)
            {
                for (LinkedListNode<Node> node = m_Children.First; node != null; node = node.Next)
                {
                    child = node.Value;
                    if (child.m_Name == childName)
                    {
                        return;
                    }
                }

                child = new Node(childName, this);
                m_Children.AddLast(child);
            }

            public bool RemoveChild(string childName)
            {
                for (LinkedListNode<Node> node = m_Children.First; node != null; node = node.Next)
                {
                    if (node.Value.m_Name == childName)
                    {
                        m_Children.Remove(node);
                        return true;
                    }
                }

                return false;
            }

            public void ClearChildren()
            {
                m_Children.Clear();
            }
        }

        #endregion

        #region Tree

        private readonly Node m_Root;
        private readonly char m_SplitChar;

        private PrefixTree(string name, char splitChar = '/')
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            m_SplitChar = splitChar;
            string[] splitPath = name.Split(m_SplitChar);
            if (splitPath.Length > 1)
            {
                m_Root = new Node(splitPath[0], null);
                m_Root.AddChild(name, out _);
            }
            else
            {
                m_Root = new Node(name, null);
            }
        }

        public bool ContainsChild(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string[] splitPath = path.Split(m_SplitChar);
            Node node = m_Root;
            int startIndex = splitPath[0] == m_Root.Name ? 1 : 0;
            for (int i = startIndex, length = splitPath.Length; i < length; i++)
            {
                if (node.TryGetChild(splitPath[i], out node) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryGetChild(string path, out Node node)
        {
            if (string.IsNullOrEmpty(path))
            {
                node = null;
                return false;
            }

            string[] splitPath = path.Split(m_SplitChar);
            node = m_Root;
            int startIndex = splitPath[0] == m_Root.Name ? 1 : 0;
            for (int i = startIndex, length = splitPath.Length; i < length; i++)
            {
                if (node.TryGetChild(splitPath[i], out node) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AddChildNode(string path, out Node node)
        {
            if (string.IsNullOrEmpty(path))
            {
                node = null;
                return false;
            }

            string[] splitPath = path.Split(m_SplitChar);
            node = m_Root;
            int startIndex = splitPath[0] == m_Root.Name ? 1 : 0;
            for (int i = startIndex, length = splitPath.Length; i < length; i++)
            {
                node.AddChild(splitPath[i], out node);
            }

            return true;
        }

        public bool RemoveChildNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string[] splitPath = path.Split(m_SplitChar);
            Node node = m_Root;
            int startIndex = splitPath[0] == m_Root.Name ? 1 : 0;
            for (int i = startIndex, length = splitPath.Length - 1; i < length; i++)
            {
                if (node.TryGetChild(splitPath[i], out node) == false)
                {
                    return false;
                }
            }

            return node.RemoveChild(splitPath[splitPath.Length - 1]);
        }

        public void ClearChildren()
        {
            m_Root.ClearChildren();
        }

        #endregion
    }
}