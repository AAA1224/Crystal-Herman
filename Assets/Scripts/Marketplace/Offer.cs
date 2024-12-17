using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using KoboldTools;
using KoboldTools.Logging;

namespace Herman
{
    /// <summary>
    /// <see cref="UnityEvent"/> with an Offer as argument.
    /// </summary>
    public class OfferApplyEvent : UnityEvent<Offer, PolyPlayer> { }

    /// <summary>
    /// Contains an offer that can be added to a marketplace. Offers expose references to their <see cref="Benefit"/> and <see cref="Cost"/>
    /// </summary>
    public interface IOffer : IItem, INetworkUnique
    {
        /// <summary>
        /// If set to <c>true</c>, the offer will not be deleted once applied.
        /// </summary>
        bool persistent { get; }
        /// <summary>
        /// If set to <c>true</c>, the offer cannot be deleted at the end of the month.
        /// </summary>
        bool essential { get; }
        /// <summary>
        /// If set to <c>true</c>, the offer is only visible to the Mayor. If
        /// set to <c>false</c>, the offer is only visible to regular players.
        /// </summary>
        bool visibleToMayor { get; }
        /// <summary>
        /// Holds the tags that describe this offer. Can be used to link offers
        /// to talents and incidents.
        /// </summary>
        List<string> tags { get; }
        /// <summary>
        /// The cost for creating this offer. Applied to the creators <see cref="Pocket"/>
        /// </summary>
        Cost creationCost { get; set; }
        /// <summary>
        /// The benefit of creating this offer. Applied to the creator's <see cref="Pocket"/>
        /// </summary>
        Benefit creationBenefit { get; set; }
        /// <summary>
        /// The cost for selling this offer. Applied to the creators <see cref="Pocket"/>
        /// </summary>
        Cost sellingCost { get; set; }
        /// <summary>
        /// The cost for buying this offer. Applied to the buyers <see cref="Pocket"/>
        /// </summary>
        Cost buyingCost { get; set; }
        /// <summary>
        /// The benefit for buying this offer. Applied to the buyer <see cref="Player"/>
        /// </summary>
        Benefit buyingBenefit { get; set; }
        /// <summary>
        /// The benefit for selling this offer. Applied to the creator <see cref="Player"/>
        /// </summary>
        Benefit sellingBenefit { get; set; }
        /// <summary>
        /// Invoked, when the offer is applied to the relevant players.
        /// </summary>
        OfferApplyEvent offerApplied { get; }
    }
    [Serializable]
    [CreateAssetMenu(fileName = "NewOffer", menuName = "New Offer")]
    public class Offer : ScriptableObject, IOffer, IEquatable<Offer>
    {
        [SerializeField]
        private SerializableGuid _guid;
        [Tooltip("If set to true, the offer will not be deleted once applied.")]
        [SerializeField]
        private bool _persistent;
        [Tooltip("If set to true, the offer cannot be deleted at the end of the month.")]
        [SerializeField]
        private bool _essential;
        [Tooltip("If set to true, the offer is only visible to the Mayor. If set to false, the offer is only visible to regular players.")]
        [SerializeField]
        private bool _visibleToMayor;
        [Tooltip("The tags that describe this offer. Can be used to link offers to talents and incidents.")]
        [SerializeField]
        private List<string> _tags;
        [Tooltip("The cost of creating this offer. Applied to the seller when creating the offer.")]
        [SerializeField]
        private Cost _creationCost;
        [Tooltip("The benefit of creating this offer. Applied to the seller when creating the offer.")]
        [SerializeField]
        private Benefit _creationBenefit;
        [Tooltip("The cost of selling this offer. Applied to the seller when the offer is applied.")]
        [SerializeField]
        private Cost _sellingCost;
        [Tooltip("The cost of buying this offer. Applied to the buyer when the offer is applied.")]
        [SerializeField]
        private Cost _buyingCost;
        [Tooltip("The benefit of buying this offer. Applied to the buyer when the offer is applied.")]
        [SerializeField]
        private Benefit _buyingBenefit;
        [Tooltip("The benefit of selling this offer. Applied to the seller when the offer is applied.")]
        [SerializeField]
        private Benefit _sellingBenefit;
        [Tooltip("The title of the offer.")]
        [SerializeField]
        private string _itemTitle;
        [Tooltip("The description of the offer.")]
        [SerializeField]
        [TextArea]
        private string _itemDescription;
        private OfferApplyEvent _offerApplied = new OfferApplyEvent();

        public Offer Clone()
        {
            Offer newOffer = ScriptableObject.CreateInstance<Offer>();
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(this), newOffer);
            return newOffer;
        }

        public bool persistent
        {
            get
            {
                return this._persistent;
            }
        }

        public bool essential
        {
            get
            {
                return this._essential;
            }
        }

        public bool visibleToMayor
        {
            get
            {
                return this._visibleToMayor;
            }
        }

        public List<string> tags
        {
            get
            {
                return this._tags;
            }

            set
            {
                this._tags = value;
            }
        }

        public Cost creationCost
        {
            get
            {
                return _creationCost;
            }

            set
            {
                _creationCost = value;
            }
        }

        public Benefit creationBenefit
        {
            get
            {
                return _creationBenefit;
            }

            set
            {
                _creationBenefit = value;
            }
        }

        public Cost sellingCost
        {
            get
            {
                return _sellingCost;
            }

            set
            {
                _sellingCost = value;
            }
        }

