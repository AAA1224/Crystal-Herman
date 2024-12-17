using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace KoboldTools
{
    public class InteractiveEvent : MonoBehaviour, IInteractive
    {
        [SerializeField]
        private bool _eventDisabled = false;
        public bool eventDisabled
        { get { return _eventDisabled; } set { _eventDisabled = value; } }

        public UnityEvent interacted
        {
            get
            {
                return interaction;
            }
        }

        public Interactions.InteractionType interactionType = Interactions.InteractionType.POINTERUP;
        public UnityEvent interaction = new UnityEvent();

        public void onPointerDown()
        {
            if (_eventDisabled || interactionType != Interactions.InteractionType.POINTERDOWN)
                return;

            interaction.Invoke();
        }

        public void onPointerUp()
        {
            if (_eventDisabled || interactionType != Interactions.InteractionType.POINTERUP)
                return;

            interaction.Invoke();
        }
    }
}
