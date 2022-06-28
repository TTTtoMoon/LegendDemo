using System;
using System.Collections.Generic;
using System.Reflection;

namespace RogueGods.Utility
{
    [Flags]
    public enum ECacheFlags
    {
        None = 0,
        Field = 1 << 0,
        Property = 1 << 1,
        Method = 1 << 2,
        All = ~0,
    }

    public static class CommonBindingFlags
    {
        public const BindingFlags All = (BindingFlags) ~0;
        public const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        public const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
    }

    public class TypeReflectionCache
    {
        private Type m_Type;
        private BindingFlags m_BindingFlags;

        public Type Type => m_Type;

        private ECacheFlags m_CacheFlags;
        private Dictionary<string, FieldInfo> m_Fields = new Dictionary<string, FieldInfo>();
        private Dictionary<string, PropertyInfo> m_Properties = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, MethodInfo> m_Methods = new Dictionary<string, MethodInfo>();

        public TypeReflectionCache(Type rType, BindingFlags sBindingFlags, ECacheFlags sCacheFlags = ECacheFlags.None)
        {
            m_Type = rType;
            m_BindingFlags = sBindingFlags;
            Cache(sCacheFlags);
        }

        public void Cache(ECacheFlags sCacheFlags)
        {
            if (sCacheFlags.HasFlag(ECacheFlags.Field) && m_CacheFlags.HasFlag(ECacheFlags.Field) == false)
            {
                FieldInfo[] array = m_Type.GetFields(m_BindingFlags);
                for (int i = 0, length = array.Length; i < length; i++)
                {
                    FieldInfo item = array[i];
                    m_Fields[item.Name] = item;
                }
            }

            if (sCacheFlags.HasFlag(ECacheFlags.Property) && m_CacheFlags.HasFlag(ECacheFlags.Property) == false)
            {
                PropertyInfo[] array = m_Type.GetProperties(m_BindingFlags);
                for (int i = 0, length = array.Length; i < length; i++)
                {
                    PropertyInfo item = array[i];
                    m_Properties[item.Name] = item;
                }
            }

            if (sCacheFlags.HasFlag(ECacheFlags.Method) && m_CacheFlags.HasFlag(ECacheFlags.Method) == false)
            {
                MethodInfo[] array = m_Type.GetMethods(m_BindingFlags);
                for (int i = 0, length = array.Length; i < length; i++)
                {
                    MethodInfo item = array[i];
                    m_Methods[item.Name] = item;
                }
            }

            m_CacheFlags |= sCacheFlags;
        }

        public bool TryGetField(string name, out FieldInfo result)
        {
            if (m_Fields.TryGetValue(name, out result))
            {
                return true;
            }

            if (m_CacheFlags.HasFlag(ECacheFlags.Field))
            {
                return false;
            }

            result = m_Type.GetField(name, m_BindingFlags);
            bool found = result != null;
            if (found)
            {
                m_Fields[name] = result;
            }

            return found;
        }

        public bool TryGetProperty(string name, out PropertyInfo result)
        {
            if (m_Properties.TryGetValue(name, out result))
            {
                return true;
            }

            if (m_CacheFlags.HasFlag(ECacheFlags.Property))
            {
                return false;
            }

            result = m_Type.GetProperty(name, m_BindingFlags);
            bool found = result != null;
            if (found)
            {
                m_Properties[name] = result;
            }

            return found;
        }

        public bool TryGetMethod(string name, out MethodInfo result)
        {
            if (m_Methods.TryGetValue(name, out result))
            {
                return true;
            }

            if (m_CacheFlags.HasFlag(ECacheFlags.Property))
            {
                return false;
            }

            result = m_Type.GetMethod(name, m_BindingFlags);
            bool found = result != null;
            if (found)
            {
                m_Methods[name] = result;
            }

            return found;
        }

        public void Clear()
        {
            m_CacheFlags = ECacheFlags.None;
            m_Fields.Clear();
            m_Properties.Clear();
            m_Methods.Clear();
        }
    }

    public class ManyTypesReflectionCache
    {
        private BindingFlags m_BindingFlags;
        private Dictionary<Type, TypeReflectionCache> m_CachedTypes = new Dictionary<Type, TypeReflectionCache>();

        public ManyTypesReflectionCache(BindingFlags sBindingFlags)
        {
            m_BindingFlags = sBindingFlags;
        }

        public bool TryGetField(Type rType, string rName, out FieldInfo rResult)
        {
            if (m_CachedTypes.TryGetValue(rType, out TypeReflectionCache cache) == false)
            {
                cache = new TypeReflectionCache(rType, m_BindingFlags);
                m_CachedTypes[rType] = cache;
            }

            return cache.TryGetField(rName, out rResult);
        }

        public bool TryGetProperty(Type rType, string rName, out PropertyInfo rResult)
        {
            if (m_CachedTypes.TryGetValue(rType, out TypeReflectionCache cache) == false)
            {
                cache = new TypeReflectionCache(rType, m_BindingFlags);
                m_CachedTypes[rType] = cache;
            }

            return cache.TryGetProperty(rName, out rResult);
        }

        public bool TryGetMethod(Type rType, string rName, out MethodInfo rResult)
        {
            if (m_CachedTypes.TryGetValue(rType, out TypeReflectionCache cache) == false)
            {
                cache = new TypeReflectionCache(rType, m_BindingFlags);
                m_CachedTypes[rType] = cache;
            }

            return cache.TryGetMethod(rName, out rResult);
        }

        public void Clear()
        {
            m_CachedTypes.Clear();
        }
    }
}