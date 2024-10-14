using Herman;
using KoboldTools;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerDisplaySymbols : MonoBehaviour
{
    [Header("Taxe Collection")]
    public Button TaxCollectionButton;
    public string TaxTitleTextId = "offerPopUpTaxTitle";
    public string TaxDescriptionTextId = "offerPopUpTaxDescription";

    [Header("Welfare")]
    public Button WelfareButton;
    //public Offer WelfareOffer;
    public string WelfareTitleTextId = "offerPopUpWelfareTitle";
    public string WelfareDescriptionTextId = "offerPopUpWelfareDescription";

    [Header("General")]
    public UiResource Resource;
    public GameObject turnComplete;
    public string CloseButtonTextId = "offerPopUpCloseButton";
    public string ApplyButtonTextId = "offerPopUpApplyButton";

    //private Marketplace OwnMarketplace;
    private PolyPlayer LocalPlayer = new PolyPlayer();
    private PolyPlayer _model = new PolyPlayer();
    //private Offer CurrentTaxOffer;

    public PolyPlayer model
    {
        get
        {
            return this._model;
        }
        set
        {
            this._model = value;
            onModelChanged();
        }
    }

    public new IEnumerator Start()
    {
        while (MainGameManager.Instance == null)
        {
            yield return null;
        }

        MainGameManager.Instance.onAuthoritativePlayerChanged.AddListener(this.onAuthoritativePlayerChanged);
        this.onAuthoritativePlayerChanged();
    }

    public void onModelChanged()
    {
        if (this.model != null)
        {
            this.model.PlayerStateChanged.AddListener(this.onPlayerStateChanged);
            //this.OwnMarketplace = this.model.OwnMarketplace;
            //if (this.OwnMarketplace != null)
            //{
            //    this.OwnMarketplace.onOfferAdd.AddListener(this.onOffersChanged);
            //    this.OwnMarketplace.onOfferRemove.AddListener(this.onOffersChanged);
            //}
            this.updateSymbols();

            // Register to events for the turn completion symbol.
            if (this.turnComplete != null)
            {
                this.model.OnTurnCompleted.AddListener(this.onTurnCompleted);
                this.model.OnWaitingForTurnCompletion.AddListener(this.onWaitingForTurnCompletion);
                this.onTurnCompleted();
            }

            // Register the tax collection symbol button.
            if (this.TaxCollectionButton != null)
            {
                this.TaxCollectionButton.onClick.AddListener(this.onClickTaxSymbol);
            }

            // Register the welfare symbol button.
            if (this.WelfareButton != null)
            {
                this.WelfareButton.onClick.AddListener(this.onClickWelfareSymbol);
            }
        }
    }

    public void onModelRemoved()
    {
        //if (GameFlow.instance != null)
        //{
        //    GameFlow.instance.changeState.RemoveListener(this.onStateChanged);
        //}
        if (this.model != null)
        {
            this.model.PlayerStateChanged.RemoveListener(this.onPlayerStateChanged);
            //if (this.OwnMarketplace != null)
            //{
            //    this.OwnMarketplace.onOfferAdd.RemoveListener(this.onOffersChanged);
            //    this.OwnMarketplace.onOfferRemove.RemoveListener(this.onOffersChanged);
            //}
            if (this.turnComplete != null)
            {
                this.model.OnTurnCompleted.RemoveListener(this.onTurnCompleted);
                this.model.OnWaitingForTurnCompletion.RemoveListener(this.onWaitingForTurnCompletion);
            }
            if (this.TaxCollectionButton != null)
            {
                this.TaxCollectionButton.onClick.RemoveListener(this.onClickTaxSymbol);
            }
            if (this.WelfareButton != null)
            {
                this.WelfareButton.onClick.RemoveListener(this.onClickWelfareSymbol);
            }
        }
    }

    private void onAuthoritativePlayerChanged()
    {
        this.LocalPlayer = MainGameManager.Instance.localPlayer;
    }

    private void onTurnCompleted()
    {
        if (this.turnComplete != null)
        {
            this.turnComplete.SetActive(false);
        }
    }

    private void onWaitingForTurnCompletion()
    {
        if (this.turnComplete != null)
        {
            this.turnComplete.SetActive(true);
        }
    }

    //private void onOffersChanged(Offer offer)
    //{
    //    this.updateSymbols();
    //}

    private void onPlayerStateChanged()
    {
        this.updateSymbols();
    }

    //private void onStateChanged(int oldState, int newState)
    //{
    //    if (newState == (int)PolymoneyGameFlow.FlowStates.PLAYER_TRADE)
    //    {
    //        this.updateSymbols();
    //    }
    //}

    private void updateSymbols()
    {

        if (this.TaxCollectionButton != null)
        {
            if (LocalPlayer.Mayor)
            {
                //Offer taxOffer = this.OwnMarketplace.offers.Find(e => e.EquivalentTags(Level.instance.taxTags));
                //if (taxOffer != null)
                //{
                    this.TaxCollectionButton.gameObject.SetActive(true);
                //    this.CurrentTaxOffer = taxOffer;
                //}
                //else
                //{
                    //this.TaxCollectionButton.gameObject.SetActive(false);
                //}
            }
            else
            {
                this.TaxCollectionButton.gameObject.SetActive(false);
            }
        }

        if (this.WelfareButton != null)
        {
            if (LocalPlayer.Mayor)
            {
                Incident taxIncident = this.model.Incidents.Find(e => e.EquivalentTags(MainGameManager.Instance.taxTags) && e.State == IncidentState.UNTOUCHED);
                if (taxIncident != null)
                {
                    this.WelfareButton.gameObject.SetActive(true);
                }
                else
                {
                    this.WelfareButton.gameObject.SetActive(false);
                }
            }
            else
            {
                this.WelfareButton.gameObject.SetActive(false);
            }
        }
    }

    private void onClickTaxSymbol()
    {
        string title = Localisation.instance.getLocalisedText(this.TaxTitleTextId);
        string content = Localisation.instance.getLocalisedText(this.TaxDescriptionTextId);
        string closeText = Localisation.instance.getLocalisedText(this.CloseButtonTextId);
        string applyText = Localisation.instance.getLocalisedText(this.ApplyButtonTextId);

        Alert.show(false, title, content, null, closeText, applyText);

        //Alert.info(content, new Alert.AlertParams
        //{
        //    title = title,
        //    closeText = closeText,
        //    sprite = this.Resource.GetSpriteByTags(this.CurrentTaxOffer.tags),
        //    callbacks = new Alert.AlertCallback[] {
        //            new Alert.AlertCallback {
        //                buttonText = applyText,
        //                callback = () => {
        //                    RootLogger.Info(this, "Applying the tax offer ({0}) to buyer {1} and seller {2}", this.CurrentTaxOffer, this.LocalPlayer, this.model);
        //                    this.LocalPlayer.ClientApplyOffer(this.CurrentTaxOffer, this.LocalPlayer, this.model);
        //                    this.CurrentTaxOffer = null;
        //                    Alert.close();
        //                },
        //                mainButton = true,
        //            },
        //        },
        //});
    }

    private void onClickWelfareSymbol()
    {
        string title = Localisation.instance.getLocalisedText(this.WelfareTitleTextId);
        string content = Localisation.instance.getLocalisedText(this.WelfareDescriptionTextId);
        string closeText = Localisation.instance.getLocalisedText(this.CloseButtonTextId);
        string applyText = Localisation.instance.getLocalisedText(this.ApplyButtonTextId);

        Alert.show(false, title, content, null, closeText, applyText);

        //Alert.info(content, new Alert.AlertParams
        //{
        //    title = title,
        //    closeText = closeText,
        //    sprite = this.Resource.GetSpriteByTags(this.WelfareOffer.tags),
        //    callbacks = new Alert.AlertCallback[] {
        //            new Alert.AlertCallback {
        //                buttonText = applyText,
        //                callback = () => {
        //                    RootLogger.Info(this, "Applying the welfare offer ({0}) to buyer {1} and seller {2}", this.WelfareOffer, this.LocalPlayer, this.model);
        //                    this.LocalPlayer.ClientApplyOffer(this.WelfareOffer, this.LocalPlayer, this.model);
        //                    Alert.close();
        //                },
        //                mainButton = true,
        //            },
        //        },
        //});
    }
}
