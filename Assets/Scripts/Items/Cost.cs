using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Herman
{
    /// <summary>
    /// Applies a cost of resources to a pocket
    /// </summary>
    public interface ICost
    {
        /// <summary>
        /// Determines the magnitude of expenses for each currency,
        /// when the cost is applied in <see cref="Cost.applyCost"/>.
        /// </summary>
        /// <value>The expenses.</value>
        List<CurrencyValue> Expenses { get; set; }
        /// <summary>
        /// Returns <c>true</c> if this cost contains an expenses value for
        /// the specified currency.
        /// </summary>
        bool ContainsExpenses(Currency currency);
        /// <summary>
        /// Tries to retrieve the value of the specified currency removed by this
        /// cost.
        /// </summary>
        bool TryGetExpenses(Currency currency, out int value);
        /// <summary>
        /// Sets the expenses value for the specified currency.
        /// </summary>
        void SetExpenses(Currency currency, int value);
        uint BreakBuilding { get; set; }
        /// <summary>
        /// Determines the amount of time detracted from the <see cref="Pocket"/>
        /// when the cost is applied in <see cref="Cost.applyCost"/>.
        /// </summary>
        /// <value>The time expenditure.</value>
        int Time { get; set; }
        /// <summary>
        /// Applies the cost to the resources in <paramref name="pocket"/>
        /// </summary>
        /// <param name="pocket">the pocket to apply the cost to</param>
        void applyCost(Pocket pocket);
        /// <summary>
        /// Returns <c>true</c>, if the cost will not affect the player.
        /// </summary>
        bool IsNeutral { get; }
    }

    /// <summary>
    /// Applies a cost of resources to a pocket
    /// </summary>
    [Serializable]
    public class Cost : ICost, IEquatable<Cost>
    {
        [SerializeField] private List<CurrencyValue> expenses;
        [SerializeField] private uint breakBuilding;
        [SerializeField] private int time;

        public Cost()
        {
            this.expenses = new List<CurrencyValue>();
            this.breakBuilding = 0;
            this.time = 0;
        }
        public Cost(Cost other)
        {
            this.expenses = new List<CurrencyValue>(other.Expenses);
            this.breakBuilding = other.BreakBuilding;
            this.time = other.Time;
        }
        public Cost(Benefit opposite)
        {
            this.expenses = new List<CurrencyValue>();
            foreach (CurrencyValue entry in opposite.Income)
            {
                this.expenses.Add(entry);
            }
            this.breakBuilding = opposite.RepairBuilding;
            this.time = 0;
        }
        public List<CurrencyValue> Expenses
        {
            get
            {
                return this.expenses;
            }

            set
            {

                this.expenses = value;
            }
        }
        public bool ContainsExpenses(Currency currency)
        {
            return this.expenses.Count(c => c.GetCurrency() == currency) == 1;
        }

        public bool TryGetExpenses(Currency currency, out int value)
        {
            if (this.ContainsExpenses(currency))
            {
                CurrencyValue target = this.expenses.Find(c => c.GetCurrency() == currency);
                value = target.value;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        public void SetExpenses(Currency currency, int value)
        {
            if (this.ContainsExpenses(currency))
            {
                int target = this.expenses.FindIndex(e => e.GetCurrency() == currency);
                this.expenses[target].value = value;
            }
            else
            {
                this.expenses.Add(new CurrencyValue(currency, value));
            }
        }
        public uint BreakBuilding
        {
            get
            {
                return this.breakBuilding;
            }

            set
            {
                this.breakBuilding = value;
            }
        }
        public int Time
        {
            get
            {
                return this.time;
            }

            set
            {
                this.time = value;
            }
        }
        public void applyCost(Pocket pocket)
        {
            // Apply the financial costs.
            if (this.expenses.Count > 0)
            {
                foreach (CurrencyValue e in this.expenses)
                {
                    try
                    {
                        Currency currency = e.GetCurrency();
                        int oldValue = 0;

                        if (pocket.TryGetBalance(currency, out oldValue))
                        {
                            pocket.SetBalance(currency, oldValue - e.value);
                        }
                        else
                        {
                            pocket.SetBalance(currency, -e.value);
                        }
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError( "The string {0} cannot be coerced to a member of the enum Currency." + e.currency);
                    }
                }
            }

            // Apply the time expenditure
            if (this.time > 0)
            {
                pocket.TimeAllowance -= time;
            }
            else if(this.time < 0)
            {
                pocket.TimeAllowance -= time;
                Debug.Log("Returning " + time + "for a total of " + pocket.TimeAllowance);
            }
        }
        public bool IsNeutral
        {
            get
            {
                return this.Expenses.Count == 0 &&
                    this.BreakBuilding == 0 &&
                    this.Time == 0;
            }
        }
        public bool Equals(Cost other)
        {
            bool expensesEq = this.Expenses.SequenceEqual(other.Expenses);
            bool breakEq = this.BreakBuilding == other.BreakBuilding;
            bool timeEq = this.Time == other.Time;

            return expensesEq && timeEq && breakEq;
        }
        public override string ToString()
        {
            return String.Format("Cost(e={0}, t={1}, bb={2})", this.expenses.ToString(), this.time, this.BreakBuilding);
        }
    }
}
