using KoboldTools.Logging;
using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingShowSymbol : VCBehaviour<Building>
{
    public GameObject symbol = null;
    public Text title = null;

    public override void onModelChanged()
    {
        this.model.Marketplace.sellerChanged.AddListener(this.sellerChanged);
        this.symbol.SetActive(false);
    }

    public override void onModelRemoved()
    {
        this.model.Marketplace.sellerChanged.RemoveListener(this.sellerChanged);
    }

    private void sellerChanged()
    {
        RootLogger.Debug(this, "Seller changed");
        if (this.model.Marketplace.seller != null)
        {
            RootLogger.Debug(this, "Has real seller {0}", model);
            title.text = string.Format(Localisation.instance.getLocalisedText("specialBuildingOwnerTitle"), this.model.Marketplace.seller.Person.LocalisedTitle);
            this.symbol.SetActive(true);
        }
        else
        {
            this.symbol.SetActive(false);
        }
    }
}