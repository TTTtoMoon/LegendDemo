using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueGods
{
    public class DontDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
