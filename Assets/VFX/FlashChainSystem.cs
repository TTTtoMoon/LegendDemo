using System;
using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RogueGods.Gameplay.VFX
{
    public class FlashChainSystem : GameSystem
    {
        private List<FlashChain> _FlashChain = new List<FlashChain>();

        public void AddLightChain(GameObject chain, Vector3 startPosition, float range, float interval, float duration, int maxSpreadCount, Action<Actor> callBack)
        {
            FlashChain flashChain = chain.GetComponent<FlashChain>();
            flashChain = Object.Instantiate(flashChain);
            flashChain.SetInfo(startPosition, range, interval, duration, maxSpreadCount, callBack);
            _FlashChain.Add(flashChain);
        }

        public void RemoveFlashChain(FlashChain flashChain)
        {
            _FlashChain.Remove(flashChain);
            Object.DestroyImmediate(flashChain.gameObject);
        }
    }
}