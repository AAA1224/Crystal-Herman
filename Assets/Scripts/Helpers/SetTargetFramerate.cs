using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KoboldTools
{

    public class SetTargetFramerate : MonoBehaviour
    {
        public int targetFrameRate = 30;
        void Start()
        {
            Application.targetFrameRate = targetFrameRate;
        }

    }
}
