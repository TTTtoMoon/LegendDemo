using System;
using System.Collections.Generic;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay
{
    public class SlotPoint : MonoBehaviour
    {
        [Serializable]
        private struct SlotPointInfo
        {
            public TransformSlot Slot;
            public Transform     Transform;
        }

        [SerializeField] [TableList] [LabelText("插槽")]
        private SlotPointInfo[] m_Slots = new SlotPointInfo[0];


        /// <summary>
        /// 获取插槽transform
        /// </summary>
        public Transform GetSlotTransform(TransformSlot slot)
        {
            for (int i = 0, length = m_Slots.Length; i < length; i++)
            {
                if (m_Slots[i].Slot == slot)
                {
                    return m_Slots[i].Transform;
                }
            }

            Debugger.LogWarning($"无法找到对应插槽{slot}，请检查prefab上的配置");
            return transform;
        }

        /// <summary>
        /// 获取插槽位置
        /// </summary>
        public Vector3 GetSlotPosition(TransformSlot slot)
        {
            Transform slotTransform = GetSlotTransform(slot);
            return slotTransform.position;
        }

        [Button("自动寻找点位")]
        public void Check()
        {
            Transform[] trans = transform.GetComponentsInChildren<Transform>();
            Set(TransformSlot.Root, transform);
            for (int i = 0; i < trans.Length; i++)
            {
                string name = trans[i].name.ToLower().Replace(" ","");
                switch (name)
                {
                    case "tip":
                        Set(TransformSlot.Tip, trans[i]);
                        break;
                    case "hurtpoint":
                        Set(TransformSlot.HitPosition, trans[i]);
                        break;
                    case "headpoint":
                        Set(TransformSlot.Head, trans[i]);
                        break;
                    case "bip001ltoe0":
                        Set(TransformSlot.LeftFoot, trans[i]);
                        break;
                    case "bip001rtoe0":
                        Set(TransformSlot.RightFoot, trans[i]);
                        break;
                    case "bip001lhand":
                        Set(TransformSlot.LeftHand,   trans[i]);
                        Set(TransformSlot.LeftWeapon, trans[i]);
                        break;
                    case "bip001rhand":
                        Set(TransformSlot.RightHand,   trans[i]);
                        Set(TransformSlot.RightWeapon, trans[i]);
                        break;
                    case "bip001spine1":
                        Set(TransformSlot.BackWeapon, trans[i]);
                        break;
                }
            }
        }

        private void Set(TransformSlot slot,Transform transform)
        {
            for (int i = 0; i < m_Slots.Length; i++)
            {
                if(m_Slots[i].Slot==slot)
                {
                    m_Slots[i].Transform=transform;
                    return;
                }
            }

            List<SlotPointInfo> slots = new List<SlotPointInfo>(m_Slots);

            SlotPointInfo slotInfo = new SlotPointInfo();
            slotInfo.Slot      = slot;
            slotInfo.Transform = transform;
            
            slots.Add(slotInfo);

            m_Slots = slots.ToArray();
        }
    }
}
