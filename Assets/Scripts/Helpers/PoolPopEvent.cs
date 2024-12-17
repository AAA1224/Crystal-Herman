using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class PoolPopEvent : MonoBehaviour
    {
        public UnityEvent popEvent = new UnityEvent();
        public void onPop()
        {
            popEvent.Invoke();
        }
    }
}
