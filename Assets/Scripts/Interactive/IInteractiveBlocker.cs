using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace KoboldTools
{
    public interface IInteractiveBlocker
    {
        bool blocked { get; }
    }
}
