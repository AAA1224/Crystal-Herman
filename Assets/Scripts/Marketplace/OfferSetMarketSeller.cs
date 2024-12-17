using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoboldTools;
using KoboldTools.Logging;
using UnityEngine.Networking;
using System.Linq;
using Photon.Pun;

namespace Herman{

    [System.Serializable]
    public class OfferMarketPair
    {
        public Offer offer;
        public Marketplace marketplace;
    }

    public class OfferSetMarketSeller : MonoBehaviour
    {
        public MarketplaceSet runtimeMarketplaces;
        public OfferSetMarketPairSet offerMarketPairSet;

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            foreach (OfferMarketPair omp in offerMarketPairSet.offerMarketPairs)
            {
                omp.offer.offerApplied.AddListener(offerApplied);
            }
        }

        private void offerApplied(Offer offer, PolyPlayer buyer)
        {
            OfferMarketPair[] pairs = offerMarketPairSet.offerMarketPairs.Where(p => p.offer.guid.Equals(offer.guid)).ToArray();
            foreach(OfferMarketPair pair in pairs)
            {
                runtimeMarketplaces.syncProvider.SetMarketSeller(pair.marketplace.guid.ToString(), buyer.uniqueId);
            }
        }

    }

}
