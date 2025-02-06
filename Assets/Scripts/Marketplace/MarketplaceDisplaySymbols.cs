using Herman;
using KoboldTools;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools.Logging;

public abstract class MarketplaceDisplaySymbols : MonoBehaviour
{
    public Transform symbolTemplate;

    private Marketplace market;
    private Pool<Transform> symbolPool;

    public abstract Marketplace getMarketplace();

    public void Awake()
    {
        this.market = this.getMarketplace();
        symbolPool = new Pool<Transform>(symbolTemplate);
    }

    public void Start()
    {
        if (this.market != null)
        {
            this.market.onOfferAdd.AddListener(this.onOffersChanged);
            this.market.onOfferRemove.AddListener(this.onOffersChanged);
            onOffersChanged(null);
        }
    }

    public void onOffersChanged(Offer offer)
    {
        if (this.market.seller != null)
        {
            //this.offersAvailable.SetActive(true);
            symbolPool.releaseAll();
            foreach (Offer o in this.market.offers)
            {
                PolyPlayer player = MainGameManager.Instance.localPlayer;
                if (!player.Mayor && !o.visibleToMayor)
                {
                    Transform t = symbolPool.pop();
                    VC<Offer>.addModelToAllControllers(o, t.gameObject, true);
                    t.gameObject.SetActive(true);

                    Button b = t.GetComponent<Button>();
                    if (b != null)
                    {
                        b.onClick.RemoveAllListeners();
                        b.onClick.AddListener(() => {
                            RootLogger.Debug(this, "Executed the onClick handler");
                            player.WatchedMarket = this.market;
                        });
                    }
                }
            }
        }
        else
        {
            symbolPool.releaseAll();
            //this.offersAvailable.SetActive(false);
        }
    }

}
