using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace KoboldTools
{
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<int, int> { }
}
