using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using KoboldTools.Logging;
using Herman;

namespace Herman
{
    public interface IMarketplaceSetSyncProvider
    {
        void AddMarketplace(string marketplaceData, string marketplaceGuid);
        void RemoveMarketplace(string marketplaceGuid);
        void AddOffer(string marketplaceGuid, string offerData, string offerGuid);
        void RemoveOffer(string marketplaceGuid, string offerGuid);
        void ClearOffers(string marketplaceGuid, bool persistent);
        void SetMarketSeller(string marketplaceGuid, string uniqueId);
    }

    public class MarketplaceSetSync : MonoBehaviourPun, IMarketplaceSetSyncProvider
    {
        public MarketplaceSet runtimeMarketplaces = null;

        private void OnEnable()
        {
            runtimeMarketplaces.syncProvider = this;
        }

        private void OnDisable()
        {
            runtimeMarketplaces.syncProvider = null;
        }

        public void Start()
        {
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            while (photonView == null) yield return null;
            Debug.Log("init");
            // Distribute GUIDs for initial marketplaces
            int offerCount = 0;
            string[] newMarketGuids = new string[runtimeMarketplaces.marketplaces.Count];
            for (int i = 0; i < newMarketGuids.Length; i++)
            {
                newMarketGuids[i] = Guid.NewGuid().ToString();
                offerCount += runtimeMarketplaces.marketplaces[i].offers.Count;
            }

            string[] newOfferGuids = new string[offerCount];
            for (int i = 0; i < newOfferGuids.Length; i++)
            {
                newOfferGuids[i] = Guid.NewGuid().ToString();
            }

            photonView.RPC("RpcSetInitialGuids", RpcTarget.AllBuffered, newMarketGuids, newOfferGuids);
        }

        [PunRPC]
        private void RpcSetInitialGuids(string[] marketGuids, string[] offerGuids)
        {
            int offerIndex = 0;
            int marketIndex = 0;

            foreach (Marketplace marketplace in runtimeMarketplaces.marketplaces)
            {
                if (marketIndex >= marketGuids.Length)
                {
                    RootLogger.Exception(this, "Not enough GUIDs provided for initial marketplaces.");
                }
                else
                {
                    marketplace.guid = new Guid(marketGuids[marketIndex]);
                    marketIndex++;

                    foreach (Offer offer in marketplace.offers)
                    {
                        if (offerIndex >= offerGuids.Length)
                        {
                            RootLogger.Exception(this, "Not enough GUIDs provided for initial offers.");
                        }
                        else
                        {
                            RootLogger.Debug(this, "Rpc: Set GUID of Offer '{0}' to '{1}'", offer.name, offerGuids[offerIndex]);
                            offer.guid = new Guid(offerGuids[offerIndex]);
                            offerIndex++;
                        }
                    }
                }
            }
        }

        public void AddMarketplace(string marketplaceData, string marketplaceGuid)
        {
            Debug.Log("add marketplace");
            photonView.RPC("RpcAddMarketplace", RpcTarget.AllBuffered, marketplaceData, marketplaceGuid);
        }

        [PunRPC]
        private void RpcAddMarketplace(string marketplaceData, string marketplaceGuid)
        {
            Marketplace newMarketplace = ScriptableObject.CreateInstance<Marketplace>();
            JsonUtility.FromJsonOverwrite(marketplaceData, newMarketplace);
            newMarketplace.guid = new Guid(marketplaceGuid);
            runtimeMarketplaces.addMarketplace(newMarketplace);
        }

        public void AddOffer(string marketplaceGuid, string offerData, string offerGuid)
        {
            photonView.RPC("RpcAddOffer", RpcTarget.AllBuffered, marketplaceGuid, offerData, offerGuid);
        }

        [PunRPC]
        private void RpcAddOffer(string marketplaceGuid, string offerData, string offerGuid)
        {
            Offer newOffer = ScriptableObject.CreateInstance<Offer>();
            JsonUtility.FromJsonOverwrite(offerData, newOffer);
            newOffer.guid = new Guid(offerGuid);
            runtimeMarketplaces.getByGuid(marketplaceGuid).addOffer(newOffer);
        }

        public void RemoveMarketplace(string marketplaceGuid)
        {
            photonView.RPC("RpcRemoveMarketplace", RpcTarget.AllBuffered, marketplaceGuid);
        }

        [PunRPC]
        private void RpcRemoveMarketplace(string marketplaceGuid)
        {
            runtimeMarketplaces.removeMarketplace(runtimeMarketplaces.getByGuid(marketplaceGuid));
        }

        public void RemoveOffer(string marketplaceGuid, string offerGuid)
        {
            photonView.RPC("RpcRemoveOffer", RpcTarget.AllBuffered, marketplaceGuid, offerGuid);
        }

        [PunRPC]
        private void RpcRemoveOffer(string marketplaceGuid, string offerGuid)
        {
            Marketplace marketplace = runtimeMarketplaces.getByGuid(marketplaceGuid);
            Offer offer = marketplace.getOfferByGuid(offerGuid);
            marketplace.removeOffer(offer);
        }

        public void ClearOffers(string marketplaceGuid, bool persistent)
        {
            photonView.RPC("RpcClearOffers", RpcTarget.AllBuffered, marketplaceGuid, persistent);
        }

        [PunRPC]
        private void RpcClearOffers(string marketplaceGuid, bool persistent)
        {
            Marketplace marketplace = runtimeMarketplaces.getByGuid(marketplaceGuid);
            marketplace.clearOffers(persistent);
        }

        public void SetMarketSeller(string marketplaceGuid, string uniqueId)
        {
            Debug.Log("set market place seller from client " + marketplaceGuid + ", " + uniqueId);
            photonView.RPC("RpcSetMarketSeller", RpcTarget.AllBuffered, marketplaceGuid, uniqueId);
        }

        [PunRPC]
        private void RpcSetMarketSeller(string marketplaceGuid, string uniqueId)
        {
            Debug.Log("set market place seller at all clients " + marketplaceGuid + ", " + uniqueId);
            PolyPlayer player = MainGameManager.Instance.polyPlayers.Find(player => player.uniqueId == uniqueId);
            runtimeMarketplaces.getByGuid(marketplaceGuid).seller = player;
        }
    }
}
