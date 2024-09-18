using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KoboldTools;

namespace Herman
{
    [Serializable]
    public struct SerializableGuid : IComparable, IComparable<SerializableGuid>, IEquatable<SerializableGuid>
    {
        public string Value;

        private SerializableGuid(string value)
        {
            this.Value = value;
        }

        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid { Value = Guid.NewGuid().ToString() };
        }

        public static implicit operator SerializableGuid(Guid other)
        {
            return new SerializableGuid(other.ToString());
        }

        public static implicit operator Guid(SerializableGuid other)
        {
            return new Guid(other.Value);
        }

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }

            if (!(other is SerializableGuid))
            {
                throw new ArgumentException("Cannot compare SerializableGuid with arbitrary objects");
            }
            SerializableGuid guid = (SerializableGuid)other;

            return guid.Value == this.Value ? 0 : 1;
        }

        public int CompareTo(SerializableGuid other)
        {
            return other.Value == this.Value ? 0 : 1;
        }

        public bool Equals(SerializableGuid other)
        {
            return this.Value == other.Value;
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Value != null ? this.Value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return !String.IsNullOrEmpty(this.Value) ? new Guid(this.Value).ToString() : string.Empty;
        }
    }
    
    [Serializable]
    public enum IncidentState
    {
        UNTOUCHED = 0,
        IGNORED = 1,
        RESOLVED = 2,
        APPLIED = 3,
    }

    [Serializable]
    public enum IncidentCategory
    {
        CITY = 1,
        RECURRENT_CITY = 2,
        RECURRENT = 3,
        TALENT = 4,
        LUCK = 5,
        DISASTER = 6,
    }

    /// <summary>
    /// Defines an incident that may occur to a player over the course of the game.
    /// Currently, they are chosen randomly twice for every turn and player.
    /// </summary>
    [Serializable]
    public class Incident : IItem, IEquatable<Incident>
    {
        [SerializeField] private SerializableGuid instanceId;
        [SerializeField] private int id;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private IncidentState state;
        [SerializeField] private bool ignorable;
        [SerializeField] private bool immediate;
        [SerializeField] private bool influenceable;
        [SerializeField] private int pickPoolSize;
        [SerializeField] private int month;
        [SerializeField] private string type;
        [SerializeField] private List<string> tags;
        [SerializeField] private string addSerializedOffer;
        [SerializeField] private Cost applicationCost;
        [SerializeField] private Benefit applicationBenefit;
        [SerializeField] private Cost ignoranceCost;

        public Incident()
        {
            this.instanceId = Guid.NewGuid();
            this.id = -1;
            this.title = null;
            this.description = null;
            this.state = IncidentState.UNTOUCHED;
            this.ignorable = false;
            this.immediate = false;
            this.influenceable = false;
            this.pickPoolSize = 1;
            this.month = 0;
            this.type = null;
            this.tags = new List<string>();
            this.addSerializedOffer = null;
            this.applicationCost = new Cost();
            this.applicationBenefit = new Benefit();
            this.ignoranceCost = new Cost();
        }

        public Incident(Incident other)
        {
            this.instanceId = other.InstanceId;
            this.Id = other.Id;
            this.Title = other.Title;
            this.Description = other.Description;
            this.State = other.State;
            this.Ignorable = other.Ignorable;
            this.Immediate = other.Immediate;
            this.Influenceable = other.Influenceable;
            this.PickPoolSize = other.PickPoolSize;
            this.Month = other.Month;
            this.Type = other.Type;
            this.Tags = new List<string>(other.Tags);
            this.AddSerializedOffer = other.AddSerializedOffer;
            this.ApplicationCost = new Cost(other.ApplicationCost);
            this.ApplicationBenefit = new Benefit(other.ApplicationBenefit);
            this.IgnoranceCost = new Cost(other.IgnoranceCost);
        }
        public Incident Clone()
        {
            Incident tmp = new Incident(this);
            tmp.instanceId = Guid.NewGuid();
            return tmp;
        }
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        public SerializableGuid InstanceId
        {
            get
            {
                return this.instanceId;
            }

            set
            {
                this.instanceId = value;
            }
        }
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }

        public string LocalisedTitle
        {
            get
            {
                if (Localisation.instance.hasTextId(this.title))
                {
                    return Localisation.instance.getLocalisedText(this.title);
                }
                else
                {
                    return this.title;
                }
            }
        }

        public string LocalisedDescription
        {
            get
            {
                if (Localisation.instance.hasTextId(this.description))
                {
                    return Localisation.instance.getLocalisedText(this.description);
                }
                else
                {
                    return this.description;
                }
            }
        }

        public IncidentState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
            }
        }

        /// <summary>
        /// If true, the incident may be ignored by players.
        /// </summary>
        public bool Ignorable
        {
            get
            {
                return this.ignorable;
            }

            set
            {
                this.ignorable = value;
            }
        }

        /// <summary>
        /// If true, the incident shall be applied immediately, not at the end
        /// of the month.
        /// </summary>
        public bool Immediate
        {
            get
            {
                return this.immediate;
            }

            set
            {
                this.immediate = value;
            }
        }

        /// <summary>
        /// If true, the incident may be influenced by players through trading
        /// or through their own talents.
        /// </summary>
        public bool Influenceable
        {
            get
            {
                return this.influenceable;
            }

            set
            {
                this.influenceable = value;
            }
        }

        /// <summary>
        /// Determines the relative probability of picking the incident
        /// over other incidents.
        /// </summary>
        /// <value>The size of the incident pick pool.</value>
        public int PickPoolSize
        {
            get
            {
                return this.pickPoolSize;
            }

            set
            {
                this.pickPoolSize = value;
            }
        }

        /// <summary>
        /// Determines the game month in which the incident occurs to the city.
        /// Only actually has a meaning for the city player and will not affect
        /// incidents for other players.
        /// </summary>
        /// <value>The month in which the incident will occur to the city.</value>
        public int Month
        {
            get
            {
                return this.month;
            }

            set
            {
                this.month = value;
            }
        }

        /// <summary>
        /// Determines the type of incident.
        /// </summary>
        /// <value>The type of the incident.</value>
        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Determines the tags associated with the incident.
        /// </summary>
        /// <value>The incident tags.</value>
        public List<string> Tags
        {
            get
            {
                return this.tags;
            }

            set
            {
                this.tags = value;
            }
        }
        public string AddSerializedOffer
        {
            get
            {
                return this.addSerializedOffer;
            }

            set
            {
                this.addSerializedOffer = value;
            }
        }

        /// <summary>
        /// Specifies the costs an incident may cause.
        /// </summary>
        /// <value>The incident cost.</value>
        public Cost ApplicationCost
        {
            get
            {
                return this.applicationCost;
            }

            set
            {
                this.applicationCost = value;
            }
        }

        /// <summary>
        /// Specifies the possible benefits of the incident.
        /// </summary>
        /// <value>The incident benefit.</value>
        public Benefit ApplicationBenefit
        {
            get
            {
                return this.applicationBenefit;
            }

            set
            {
                this.applicationBenefit = value;
            }
        }

        public EntityType ApplicationEntityType
        {
            get
            {
                bool costNeutral = this.ApplicationCost.IsNeutral;
                bool benefitNeutral = this.ApplicationBenefit.IsNeutral;

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

        public CurrencyValue ApplicationBalance
        {
            get
            {
                Cost c = this.ApplicationCost;
                Benefit b = this.ApplicationBenefit;
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

        public Cost IgnoranceCost
        {
            get
            {
                return this.ignoranceCost;
            }

            set
            {
                this.ignoranceCost = value;
            }
        }

        public bool ContainsTags(List<string> other)
        {
            return other.All(t => this.Tags.Contains(t));
        }

        public bool EquivalentTags(List<string> other)
        {
            return this.Tags.SequenceEqual(other);
        }

        public bool Identical(Incident other)
        {
            return this.InstanceId.Equals(other.InstanceId) &&
                this.Id == other.Id &&
                this.Title == other.Title &&
                this.Description == other.Description &&
                this.State == other.State &&
                this.Ignorable == other.Ignorable &&
                this.Immediate == other.Immediate &&
                this.Influenceable == other.Influenceable &&
                this.PickPoolSize == other.PickPoolSize &&
                this.Month == other.Month &&
                this.Type == other.Type &&
                this.EquivalentTags(other.Tags) &&
                this.AddSerializedOffer == other.AddSerializedOffer &&
                this.ApplicationCost.Equals(other.ApplicationCost) &&
                this.ApplicationBenefit.Equals(other.ApplicationBenefit) &&
                this.IgnoranceCost.Equals(other.IgnoranceCost);
        }

        public bool Equals(Incident other)
        {
            return this.InstanceId.Equals(other.InstanceId);
        }

        public override string ToString()
        {
            return String.Format("Incident(id={0}, title='{1}', state={2}, instanceId={3})", this.Id, this.Title, this.State, this.InstanceId);
        }
    }

}
