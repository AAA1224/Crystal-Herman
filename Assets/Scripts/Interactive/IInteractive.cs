using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace KoboldTools
{
    public interface IInteractive
    {
        UnityEvent interacted { get; }
        void onPointerDown();
        void onPointerUp();
    }
}
