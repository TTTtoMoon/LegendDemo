using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueGods.Gameplay.VFX
{
    public class ParticleSpeedChange : MonoBehaviour
    {
        public float Speed;
        private void Awake()
        {
            var particleSystems= transform.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                var main = particleSystems[i].main;
                main.simulationSpeed                    = Speed;
            }
        }
    }
}
