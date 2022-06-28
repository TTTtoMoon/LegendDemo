using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using RogueGods.Gameplay;
using RogueGods.Gameplay.AbilityDriven;
using RogueGods.Gameplay.VFX;
using UnityEngine;

namespace RogueGods
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static DamageSystem     DamageSystem     => Instance.m_DamageSystem;
        public static AbilitySystem    AbilitySystem    => Instance.m_AbilitySystem;
        public static VFXSystem        VFXSystem        => Instance.m_VFXSystem;
        public static OrbSystem        OrbSystem        => Instance.m_OrbSystem;
        public static FlashChainSystem FlashChainSystem => Instance.m_FlashChainSystem;
        public static LineRenderSystem LineRenderSystem => Instance.m_LineRenderSystem;
        public static CameraSystem     CameraSystem     => Instance.m_CameraSystem;

        private readonly DamageSystem     m_DamageSystem     = new DamageSystem();
        private readonly AbilitySystem    m_AbilitySystem    = new AbilitySystem();
        private readonly VFXSystem        m_VFXSystem        = new VFXSystem();
        private readonly OrbSystem        m_OrbSystem        = new OrbSystem();
        private readonly FlashChainSystem m_FlashChainSystem = new FlashChainSystem();
        private readonly LineRenderSystem m_LineRenderSystem = new LineRenderSystem();
        private readonly CameraSystem     m_CameraSystem     = new CameraSystem();

        private GameSystem[] m_Systems;

        public enum FixedOrder
        {
            //默认
            Default = 0,
            Last    = int.MaxValue,
        }


        public enum UpdateMonoOrder
        {
            Pre = -1,

            //默认
            Default = 0,

            /// <summary>
            /// 更新transform
            /// </summary>
            UpdateTransform = 100,
            UpdateBloodPosition,
            Last = int.MaxValue,
        }

        public enum LateUpdateMonoOrder
        {
            //默认
            Default = 0,
            RotateActor,


            /// <summary>
            /// 后置更新transform，主要用于跟随等
            /// </summary>
            LateUpdateTransform = 10000,

            /// <summary>
            /// 更新能力
            /// </summary>
            UpdateAbility = 20000, //（很多数据需要等待transform更新完毕，所以放这么后面）

            /// <summary>
            /// 更新新手引导
            /// </summary>
            UpdateNewbieGuidance = 30000,

            Last = int.MaxValue,
        }

        public enum GizmoMonoOrder
        {
            //默认
            Default = 0,
            Last    = int.MaxValue,
        }

        private readonly LinkedList<MonoClass> _FixedUpdateList = new LinkedList<MonoClass>();
        private readonly LinkedList<MonoClass> _UpdateList      = new LinkedList<MonoClass>();
        private readonly LinkedList<MonoClass> _LateUpdateList  = new LinkedList<MonoClass>();
#if UNITY_EDITOR
        private readonly LinkedList<MonoClass> _GizmoList = new LinkedList<MonoClass>();
