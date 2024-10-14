using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace KoboldTools
{

    public interface IPanel
    {
        bool isTransitioning { get; }
        bool isOpen
        { get; }
        CanvasGroup canvasGroup
        { get; }
        UnityEvent open
        { get; }
        UnityEvent close
        { get; }
        UnityEvent openComplete
        { get; }
        UnityEvent closeComplete
        { get; }

        void onOpen();
        void onClose();
        void onOpenComplete();
        void onCloseComplete();

    }
}
