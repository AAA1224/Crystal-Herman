using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Herman
{
    /// <summary>
    /// Applies a benefit of resources to a pocket
    /// </summary>
    public interface IBenefit
    {
        /// <summary>
        /// Determines the set of tags that must match a known incident
        /// in order to remove that incident from the player.
        /// </summary>
        /// <value>The remove incident.</value>
        List<string> RemoveIncident { get; set; }
        /// <summary>
        /// Determines which talent will be added by the benefit, specified by
        /// the set of tags that identify said target.
        /// </summary>
        List<string> AddTalent { get; set; }
        /// <summary>
        /// Determines the amount of fairy dust given to the player.
        /// </summary>
        /// <value>The fairy dust.</value>
        int FairyDust { get; set; }
        /// <summary>
        /// Determines the magnitude of expenses for each currency,
        /// when the cost is applied in <see cref="Cost.applyCost"/>.
        /// </summary>
        /// <value>The expenses.</value>
        List<CurrencyValue> Income { get; set; }
        /// <summary>
        /// Returns <c>true</c> if this benefit contains an income value for
        /// the specified currency.
        /// </summary>
        bool ContainsIncome(Currency currency);
        /// <summary>
        /// Tries to retrieve the value of the specified currency added by this
        /// benefit.
        /// </summary>
        bool TryGetIncome(Currency currency, out int value);
        /// <summary>
        /// Sets the value for income of the specified currency.
        /// </summary>
        void SetIncome(Currency currency, int value);
        /// <summary>
        /// Determines which building (identified by NetId) will be repaired.
        /// </summary>
        uint RepairBuilding { get; set; }
        /// <summary>
        /// Applies the benefit to the resources in <paramref name="player"/>
        /// </summary>
        /// <param name="player">Player.</param>
        void applyBenefit(List<Talent> talents, List<Incident> incidents, Pocket pocket);
        /// <summary>
        /// Given a player, determines which incidents will be deleted
        /// by this benefit.
        /// </summary>
        /// <param name="player">Player.</param>
        List<int> getRemovableIncidents(List<Incident> incidents);
        bool IsNeutral { get; }
    }

    [Serializable]
    public class Benefit : IBenefit, IEquatable<Benefit>
    {
        [SerializeField] private List<string> removeIncident;
        [SerializeField] private List<string> addTalent;
        [SerializeField] private int fairyDust;
        [SerializeField] private List<CurrencyValue> income;
        [SerializeField] private uint repairBuilding;

        public Benefit()
        {
            this.removeIncident = new List<string>();
            this.addTalent = new List<string>();
            this.fairyDust = 0;
            this.income = new List<CurrencyValue>();
            this.repairBuilding = 0;
        }
        public Benefit(Benefit other)
        {
            this.removeIncident = new List<string>(other.RemoveIncident);
            this.addTalent = new List<string>(other.AddTalent);
            this.fairyDust = other.FairyDust;
            this.income = new List<CurrencyValue>(other.Income);
            this.repairBuilding = other.RepairBuilding;
        }
        public Benefit(Cost opposite)
        {
            this.removeIncident = new List<string>();
            this.addTalent = new List<string>();
            this.fairyDust = 0;
            this.income = new List<CurrencyValue>();
            foreach (CurrencyValue entry in opposite.Expenses)
            {
                this.income.Add(entry);
            }
            this.repairBuilding = opposite.BreakBuilding;
        }
        public List<string> RemoveIncident
        {
            get
            {
                return this.removeIncident;
            }

            set
            {
                this.removeIncident = value.OrderBy(q => q).ToList();
            }
        }
        public List<string> AddTalent
        {
            get
            {
                return this.addTalent;
            }

            set
            {
                this.addTalent = value.OrderBy(q => q).ToList();
            }
        }
        public int FairyDust
        {
            get
            {
                return this.fairyDust;
            }

            set
            {
                this.fairyDust = value;
            }
        }
        public List<CurrencyValue> Income
        {
            get
            {
                return this.income;
            }

            set
            {
                this.income = value;
            }
        }
        public bool ContainsIncome(Currency currency)
        {
            return this.income.Count(c => c.GetCurrency() == currency) == 1;
        }
        public bool TryGetIncome(Currency currency, out int value)
        {
            if (this.ContainsIncome(currency))
            {
                CurrencyValue target = this.income.Find(c => c.GetCurrency() == currency);
                value = target.value;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        public void SetIncome(Currency currency, int value)
        {
            if (this.ContainsIncome(currency))
            {
                int target = this.income.FindIndex(e => e.GetCurrency() == currency);
                this.income[target].value = value;
            }
            else
            {
                this.income.Add(new CurrencyValue(currency, value));
            }
        }
        public uint RepairBuilding
        {
            get
            {
                return this.repairBuilding;
            }

            set
            {
                this.repairBuilding = value;
            }
        }
        public void applyBenefit(List<Talent> talents, List<Incident> incidents, Pocket pocket)
        {
            // Apply the fairy dust increase.
            if (this.fairyDust != 0)
            {
                pocket.FairyDust += this.fairyDust;
            }

            // Apply the financial benefits.
            if (this.income.Count > 0)
            {
                foreach (CurrencyValue e in this.income)
                {
                    try
                    {
                        Currency currency = e.GetCurrency();
                        int oldValue = 0;
                        if (pocket.TryGetBalance(currency, out oldValue))
                        {
                            pocket.SetBalance(currency, oldValue + e.value);
                        }
                        else
                        {
                            pocket.SetBalance(currency, e.value);
                        }
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError("The string {0} cannot be coerced to a member of the enum Currency." + e.currency);
                    }
                }
            }

            // Apply talents.
            if (this.addTalent.Count > 0)
            {
                foreach (Talent talent in MainGameManager.Instance.loadedLevelData.Talents)
                {
                    if (talent.EquivalentTags(this.addTalent))
                    {
                        if (talents.Count(e => e.EquivalentTags(this.addTalent)) == 0)
                        {
                            talents.Add(talent);
                        }
                    }
                }
            }


            // Mark the requested incidents as resolved.
            List<int> resolveIndices = this.getRemovableIncidents(incidents);
            if (resolveIndices.Count > 0)
            {
                foreach (int idx in resolveIndices.OrderByDescending(q => q))
                {
                    incidents[idx].State = IncidentState.RESOLVED;
                }
            }
        }
        public List<int> getRemovableIncidents(List<Incident> incidents)
        {
            List<int> resolveIndices = new List<int>();
            if (this.removeIncident.Count > 0)
            {
                for (int i = 0; i < incidents.Count; i++)
                {
                    if (incidents[i].EquivalentTags(this.removeIncident) && incidents[i].Influenceable)
                    {
                        resolveIndices.Add(i);
                    }
                }
            }
            return resolveIndices;
        }
        public bool IsNeutral
        {
            get
            {
                return this.RemoveIncident.Count == 0 &&
                    this.AddTalent.Count == 0 &&
                    this.FairyDust == 0 &&
                    this.Income.Count == 0;
            }
        }
        public bool Equals(Benefit other)
        {
            bool incidentEq = this.RemoveIncident.SequenceEqual(other.RemoveIncident);
            bool talentEq = this.AddTalent.SequenceEqual(other.AddTalent);
            bool fairyEq = this.FairyDust == other.FairyDust;
            bool incomeEq = this.Income.SequenceEqual(other.Income);
            bool repairEq = this.RepairBuilding == other.RepairBuilding;

            return incidentEq && talentEq && fairyEq && incomeEq && repairEq;
        }
        public override string ToString()
        {
            return String.Format("Benefit(ri={0}, at={1}, f={2}, i={3}, rb={4})", this.removeIncident.ToString(), this.addTalent.ToString(), this.fairyDust, this.income.ToString(), this.RepairBuilding);
        }
    }
}
