using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KoboldTools.Logging;

namespace KoboldTools
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Panel : MonoBehaviour, IPanel
    {
        [SerializeField]
        private UnityEvent _close = new UnityEvent();
        [SerializeField]
        private UnityEvent _closeComplete = new UnityEvent();
        [SerializeField]
        private UnityEvent _open = new UnityEvent();
        [SerializeField]
        private UnityEvent _openComplete = new UnityEvent();
        [SerializeField]
        private bool _isOpen;
        private bool _isTransitioning = false;
        private CanvasGroup _canvasGroup = null;

        public void Awake()
        {
            this._canvasGroup = GetComponent<CanvasGroup>();
        }

        public UnityEvent close
        {
            get
            {
                return this._close;
            }
        }

        public UnityEvent closeComplete
        {
            get
            {
                return this._closeComplete;
            }
        }

        public UnityEvent open
        {
            get
            {
                return this._open;
            }
        }

        public UnityEvent openComplete
        {
            get
            {
                return this._openComplete;
            }
        }

        public CanvasGroup canvasGroup
        {
            get
            {
                return this._canvasGroup;
            }
        }

        public bool isOpen
        {
            get
            {
                return this._isOpen;
            }
        }

        public bool isTransitioning
        {
            get
            {
                return _isTransitioning;
            }
        }

        [ContextMenu("Close")]
        public void onClose()
        {
            if (!this.isOpen)
            {
                return;
            }
            RootLogger.Debug(this, "Closing the panel");
            this._isTransitioning = true;
            this._close.Invoke();
            this._isOpen = false;
        }

        public void onCloseComplete()
        {
            RootLogger.Debug(this, "Closing the panel is complete");
            this._isTransitioning = false;
            this._closeComplete.Invoke();
        }

        [ContextMenu("Open")]
        public void onOpen()
        {

            if (this.isOpen)
            {
                return;
            }
            RootLogger.Debug(this, "Opening the panel");
            this._isTransitioning = true;
            this._open.Invoke();
            this._isOpen = true;
        }

        public void onOpenComplete()
        {
            RootLogger.Debug(this, "Opening the panel is complete");
            this._isTransitioning = false;
            this._openComplete.Invoke();
        }
    }
}
