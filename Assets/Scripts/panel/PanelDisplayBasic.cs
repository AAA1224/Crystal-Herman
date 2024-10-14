using UnityEngine;
using System.Collections;
namespace KoboldTools
{
    public class PanelDisplayBasic : ViewController<IPanel>
    {

        private bool _cacheInteracteable;
        //private bool _cacheBlocksRaycasts;

        public override void onModelChanged()
        {
            model.open.AddListener(opened);
            model.close.AddListener(closed);
            model.closeComplete.AddListener(closeCompleted);
            _cacheInteracteable = model.canvasGroup.interactable;
            //_cacheBlocksRaycasts = model.canvasGroup.blocksRaycasts;

            //set default
            if (!model.isOpen)
            {
                closeCompleted();
            }
            else
            {
                openCompleted();
            }
        }

        public override void onModelRemoved()
        {
            model.open.RemoveListener(opened);
            model.close.RemoveListener(closed);
            model.closeComplete.RemoveListener(closeCompleted);
        }

        private void opened()
        {
            model.canvasGroup.alpha = 1f;
            model.canvasGroup.interactable = _cacheInteracteable;
            model.canvasGroup.blocksRaycasts = _cacheInteracteable;
            model.onOpenComplete();
        }

        private void closed()
        {
            model.onCloseComplete();
        }

        private void closeCompleted()
        {
            model.canvasGroup.alpha = 0f;
            model.canvasGroup.interactable = false;
            model.canvasGroup.blocksRaycasts = false;
        }

        private void openCompleted()
        {
            model.canvasGroup.alpha = 1f;
        }

    }
}
