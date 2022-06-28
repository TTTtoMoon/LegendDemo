using System;

namespace RogueGods.Utility
{
    public class NiceDelegateException : Exception
    {
        private string m_Message;

        public override string Message => ToString();

        public NiceDelegateException(string targetName, string methodName, Exception sourceException)
        {
            m_Message = $"执行委托[{targetName}.{methodName}]失败 => \n{sourceException}";
        }

        public override string ToString()
        {
            return m_Message;
        }
    }
}