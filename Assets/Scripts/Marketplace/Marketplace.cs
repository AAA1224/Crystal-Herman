using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    public class OfferEvent : UnityEvent<Offer> { }

    /// <summary>
    /// Contains a list of <see cref="Offer"/>
    /// </summary>
    public interface IMarketplace : IItem, INetworkUnique
    {
        /// <summary>
        /// Indicates which player currently owns the marketplace.
        /// </summary>
        PolyPlayer seller { get; set; }
        /// <summary>
        /// A list of all the available <see cref="Offer"/> in the marketplace.
        /// </summary>
        List<Offer> offers { get; }
        /// <summary>
        /// Add a new <see cref="Offer"/>
        /// </summary>
        /// <param name="offer">The <see cref="Offer to be added"/></param>
        void addOffer(Offer offer);
        /// <summary>
        /// Remove a <see cref="Offer"/>
        /// </summary>
        /// <param name="offer">The <see cref="Offer to be removed"/></param>
        void removeOffer(Offer offer);
        /// <summary>
        /// Clears all offers on this marketplace. If the parameter persistent
        /// is set to true, also remove persistent offers.
        /// </summary>
        void clearOffers(bool persistent);
        /// <summary>
        /// Event that notifies listeners whan the seller has changed.
        /// </summary>
        UnityEvent sellerChanged { get; }
        /// <summary>
        /// Event that notifies listeners when a new <see cref="Offer"/> is added
        /// </summary>
        OfferEvent onOfferAdd { get; }
        /// <summary>
        /// Event that notifies listeners when a <see cref="Offer"/> is removed
        /// </summary>
        OfferEvent onOfferRemove { get; }
        /// <summary>
        /// Returns the offer with the specific guid. If no offer is found it will create a default offer and return it.
        /// </summary>
        /// <param name="guid">The guid as string</param>
        /// <returns>The <see cref="Offer"/></returns>
        Offer getOfferByGuid(string guid);
    }

    [CreateAssetMenu(fileName = "New Marketplace", menuName = "New Marketplace")]
    public class Marketplace : ScriptableObject, IMarketplace, IEquatable<Marketplace>
    {
        [SerializeField]
        private Guid _guid;
        [SerializeField]
        private List<Offer> _offers = new List<Offer>();
        [SerializeField]
        private string _title = "";
        [SerializeField]
        private string _description = "";

        private PolyPlayer _seller = null;
        private UnityEvent _sellerChanged = new UnityEvent();
        private OfferEvent _onOfferAdd = new OfferEvent();
        private OfferEvent _onOfferRemove = new OfferEvent();

#if UNITY_EDITOR
        //handling initial offers reseting when exiting playmode in editor
        private List<Offer> _initialOffers = new List<Offer>();
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += playModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= playModeChanged;
        }

        private void playModeChanged(PlayModeStateChange change)
        {
            if(change == PlayModeStateChange.EnteredPlayMode)
            {
                _initialOffers = _offers.Select(o => o).ToList();
            }
            if(change == PlayModeStateChange.ExitingPlayMode)
            {
                _offers = _initialOffers;
            }
        }
#endif
        public void addOffer(Offer offer)
        {
            _offers.Add(offer);
            if (this._seller != null && offer.BuyingBalance.value != 0 && !offer.EquivalentTags(MainGameManager.Instance.taxTags))
            {
                //RootAnalytics.AddOffer(this._seller.netId.Value, offer.BuyingBalance.GetCurrency());
            }
            onOfferAdd.Invoke(offer);
        }

        public void removeOffer(Offer offer)
        {

            int idx = this._offers.FindIndex(e => e.Equals(offer));
            if (idx >= 0)
            {
                this._offers.RemoveAt(idx);
                onOfferRemove.Invoke(offer);
                //Add free time back to player
                Cost returnOffer = new Cost();
                returnOffer.Time = -offer.creationCost.Time;
                returnOffer.applyCost(seller.Pocket);
                
            }
            else
            {
                RootLogger.Exception(this, "The offer '{0}' cannot be deleted because it was not found on this marketplace.", offer);
            }
        }

        public void clearOffers(bool persistent)
        {
            List<Offer> currentOffers = new List<Offer>(this._offers);
            foreach (Offer offer in currentOffers)
            {
                if (!offer.essential && (!offer.persistent || (offer.persistent && persistent)))
                {
                    this._offers.Remove(offer);
                    onOfferRemove.Invoke(offer);
                }
            }
        }

        public void init(string _title, string _description, List<Offer> _initialOffers = null)
        {
            //init marketplace
            this._title = _title;
            this._description = _description;
            this._offers = _initialOffers != null ? _initialOffers : new List<Offer>();
        }

        public Offer getOfferByGuid(string guid)
        {
            Offer offer = _offers.FirstOrDefault(o => o.guid.ToString() == guid);
            if (offer != null)
            {
                return offer;
            }
            else
            {
                RootLogger.Error(this, "Offer with Guid {0} does not exist. Instantiating default Offer.", guid);
                return ScriptableObject.CreateInstance<Offer>();
            }
        }

        public PolyPlayer seller
        {
            get
            {
                return this._seller;
            }

            set
            {
                this._seller = value;
                this.sellerChanged.Invoke();
            }
        }

        public List<Offer> offers
        {
            get
            {
                return _offers;
            }
        }

        public int Id
        {
            get
            {
                return -1;
            }

            set
            {
                //id not implemented for marketplace
            }
        }

        public string Title {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        public string Description {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string LocalisedTitle
        {
            get
            {
                if (Localisation.instance.hasTextId(this._title))
                {
                    return Localisation.instance.getLocalisedText(this._title);
                }
                else
                {
                    RootLogger.Debug(this, "The title '{0}' cannot be localised", this._title);
                    return this._title;
                }
            }
        }

        public string LocalisedDescription
        {
            get
            {
                if (Localisation.instance.hasTextId(this._description))
                {
                    return Localisation.instance.getLocalisedText(this._description);
                }
                else
                {
                    return this._description;
                }
            }
        }

        public UnityEvent sellerChanged
        {
            get
            {
                return this._sellerChanged;
            }
        }

        public OfferEvent onOfferAdd
        {
            get
            {
                return _onOfferAdd;
            }
        }

        public OfferEvent onOfferRemove
        {
            get
            {
                return _onOfferRemove;
            }
        }

        public SerializableGuid guid
        {
            get
            {
                return _guid;
            }

            set
            {
                _guid = value;
            }
        }

        public bool Equals(Marketplace other)
        {
            return this.guid.Equals(other.guid);
        }
    }
}
