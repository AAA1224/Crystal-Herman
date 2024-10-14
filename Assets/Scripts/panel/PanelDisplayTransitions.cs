using UnityEngine;
using System.Collections;
using KoboldTools.Logging;

namespace KoboldTools
{

    public class PanelDisplayTransitions : ViewController<IPanel>
    {

        private bool _cacheInteracteable;
        private bool _cacheBlocksRaycasts;

        public EasingType closeTransition = EasingType.Linear;
        public EasingType openTransition = EasingType.Linear;
        [Tooltip("Factor of parent height or width in which the panel is transitioning.")]
        public Vector2 direction = Vector2.up;
        [Tooltip("Duration of the transition in seconds.")]
        public float openDuration = 0.7f;
        [Tooltip("Duration of the transition in seconds.")]
        public float closeDuration = 0.2f;
        [Tooltip("Delay of the transition in seconds.")]
        public float closeDelay = 0.0f;
        [Tooltip("Should the panel fade in and out in addition to the transition?")]
        public bool fade = false;
        [Tooltip("Should the panel scale in and out in addition to the transition?")]
        public bool scale = false;

        private Vector2 _openPosition = Vector2.zero;
        private Vector2 _closedPosition = Vector2.zero;
        private Vector2 _startPosition = Vector2.zero;
        private Vector3 _startScale = Vector3.zero;
        private float _progres = 0f;
        private bool _opening = false;
        private Coroutine _currentCoroutine = null;

        private RectTransform _rectTransform;

        private void Awake()
        {

            if (model == null)
                return;

            //set default
            if (model.isOpen)
            {
                openCompleted();
            }
            else
            {
                closeCompleted();
            }
        }

        public override void onModelChanged()
        {
            model.open.AddListener(opened);
            model.close.AddListener(closed);
            model.closeComplete.AddListener(closeCompleted);
            model.openComplete.AddListener(openCompleted);

            _cacheInteracteable = model.canvasGroup.interactable;
            _cacheBlocksRaycasts = model.canvasGroup.blocksRaycasts;

            _rectTransform = ((RectTransform)model.canvasGroup.transform);
            RectTransform parentRectTransform = ((RectTransform)model.canvasGroup.transform.parent);

            _openPosition = _rectTransform.anchoredPosition;
            _closedPosition = _rectTransform.anchoredPosition + new Vector2(parentRectTransform.rect.size.x * direction.x, parentRectTransform.rect.size.y * direction.y);

            //set default
            if (model.isOpen)
            {
                openCompleted();
            }
            else
            {
                closeCompleted();
            }
        }

        public override void onModelRemoved()
        {
            model.open.RemoveListener(opened);
            model.close.RemoveListener(closed);
            model.closeComplete.RemoveListener(closeCompleted);
            model.openComplete.RemoveListener(openCompleted);

            StopCoroutine(animationRoutine());

            //set default
            if (model.isOpen)
            {
                openCompleted();
            }
            else
            {
                closeCompleted();
            }
        }

        private IEnumerator animationRoutine()
        {
            RootLogger.Debug(this, "Starting panel animation");

            if (!_opening && closeDelay > 0f)
                yield return new WaitForSeconds(closeDelay); //close delay

            while (_progres < 1f)
            {
                _progres += Time.deltaTime / (_opening ? openDuration : closeDuration);
                //positioning
                if (_openPosition != _closedPosition)
                {
                    _rectTransform.anchoredPosition = _startPosition + (((_opening ? _openPosition : _closedPosition) - _startPosition) * Easing.ease(_opening ? openTransition : closeTransition, _progres));
                }
                //scaling
                if (scale)
                {
                    _rectTransform.localScale = _startScale + (((_opening ? Vector3.one : Vector3.zero) - _startScale) * Mathf.Max(Easing.ease(_opening ? openTransition : closeTransition, _progres), 0f));
                }
                //transparency
                if (fade)
                {
                    if (_opening)
                    {
                        model.canvasGroup.alpha = _progres;
                    }
                    else
                    {
                        model.canvasGroup.alpha = 1f - _progres;
                    }
                }
                yield return null;
            }

            RootLogger.Debug(this, "Panel animation complete");
            _progres = 0f;
            if (_opening)
            {
                model.onOpenComplete();
            }
            else
            {
                model.onCloseComplete();
            }

        }

        private void opened()
        {
            _opening = true;
            model.canvasGroup.alpha = fade ? 0f : 1f;
            model.canvasGroup.interactable = _cacheInteracteable;
            model.canvasGroup.blocksRaycasts = _cacheBlocksRaycasts;

            _startPosition = _rectTransform.anchoredPosition;
            _startScale = _rectTransform.localScale;

            if (gameObject.activeInHierarchy)
            {
                if (this._currentCoroutine != null) {
                    StopCoroutine(this._currentCoroutine);
                    this._currentCoroutine = null;
                }
                this._currentCoroutine = StartCoroutine(animationRoutine());
            }
            else
            {
                openCompleted();
            }
        }

        private void closed()
        {
            _opening = false;
            _startPosition = _rectTransform.anchoredPosition;
            _startScale = _rectTransform.localScale;

            if (gameObject.activeInHierarchy)
            {
                if (this._currentCoroutine != null) {
                    StopCoroutine(this._currentCoroutine);
                    this._currentCoroutine = null;
                }
                this._currentCoroutine = StartCoroutine(animationRoutine());
            }
            else
            {
                closeCompleted();
            }
        }

        private void closeCompleted()
        {
            _rectTransform.anchoredPosition = _closedPosition;
            if (scale)
                _rectTransform.localScale = Vector3.zero;
            model.canvasGroup.alpha = 0f;
            model.canvasGroup.interactable = false;
            model.canvasGroup.blocksRaycasts = false;
        }

        private void openCompleted()
        {
            _rectTransform.anchoredPosition = _openPosition;
            if (scale)
                _rectTransform.localScale = Vector3.one;
            model.canvasGroup.alpha = 1f;
        }
    }
}
