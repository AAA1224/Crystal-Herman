using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools;
using KoboldTools.Logging;
using Herman;
using Photon.Realtime;

public class PlayerCreateOffer : VCBehaviour<PolyPlayer>
{
    [Header("Offer Data")]
    public int defaultTimeCost = 10;
    public Offer defaultOffer = null;

    [Header("UI")]
    public Panel createOfferPanel = null;
    public Button createBasicOffer = null;
    public Button createTalentOfferTemplate = null;
    public Transform creationButtons = null;
    public string newOfferTextId = "newOffer";
    public string createOfferFailMoneyTextId = "createOfferFailureMoney";
    public string createOfferFailTimeTextId = "createOfferFailureTime";

    [Header("Other")]
    public OfferSetMarketPairSet offerMarketPairSet;

    private Offer newOffer = null;
    private Pool<Button> talentOfferPool;
    private OfferEditorDisplay offerEditor;

    public override void onModelChanged()
    {
        offerEditor = createOfferPanel.GetComponentInChildren<OfferEditorDisplay>();


        if (offerEditor != null)
        {
            talentOfferPool = new Pool<Button>(createTalentOfferTemplate);
            model.ChangedWatchingMarketplace.AddListener(watchedMarketplaceChanged);
            offerEditor.applyButton.onClick.AddListener(applyCreation);
            offerEditor.cancelButton.onClick.AddListener(cancelCreation);
        }
        else
        {
            RootLogger.Error(this, "No Offer Editor found on creation Panel!");
        }
    }

    public override void onModelRemoved()
    {
        if (offerEditor != null)
        {
            model.ChangedWatchingMarketplace.RemoveListener(watchedMarketplaceChanged);
            offerEditor.applyButton.onClick.RemoveListener(applyCreation);
            offerEditor.cancelButton.onClick.RemoveListener(cancelCreation);
        }
        else
        {
            RootLogger.Error(this, "No Offer Editor found on creation Panel!");
        }
    }

