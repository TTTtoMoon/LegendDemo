using System.Collections.Generic;

namespace RogueGods.Utility
{
    public static class CfgTableDataChecker
    {
        public static bool IsIntValid(int value)
        {
            return value != -1;
        }

        public static bool IsIntListValid(List<int> value)
        {
            if (value == null)
                return false;
            
            if (value.Count == 0)
                return false;
            
            if (value.Count == 1 && value[0] == 0)
            {
                return false;
            }
            
            return true;
        }
    }
}
