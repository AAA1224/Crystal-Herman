using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools;
using Herman;
using Photon.Realtime;

public class PlayerWatchMarketplace : VCBehaviour<PolyPlayer>
{
    public Panel watchedMarketPanel;
    public Button closeButton;

    public Panel watchedMarketOtherPanel;
    public Button closeOtherMarketButton;

    public override void onModelChanged()
    {
        model.ChangedWatchingMarketplace.AddListener(changedWatchedMarket);
        closeButton.onClick.AddListener(closeButtonClicked);

        closeOtherMarketButton.onClick.AddListener(closeButtonClicked);
    }

    public override void onModelRemoved()
    {
        model.ChangedWatchingMarketplace.RemoveListener(changedWatchedMarket);
        closeButton.onClick.RemoveListener(closeButtonClicked);

        closeOtherMarketButton.onClick.RemoveListener(closeButtonClicked);
    }

    private void changedWatchedMarket()
    {
        if (model.WatchedMarket != null && (!watchedMarketPanel.isOpen && !watchedMarketOtherPanel.isOpen))
        {
            if (model.WatchedMarket.seller != model)
            {
                //open view ui
                VC<IMarketplace>.addModelToAllControllers(model.WatchedMarket, watchedMarketOtherPanel.gameObject);
                watchedMarketOtherPanel.onOpen();
            }
            else
            {
                //open edit ui
                VC<IMarketplace>.addModelToAllControllers(model.WatchedMarket, watchedMarketPanel.gameObject);
                watchedMarketPanel.onOpen();
            }
        }
        else
        if (model.WatchedMarket == null && (watchedMarketPanel.isOpen || watchedMarketOtherPanel.isOpen))
        {
            //VC<IMarketplace>.removeModelFromAllControllers(model.WatchedMarket, watchedMarketPanel.gameObject);
            watchedMarketPanel.onClose();
            watchedMarketOtherPanel.onClose();
        }
    }

    private void closeButtonClicked()
    {
        model.WatchedMarket = null;
    }
}