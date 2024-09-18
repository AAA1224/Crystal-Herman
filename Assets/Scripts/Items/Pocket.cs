using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Herman
{

    /// <summary>
    /// The pocket contains the player's resources such as fiat money,
    /// complementary money, fairy dust and the remaining time allowance.
    /// </summary>
    public interface IPocket
    {
        /// <summary>
        /// Provides access to the player's balance in currencies known to the game.
        /// </summary>
        /// <value>The balance.</value>
        List<CurrencyValue> Balance { get; set; }
        /// <summary>
        /// Provides access to the remaining time allowance percentage of the monthly total.
        /// </summary>
        /// <value>The time allowance.</value>
        int TimeAllowance { get; set; }
        /// <summary>
        /// Provides access to the current amount of fairy dust.
        /// </summary>
        /// <value>The fairy dust.</value>
        int FairyDust { get; set; }
        /// <summary>
        /// Tries to get the balance of the specified currency.
        /// </summary>
        /// <returns><c>true</c>, if the pocket contains said currency, <c>false</c> otherwise.</returns>
        /// <param name="currency">The currency.</param>
        /// <param name="value">The balance.</param>
        bool TryGetBalance(Currency currency, out int value);
        /// <summary>
        /// Sets the balance for the specified currency.
        /// </summary>
        /// <param name="currency">Currency.</param>
        /// <param name="value">Value.</param>
        void SetBalance(Currency currency, int value);
        bool Contains(Currency currency);
    }

    /// <summary>
    /// The pocket contains the player's resources such as fiat money,
    /// complementary money, fairy dust and the remaining time allowance.
    /// </summary>
    [Serializable]
    public class Pocket : IPocket, IEquatable<Pocket>
    {
        [SerializeField] private List<CurrencyValue> _balance;
        [SerializeField] private int _timeAllowance;
        [SerializeField] private int _fairyDust;

        public Pocket()
        {
            this._balance = new List<CurrencyValue>();
            this._timeAllowance = 100;
            this._fairyDust = 0;
        }
        public Pocket(Pocket other)
        {
            this._balance = new List<CurrencyValue>(other.Balance);
            this._timeAllowance = other.TimeAllowance;
            this._fairyDust = other.FairyDust;
        }
        public List<CurrencyValue> Balance
        {
            get
            {
                return this._balance;
            }

            set
            {
                if (!this._balance.SequenceEqual(value))
                {
                    this._balance = value;
                }
            }
        }
        public int TimeAllowance
        {
            get
            {
                return this._timeAllowance;
            }

            set
            {
                if (this._timeAllowance != value)
                {
                    this._timeAllowance = value;
                }
            }
        }
        public int FairyDust
        {
            get
            {
                return this._fairyDust;
            }

            set
            {
                if (this._fairyDust != value)
                {
                    this._fairyDust = value;
                }
            }
        }
        public bool TryGetBalance(Currency currency, out int value)
        {
            if (this.Contains(currency))
            {
                CurrencyValue target = this._balance.Find(c => c.GetCurrency() == currency);
                value = target.value;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        public void SetBalance(Currency currency, int value)
        {
            if (this.Contains(currency))
            {
                int target = this._balance.FindIndex(c => c.GetCurrency() == currency);
                this._balance[target].value = value;           
            }
            else
            {
                this._balance.Add(new CurrencyValue(currency, value));
            }
        }
        public bool Contains(Currency currency)
        {
            return this._balance.Count(c => c.GetCurrency() == currency) == 1;
        }
        public bool Equals(Pocket other)
        {
            bool balanceEq = this.Balance.SequenceEqual(other.Balance);
            bool timeEq = Math.Abs(this.TimeAllowance - other.TimeAllowance) < float.Epsilon;
            bool fairyEq = this.FairyDust == other.FairyDust;

            return balanceEq && timeEq && fairyEq;
        }
        public override string ToString()
        {
            return String.Format("Pocket(b={0}, t={1}, f={2})", this._balance.ToString(), this._timeAllowance, this._fairyDust);
        }
    }
}
