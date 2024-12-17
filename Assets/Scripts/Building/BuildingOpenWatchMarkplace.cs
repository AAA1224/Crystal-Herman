using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    public class BuildingOpenMarketplace : VCBehaviour<Building>
    {

        public override void onModelChanged()
        {
            model.interacted.AddListener(interacted);
        }

        public override void onModelRemoved()
        {
            model.interacted.RemoveListener(interacted);
        }

        private void interacted()
        {
            if (model.Marketplace.seller != null)
            {
                if (model.Marketplace.offers.Count > 0 || model.Marketplace.seller == MainGameManager.Instance.localPlayer)
                {
                    MainGameManager.Instance.localPlayer.WatchedMarket = model.Marketplace;
                }
            }
            else
            {
                RootLogger.Warning(this, "Cannot open the marketplace, because no seller is set");
            }
        }
    }
}
