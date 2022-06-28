using System.Collections.Generic;

namespace RogueGods.Utility
{
    public static class ListPool<TItem>
    {
        public static List<TItem> Get()
        {
            return ObjectPool<List<TItem>>.Get();
        }

        public static void Release(List<TItem> list)
        {
            list.Clear();
            ObjectPool<List<TItem>>.Release(list);
        }

        public static void Dispose()
        {
            ObjectPool<List<TItem>>.Dispose();
        }
    }

    public static class HashSetPool<TItem>
    {
        public static HashSet<TItem> Get()
        {
            return ObjectPool<HashSet<TItem>>.Get();
        }

        public static void Release(HashSet<TItem> HashSet)
        {
            HashSet.Clear();
            ObjectPool<HashSet<TItem>>.Release(HashSet);
        }

        public static void Dispose()
        {
            ObjectPool<HashSet<TItem>>.Dispose();
        }
    }
}