using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    public class MarketplaceDisplayCarousel : VCBehaviour<IMarketplace>
    {
        public MarketplaceCarousel carousel = null;
        public Text marketTitle;
        public Text marketDescription;

        private List<Offer> offerList = new List<Offer>();

        public override void onModelChanged()
        {
            RootLogger.Debug(this, "onModelChanged called");
            offerList = new List<Offer>();

            PolyPlayer player = MainGameManager.Instance.localPlayer;
            foreach (Offer offer in this.model.offers)
            {
                if ((player.Mayor && offer.visibleToMayor) || (!player.Mayor && !offer.visibleToMayor))
                {
                    offerList.Add(offer);
                }
            }

            model.onOfferAdd.AddListener(addedOffer);
            model.onOfferRemove.AddListener(removedOffer);

            VC<List<Offer>>.addModelToAllControllers(offerList, carousel.gameObject);
        }

        public override void onModelRemoved()
        {
            RootLogger.Debug(this, "onModelRemoved called");
            model.onOfferAdd.RemoveListener(addedOffer);
            model.onOfferRemove.RemoveListener(removedOffer);
        }

        private void addedOffer(Offer offer)
        {
            RootLogger.Debug(this, "addedOffer({0}) called", offer);
            offerList.Add(offer);
            carousel.initialize();
        }

        private void removedOffer(Offer offer)
        {
            RootLogger.Debug(this, "removedOffer({0}) called", offer);
            offerList.Remove(offer);
            carousel.initialize();
        }
    }
}
