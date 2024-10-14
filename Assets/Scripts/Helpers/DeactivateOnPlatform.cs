using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoboldTools
{
    public class DeactivateOnPlatform : MonoBehaviour
    {
        public RuntimePlatform platform;

        void Awake()
        {
            if (Application.platform == platform)
                gameObject.SetActive(false);
        }
    }
}
