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
    public class MarketplaceDisplay : VCBehaviour<IMarketplace>
    {
        public Text marketTitle;
        public Text marketDescription;
        public OfferSetMarketPairSet offerMarketPairSet;
        public OfferDisplay offerTemplate;

        private LinkedPool<OfferDisplay> offerPool;

        private void Awake()
        {
            this.offerPool = new LinkedPool<OfferDisplay>(offerTemplate);
        }

        public override void onModelChanged()
        {
            PolyPlayer player = MainGameManager.Instance.localPlayer;
            this.offerPool.releaseAll();
            foreach (Offer offer in this.model.offers)
            {
                if ((player.Mayor) || (!player.Mayor && !offer.visibleToMayor))
                {
                    addedOffer(offer);
                }
            }

            model.onOfferAdd.AddListener(addedOffer);
            model.onOfferRemove.AddListener(removedOffer);

            // Determine if the current market is on a special building.
            if (this.marketTitle != null)
            {
                this.marketTitle.text = this.model.LocalisedTitle;
            }

            if (this.marketDescription != null && this.offerMarketPairSet.offerMarketPairs.Any(p => p.marketplace.Equals(this.model)))
            {
                this.marketDescription.text = Localisation.instance.getLocalisedText("tutoQCreateOffer");
            }
            else
            {
                this.marketDescription.text = this.model.LocalisedDescription;
            }
        }

        public override void onModelRemoved()
        {
            this.offerPool.releaseAll();
            model.onOfferAdd.RemoveListener(addedOffer);
            model.onOfferRemove.RemoveListener(removedOffer);
        }

        private void addedOffer(Offer offer)
        {
            // Instantiate the template UI object and update any view controllers therein.
            OfferDisplay offerObject = this.offerPool.pop();
            VC<Offer>.addModelToAllControllers(offer, offerObject.gameObject);
            offerObject.gameObject.SetActive(true);
        }

        private void removedOffer(Offer offer)
        {
            RootLogger.Debug(this, "Remove Offer: {0}", offer);
            OfferDisplay deadOfferDisplay = offerPool.getUsed().FirstOrDefault(o => o.model.Equals(offer));
            offerPool.releaseOne(deadOfferDisplay);

            //Level.instance.authoritativePlayer.Pocket.TimeAllowance += offer.creationCost.Time;
        }
    }
}