    private void watchedMarketplaceChanged()
    {
        if (model.WatchedMarket != null && model.WatchedMarket.seller == model)
        {
            //setup pool
            if (talentOfferPool == null)
                talentOfferPool = new Pool<Button>(createTalentOfferTemplate);

            //mayor has non talent buttons
            if (!model.Mayor)
            {
                //setup talent buttons
                creationButtons.gameObject.SetActive(true);
                createBasicOffer.onClick.AddListener(() => createOffer(null));
                foreach (Talent talent in model.Talents)
                {
                    Talent currTalent = talent; //create new reference to talent, so that it will be available in anonymous delegates
                    Button talentButton = talentOfferPool.pop();

                    //add talent to all viewcontrollers on the button
                    VC<Talent>.addModelToAllControllers(currTalent, talentButton.gameObject);
                    talentButton.onClick.AddListener(() => createOffer(currTalent));
                    talentButton.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            //we are not seller of this marketplace. disable offer creation
            creationButtons.gameObject.SetActive(false);
            createBasicOffer.onClick.RemoveAllListeners();
            foreach (Button talentButton in talentOfferPool.getUsed())
            {
                talentButton.onClick.RemoveAllListeners();
            }
            talentOfferPool.releaseAll();
        }
    }

    private void cancelCreation()
    {
        createOfferPanel.onClose();
        VC<Offer>.removeModelFromAllControllers(newOffer, createOfferPanel.gameObject, true);
        Destroy(newOffer);
    }

    private void applyCreation()
    {
        //TODO validate temp offer
        offerEditor.applyValues();

        // If selling the offer would cost more than the player had money available (minus debt allowance), do not allow the creation of the offer.
        CurrencyValue sellingBalance = newOffer.SellingBalance;
        Currency sellingCurrency = sellingBalance.GetCurrency();
        int pocketValue;
        this.model.Pocket.TryGetBalance(sellingCurrency, out pocketValue);
        CurrencyValue pocketBalance = new CurrencyValue(sellingCurrency, pocketValue);
        CurrencyValue maxDebt = MainGameManager.Instance.MaximumDebt.SingleOrDefault(c => c.GetCurrency() == sellingCurrency);
        if (sellingBalance >= 0 || pocketBalance + maxDebt + sellingBalance >= 0)
        {
            if (sellingCurrency == Currency.FIAT)
            {
                model.valueGoldOffers += sellingBalance.value;
            }
            else if (sellingCurrency == Currency.Q)
            {
                model.valueWCOffers += sellingBalance.value;
            }


            RootLogger.Info(this, "Offer created: {0}", JsonUtility.ToJson(newOffer, true));

            //apply offer creation and send it via command to the server
            createOfferPanel.onClose();
            MainGameManager.Instance.createOffer(MainGameManager.Instance.localPlayer.uniqueId, model.WatchedMarket.guid.ToString(), newOffer);
            Destroy(newOffer);
        }
        else
        {
            RootLogger.Warning(this, "Cannot create an offer due to lacking money");
            KoboldTools.Alert.info(this.createOfferFailMoneyTextId, new KoboldTools.Alert.AlertParams { closeText = "btnClose", useLocalization = true });
            createOfferPanel.onClose();
            Destroy(newOffer);
        }
    }

    private void createOffer(Talent talent)
    {
        int playerTime = model.Pocket.TimeAllowance;

        if (playerTime >= defaultTimeCost)
        {
            // Create the offer from the default template.
            newOffer = ScriptableObject.CreateInstance<Offer>();
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(defaultOffer), newOffer);

            // Set the time cost of the offer
            newOffer.creationCost.Time = defaultTimeCost;

            // Offers on special marketplaces give players polymoney!
            Marketplace specialMarket = this.offerMarketPairSet.offerMarketPairs.Where(p => p.marketplace.Equals(model.WatchedMarket)).Select(p => p.marketplace).SingleOrDefault();
            if (specialMarket != null)
            {
                RootLogger.Info(this, "This is offer was created on a special market: {0}", newOffer);
                Building assocBuilding = MainGameManager.Instance.Buildings.Where(p => p.Marketplace != null && p.Marketplace.Equals(specialMarket)).SingleOrDefault();
                if (assocBuilding != null)
                {
                    RootLogger.Info(this, "This special offer will now repair the building {0}, if bought.", assocBuilding);
                    newOffer.sellingBenefit.RepairBuilding = assocBuilding.netId;
                }
                newOffer.creationBenefit.SetIncome(Currency.Q, MainGameManager.Instance.PolymoneyPerFreeTime);
                //Add WC offer creation stat to player
                model.numWCOffers++;
                //Add value of WC offer
                int pocketValue = newOffer.SellingBalance.value;

            }
            //If the offer is not made on the special market
            else
            {
                //Add Gold offer creation stat to player
                model.numGoldOffers++;
                //Add value of Gold offer
                int pocketValue = newOffer.SellingBalance.value;
                model.valueGoldOffers += pocketValue;
            }

            if (talent != null)
            {
                //set offer title to talent
                newOffer.Title = Localisation.instance.getLocalisedFormat(this.newOfferTextId, talent.LocalisedTitle);
                RootLogger.Info(this, "Create offer {0} that has creation cost {1}", newOffer.Title, newOffer.creationCost);
                //add talent benefits
                foreach (string talentTag in talent.Tags)
                {
                    newOffer.buyingBenefit.RemoveIncident.Add(talentTag);
                    newOffer.tags.Add(talentTag);
                }
            }
            else
            {
                //add trade tag
                newOffer.tags.Add("Trade");
            }

            //add offer to panel view and show panel
            VC<Offer>.addModelToAllControllers(newOffer, createOfferPanel.gameObject, true);
            createOfferPanel.onOpen();
        }
        else
        {
            KoboldTools.Alert.info(this.createOfferFailTimeTextId, new KoboldTools.Alert.AlertParams { closeText = "btnClose", useLocalization = true });
        }
    }
}