        public Cost buyingCost
        {
            get
            {
                return _buyingCost;
            }

            set
            {
                _buyingCost = value;
            }
        }

        public Benefit buyingBenefit
        {
            get
            {
                return _buyingBenefit;
            }

            set
            {
                _buyingBenefit = value;
            }
        }

        public Benefit sellingBenefit
        {
            get
            {
                return _sellingBenefit;
            }

            set
            {
                _sellingBenefit = value;
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
                //no change
            }
        }

        public string Title
        {
            get
            {
                return _itemTitle;
            }
            set
            {
                _itemTitle = value;
            }
        }
        public string Description
        {
            get
            {
                return _itemDescription;
            }
            set
            {
                _itemDescription = value;
            }
        }

        public string LocalisedTitle
        {
            get
            {
                if (Localisation.instance.hasTextId(this._itemTitle))
                {
                    return Localisation.instance.getLocalisedText(this._itemTitle);
                }
                else
                {
                    return this._itemTitle;
                }
            }
        }

        public string LocalisedDescription
        {
            get
            {
                if (Localisation.instance.hasTextId(this._itemDescription))
                {
                    return Localisation.instance.getLocalisedText(this._itemDescription);
                }
                else
                {
                    return this._itemDescription;
                }
            }
        }

        public EntityType SellingEntityType
        {
            get
            {
                bool costNeutral = this.sellingCost.IsNeutral;
                bool benefitNeutral = this.sellingBenefit.IsNeutral;

                if (costNeutral && !benefitNeutral)
                {
                    return EntityType.RESOURCE;
                }
                else if (!costNeutral && benefitNeutral)
                {
                    return EntityType.NEED;
                }
                else
                {
                    return EntityType.NEUTRAL;
                }
            }
        }

        public EntityType BuyingEntityType
        {
            get
            {
                bool costNeutral = this.buyingCost.IsNeutral;
                bool benefitNeutral = this.buyingBenefit.IsNeutral;

                if (costNeutral && !benefitNeutral)
                {
                    return EntityType.RESOURCE;
                }
                else if (!costNeutral && benefitNeutral)
                {
                    return EntityType.NEED;
                }
                else
                {
                    return EntityType.NEUTRAL;
                }
            }
        }

        public CurrencyValue SellingBalance
        {
            get
            {
                Cost c = this.sellingCost;
                Benefit b = this.sellingBenefit;
                int fiatExpenses = 0;
                c.TryGetExpenses(Currency.FIAT, out fiatExpenses);
                int fiatIncome = 0;
                b.TryGetIncome(Currency.FIAT, out fiatIncome);
                int qExpenses = 0;
                c.TryGetExpenses(Currency.Q, out qExpenses);
                int qIncome = 0;
                b.TryGetIncome(Currency.Q, out qIncome);

                int fiatBalance = fiatIncome - fiatExpenses;
                int qBalance = qIncome - qExpenses;

                if (fiatBalance != 0)
                {
                    return new CurrencyValue(Currency.FIAT, fiatBalance);
                }
                else if (qBalance != 0)
                {
                    return new CurrencyValue(Currency.Q, qBalance);
                }
                else
                {
                    return new CurrencyValue(Currency.FIAT, 0);
                }
            }
        }

        public CurrencyValue BuyingBalance
        {
            get
            {
                Cost c = this.buyingCost;
                Benefit b = this.buyingBenefit;
                int fiatExpenses = 0;
                c.TryGetExpenses(Currency.FIAT, out fiatExpenses);
                int fiatIncome = 0;
                b.TryGetIncome(Currency.FIAT, out fiatIncome);
                int qExpenses = 0;
                c.TryGetExpenses(Currency.Q, out qExpenses);
                int qIncome = 0;
                b.TryGetIncome(Currency.Q, out qIncome);

                int fiatBalance = fiatIncome - fiatExpenses;
                int qBalance = qIncome - qExpenses;

                if (fiatBalance != 0)
                {
                    return new CurrencyValue(Currency.FIAT, fiatBalance);
                }
                else if (qBalance != 0)
                {
                    return new CurrencyValue(Currency.Q, qBalance);
                }
                else
                {
                    return new CurrencyValue(Currency.FIAT, 0);
                }
            }
        }

        public bool EquivalentTags(List<string> other)
        {
            return this.tags.SequenceEqual(other);
        }

        public bool Equals(Offer other)
        {
            return this.guid.Equals(other.guid) || (
                this.Id == other.Id &&
                this.Title == other.Title &&
                this.Description == other.Description &&
                this.persistent == other.persistent &&
                this.essential == other.essential &&
                this.visibleToMayor == other.visibleToMayor &&
                this.EquivalentTags(other.tags) &&
                this.creationCost.Equals(other.creationCost) &&
                this.creationBenefit.Equals(other.creationBenefit) &&
                this.sellingCost.Equals(other.sellingCost) &&
                this.buyingCost.Equals(other.buyingCost) &&
                this.buyingBenefit.Equals(other.buyingBenefit) &&
                this.sellingBenefit.Equals(other.sellingBenefit)
            );
        }

        public OfferApplyEvent offerApplied
        {
            get
            {
                return this._offerApplied;
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

        public override string ToString()
        {
            return String.Format("Offer(t={0}, p={1}, e={2}, v={3}, g={4})",
                    this.Title, this.persistent, this.essential, this.visibleToMayor, this.guid);
        }
    }
}