#endif

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            m_Systems = GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField)
                .Select(x => x.GetValue(this) as GameSystem)
                .ToArray();

            MiscConfiguration.PreLoad();
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].Awake();
            }
        }

        private void Start()
        {
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].Start();
            }
        }

        public void ClearAll()
        {
            _FixedUpdateList.Clear();
            _UpdateList.Clear();
            _LateUpdateList.Clear();
#if UNITY_EDITOR
            _GizmoList.Clear();
#endif
        }

        public void RegisterFixedUpdate(Action func, int frequency = 1, FixedOrder order = FixedOrder.Default)
        {
            AddToRealList(_FixedUpdateList, func, frequency, (int)order);
            //Debugger.Log(Color.blue, "当前FixedUpdate：", _FixedUpdateList.Count);
        }

        public void RegisterUpdate(Action func, int frequency = 1, UpdateMonoOrder order = UpdateMonoOrder.Default)
        {
            AddToRealList(_UpdateList, func, frequency, (int)order);
            //Debugger.Log(Color.blue, "当前Update：", _UpdateList.Count);
        }

        public void RegisterLateUpdate(Action func, int frequency = 1,
            LateUpdateMonoOrder               order = LateUpdateMonoOrder.Default)
        {
            AddToRealList(_LateUpdateList, func, frequency, (int)order);
            //Debugger.Log(Color.blue, "当前LateUpdate：", _LateUpdateList.Count);
        }


        [Conditional("UNITY_EDITOR")]
        public void RegisterGizmo(Action func, int frequency = 1,
            GizmoMonoOrder               order = GizmoMonoOrder.Default)
        {
#if UNITY_EDITOR
            AddToRealList(_GizmoList, func, frequency, (int) order);
            //Debugger.Log(Color.blue, "当前Gizmo：", _GizmoList.Count);
#endif
        }

        public void UnRegisterFixedUpdate(Action func)
        {
            RecursiveRemove(_FixedUpdateList.Last, func);
            //Debugger.Log(Color.blue, "当前FixedUpdate：", _FixedUpdateList.Count);
        }

        public void UnRegisterUpdate(Action func)
        {
            RecursiveRemove(_UpdateList.Last, func);
            // Debugger.Log(Color.blue, "当前Update：", _UpdateList.Count);
        }

        public void UnRegisterLateUpdate(Action func)
        {
            RecursiveRemove(_LateUpdateList.Last, func);
            //Debugger.Log(Color.blue, "当前LateUpdate：", _LateUpdateList.Count);
        }

        [Conditional("UNITY_EDITOR")]
        public void UnRegisterGizmo(Action func)
        {
#if UNITY_EDITOR
            RecursiveRemove(_GizmoList.Last, func);
            // Debugger.Log(Color.blue, "当前Gizmo：", _GizmoList.Count);
#endif
        }

        // Update is called once per frame
        private void Update()
        {
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].Update();
            }

            DealList(_UpdateList);
        }

        private void LateUpdate()
        {
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].LateUpdate();
            }

            DealList(_LateUpdateList);
        }

        private void FixedUpdate()
        {
            for (int i = 0, length = m_Systems.Length; i < length; i++)
            {
                m_Systems[i].FixedUpdate();
            }

            DealList(_FixedUpdateList);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DealList(_GizmoList);
        }
#endif

        private void DealList(LinkedList<MonoClass> realList)
        {
            LinkedListNode<MonoClass> nowLinkedNode = realList.First;
            while (nowLinkedNode != null)
            {
                LinkedListNode<MonoClass> next      = nowLinkedNode.Next;
                MonoClass                 monoClass = nowLinkedNode.Value;
                if (monoClass.Func == null)
                {
                    nowLinkedNode = next;
                    continue;
                }

                monoClass.NowFrequencyCount = monoClass.NowFrequencyCount - 1;
                if (monoClass.NowFrequencyCount <= 0)
                {
                    monoClass.Func();
                    monoClass.NowFrequencyCount = monoClass.Frequency;
                }

                nowLinkedNode.Value = monoClass;

                nowLinkedNode = next;
            }

            nowLinkedNode = realList.First;
            while (nowLinkedNode != null)
            {
                LinkedListNode<MonoClass> next = nowLinkedNode.Next;
                if (nowLinkedNode.Value.Func == null)
                {
                    realList.Remove(nowLinkedNode);
                }

                nowLinkedNode = next;
            }
        }

        private void RecursiveRemove(LinkedListNode<MonoClass> node, Action func)
        {
            if (node.Value.Func == func)
            {
                node.Value.Func = null;
            }
            else if (node.Previous != null)
            {
                RecursiveRemove(node.Previous, func);
            }
        }

        private void AddToRealList(LinkedList<MonoClass> list, Action func, int frequency, int order)
        {
            MonoClass temp = new MonoClass();
            temp.Func              = func;
            temp.Frequency         = frequency;
            temp.Order             = order;
            temp.NowFrequencyCount = frequency;


            LinkedListNode<MonoClass> nowLinkedNode = list.Last;
            while (true)
            {
                if (nowLinkedNode == null)
                {
                    list.AddFirst(temp);
                    break;
                }
                else if (temp.Order >= nowLinkedNode.Value.Order)
                {
                    list.AddAfter(nowLinkedNode, temp);
                    break;
                }
                else if (temp.Order < nowLinkedNode.Value.Order && (nowLinkedNode.Previous == null || temp.Order >= nowLinkedNode.Previous.Value.Order))
                {
                    list.AddBefore(nowLinkedNode, temp);
                    break;
                }

                nowLinkedNode = nowLinkedNode.Previous;
            }
        }

        public class MonoClass
        {
            public int    Frequency;
            public Action Func;

            public int NowFrequencyCount;

            /// <summary>
            ///     smaller first
            /// </summary>
            public int Order;
        }
    }
}