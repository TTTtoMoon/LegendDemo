#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public class DamageSampler : OdinMenuEditorWindow
    {
        [Serializable]
        private class Info
        {
            public string         Title;
            public DamageRequest  Request;
            public DamageResponse Response;
            public List<Modify>   Modifies = new List<Modify>();
        }

        private class Modify
        {
            public string    Description;
            public Attacker? Attacker;
            public Defender? Defender;
        }

        private class InfoDrawer : OdinValueDrawer<Info>
        {
            private Info     Info => Property.GetValue<Info>();
            private string[] m_ModifySelections;
            private int      m_ModifySelectIndex;

            protected override void Initialize()
            {
                base.Initialize();
                m_ModifySelections = new string[Info.Modifies.Count];
                for (int i = 0; i < Info.Modifies.Count; i++)
                {
                    m_ModifySelections[i] = ObjectNames.GetUniqueName(m_ModifySelections, Info.Modifies[i].Description);
                }

                m_ModifySelectIndex = 0;
            }

            protected override void DrawPropertyLayout(GUIContent label)
            {
                EditorGUILayout.BeginHorizontal();
                Boxing(Info.Request);
                Boxing(Info.Response);
                EditorGUILayout.EndHorizontal();

                if (m_ModifySelections.Length == 0) return;
                m_ModifySelectIndex = EditorGUILayout.Popup("Modifies", m_ModifySelectIndex, m_ModifySelections);

                Attacker attacker = Attacker.Create();
                Defender defender = Defender.Create();
                for (int i = 0; i < Info.Modifies.Count && i <= m_ModifySelectIndex; i++)
                {
                    if (Info.Modifies[i].Attacker != null)
                    {
                        attacker = Info.Modifies[i].Attacker.Value;
                    }

                    if (Info.Modifies[i].Defender != null)
                    {
                        defender = Info.Modifies[i].Defender.Value;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                Boxing(attacker);
                Boxing(defender);
                EditorGUILayout.EndHorizontal();
            }

            private void Boxing<T>(in T target)
            {
                SirenixEditorGUI.BeginBox(typeof(T).Name);
                FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++)
                {
                    EditorGUILayout.LabelField(fields[i].Name, fields[i].GetValue(target)?.ToString());
                }

                PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    EditorGUILayout.LabelField(properties[i].Name, properties[i].GetValue(target).ToString());
                }

                SirenixEditorGUI.EndBox();
            }
        }

        private static List<Info>  m_Infos    = new List<Info>();
        private static Stack<Info> m_Sampling = new Stack<Info>();

        public static void BeginSample(DamageRequest request)
        {
            m_Sampling.Push(new Info()
            {
                Request = request,
            });
        }

        public static void EndSample(DamageResponse response)
        {
            Info info = m_Sampling.Pop();
            info.Response = response;
            string maker = $"{GetName(info.Request.DamageMaker)}[{info.Request.DamageMaker.GetHashCode()}]";
            string taker = $"{GetName(info.Request.DamageTaker)}[{info.Request.DamageTaker.GetHashCode()}]";
            info.Title = $"[{DateTime.Now:hh:mm:ss} (第{Time.frameCount}帧)] 造成{response.Damage}点伤害 {maker}=>{taker}";
            m_Infos.Add(info);

            string GetName(object obj)
            {
                if (obj is MonoBehaviour monoBehaviour)
                {
                    return monoBehaviour.gameObject.name;
                }

                return obj.GetType().Name;
            }
        }

        public static void PushModify(string description, in Attacker attacker)
        {
            Info info = m_Sampling.Peek();
            if (info == null) return;
            info.Modifies.Add(new Modify()
            {
                Description = description,
                Attacker    = attacker,
            });
        }

        public static void PushModify(string description, in Defender defender)
        {
            Info info = m_Sampling.Peek();
            if (info == null) return;
            info.Modifies.Add(new Modify()
            {
                Description = description,
                Defender    = defender,
            });
        }

        [MenuItem("Tools/Damage Sampler")]
        static void OpenSamplerWindow()
        {
            GetWindow<DamageSampler>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            DrawMenuSearchBar = true;
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();
            for (int i = 0; i < m_Infos.Count; i++)
            {
                tree.Add(m_Infos[i].Title, m_Infos[i]);
            }

            return tree;
        }
    }
}
#endif