using Herman;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KoboldTools.Logging;
using Photon.Realtime;
using Photon.Pun;
using KoboldTools;

public class PolyPlayer
{
    public Player player;
    private List<Talent> _talents = new List<Talent>();
    private List<Incident> _incidents = new List<Incident>();
    private Pocket _pocket = new Pocket();
    private UnityEvent _playerStateChanged = new UnityEvent();
    private UnityEvent _characterChanged = new UnityEvent();
    private UnityEvent _changedWatchingMarketplace = new UnityEvent();

    private Character _loadedCharacter = null;
    private bool _runsForMayor = false;
    private bool _mayor = false;
    private Job _job = null;
    public Person _person = null;
    private Home _home = null;
    public float _foodHealthStatus = 0.0f;
    public int _goodFoodNumber = 0;
    public int _badFoodNumber = 0;

    public Home Home
    {
        get
        {
            return this._home;
        }
        set
        {
            this._home = value;
            this._playerStateChanged.Invoke();
        }
    }
    public List<Talent> Talents
    {
        get
        {
            return this._talents;
        }
        set
        {
            this._talents = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Pocket Pocket
    {
        get
        {
            return this._pocket;
        }
        set
        {
            this._pocket = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Person Person
    {
        get
        {
            return this._person;
        }
        set
        {
            this._person = value;
            this._playerStateChanged.Invoke();
        }
    }

    public Job Job
    {
        get
        {
            return this._job;
        }
        set
        {
            this._job = value;
            this._playerStateChanged.Invoke();
        }
    }

    public UnityEvent PlayerStateChanged
    {
        get
        {
            return this._playerStateChanged;
        }
    }
    public UnityEvent OnTurnCompleted
    {
        get
        {
            return this._onTurnCompleted;
        }
    }

    public UnityEvent OnWaitingForTurnCompletion
    {
        get
        {
            return this._onWaitingForTurnCompletion;
        }
    }


    private UnityEvent _onTurnCompleted = new UnityEvent();

    private UnityEvent _onWaitingForTurnCompletion = new UnityEvent();

    public int Points;

    public string uniqueId;
    //The number of WC offers made
    public int numWCOffers;
    //The total value of the WC offers
    public int valueWCOffers;
    //The number of WC offers bought
    public int numWCBought;
    //The total value of the WC offers bought
    public int valueWCBought;
    //The number of WC offers sold
    public int numWCSold;
    //The total value of the WC offers sold
    public int valueWCSold;
    //The number of Gold offers made
    public int numGoldOffers;
    //The total value of Gold offers
    public int valueGoldOffers;
    //The number of Gold offers bought
    public int numGoldBought;
    //The total value of Gold offers bought
    public int valueGoldBought;
    //The number of Gold offers sold
    public int numGoldSold;
    //The total value of Gold offers sold
    public int valueGoldSold;

    public Character LoadedCharacter
    {
        get
        {
            return this._loadedCharacter;
        }

        set
        {
            if (this._loadedCharacter != value)
            {
                this._loadedCharacter = value;
                this._characterChanged.Invoke();
            }
        }
    }
    public bool Mayor
    {
        get
        {
            return this._mayor;
        }
        set
        {
            this._mayor = value;
        }
    }

    public List<Incident> Incidents
    {
        get
        {
            return this._incidents;
        }
        set
        {
            this._incidents = value;
            this._playerStateChanged.Invoke();
        }
    }

    public MarketplaceSet _marketplaceDb = null;
    private Marketplace _watchedMarket = null;
    public Guid _ownMarketplace;
    public UiResource Resource;
    public string MayorTitleTextId = "offerNotificationMayorName";
    public string BuyTitleTextId = "offerNotificationBuyTitle";
    public string ReceiveTitleTextId = "offerNotificationReceiveTitle";
    public string WelfareTitleTextId = "offerNotificationWelfareTitle";
    public string CloseButtonTextId = "offerNotificationCloseButton";
    public string FiatCurrencySymbolId = "fiatCurrencyLetter";
    public string QCurrencySymbolId = "qCurrencyLetter";
    public string ForFreeTextId = "forFreeText";


    /// <summary>
    /// The <see cref="IMarketplace"/> the player is watching at the
    /// moment. This field is not synchronized over the network.
    /// </summary>
    public Marketplace WatchedMarket
    {
        get
        {
            return this._watchedMarket;
        }

        set
        {
            if (this._watchedMarket != value)
            {
                this._watchedMarket = value;
                this._changedWatchingMarketplace.Invoke();
            }
        }
    }

    public Marketplace OwnMarketplace
    {
        get
        {
            return this._marketplaceDb.getByGuid(this._ownMarketplace.ToString());
        }
    }

    public List<Marketplace> OwnedMarketplaces
    {
        get
        {
            return this._marketplaceDb.marketplaces.FindAll(e => !object.ReferenceEquals(this.OwnMarketplace, e) && object.ReferenceEquals(this, e.seller));
        }
    }

    public UnityEvent ChangedWatchingMarketplace
    {
        get
        {
            return this._changedWatchingMarketplace;
        }
    }

    public int CalculateRevenue(Offer offer, Currency currency)
    {
        Cost bC = offer.buyingCost;
        Benefit bB = offer.buyingBenefit;
        Cost sC = offer.sellingCost;
        Benefit sB = offer.sellingBenefit;

        int bCValue = 0;
        bC.TryGetExpenses(currency, out bCValue);

        int bBValue = 0;
        bB.TryGetIncome(currency, out bBValue);

        int sCValue = 0;
        sC.TryGetExpenses(currency, out sCValue);

        int sBValue = 0;
        sB.TryGetIncome(currency, out sBValue);

        return Math.Abs(bBValue - bCValue);
    }

    private OfferApplyEvent _onOfferApplied = new OfferApplyEvent();

    public OfferApplyEvent OnOfferApplied
    {
        get
        {
            return this._onOfferApplied;
        }
    }

    public void registerOfferApplyEvent()
    {
        this.OnOfferApplied.AddListener(onOfferApplied);
    }

    private void onOfferApplied(Offer offer, PolyPlayer buyer)
    {
        RootLogger.Info(this, "An offer was applied!");
        if (!offer.EquivalentTags(MainGameManager.Instance.taxTags))
        {
            string mayorTitle = Localisation.instance.getLocalisedText(this.MayorTitleTextId);
            string buyerName = buyer.Mayor ? mayorTitle : buyer.Person.LocalisedTitle;
            string title = String.Empty;
            string content = String.Empty;
            CurrencyValue balance = offer.SellingBalance;

            if (offer.EquivalentTags(MainGameManager.Instance.welfareTags))
            {
                title = Localisation.instance.getLocalisedFormat(this.WelfareTitleTextId, buyerName);
            }
            else if (balance.value > 0)
            {
                title = Localisation.instance.getLocalisedFormat(this.BuyTitleTextId, buyerName);
                string textId = balance.GetCurrency() == Currency.FIAT ? this.FiatCurrencySymbolId : this.QCurrencySymbolId;
                content = String.Format("{0} {1}", balance.value, Localisation.instance.getLocalisedText(textId));
            }
            else if (balance.value < 0)
            {
                title = Localisation.instance.getLocalisedFormat(this.ReceiveTitleTextId, buyerName);
                string textId = balance.GetCurrency() == Currency.FIAT ? this.FiatCurrencySymbolId : this.QCurrencySymbolId;
                content = String.Format("{0} {1}", balance.value, Localisation.instance.getLocalisedText(textId));
            }
            else
            {
                title = Localisation.instance.getLocalisedFormat(this.ReceiveTitleTextId, buyerName);
                content = Localisation.instance.getLocalisedText(this.ForFreeTextId);
            }

            KoboldTools.Alert.info(content, new KoboldTools.Alert.AlertParams
            {
                title = title,
                closeText = Localisation.instance.getLocalisedText(this.CloseButtonTextId),
                sprite = this.Resource.GetSpriteByTags(offer.tags),
            });
        }
    }
}
