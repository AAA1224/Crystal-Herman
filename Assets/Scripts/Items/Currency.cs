using System;
using System.Collections.Generic;
using System.Linq;

namespace Herman
{
    /// <summary>
    /// Defines the currencies known to the game.
    /// </summary>
    [Serializable]
    public enum Currency
    {
        /// <summary>
        /// This variant describes fiat money, or common currency.
        /// </summary>
        FIAT = 0x1,
        /// <summary>
        /// This variant describes the first complementary
        /// currency introduced by polymoney.
        /// </summary>
        Q = 0x2,
    }

    /// <summary>
    /// Defines whether a particular incident or offer is a resource or a need.
    /// </summary>
    [Serializable]
    public enum EntityType
    {
        /// <summary>
        /// This variant indicates a resource.
        /// </summary>
        RESOURCE = 0x1,
        /// <summary>
        /// This variant indicates a need.
        /// </summary>
        NEED = 0x2,
        /// <summary>
        /// This variant indicates neither resource nor need.
        /// </summary>
        NEUTRAL = 0x3,
    }

    /// <summary>
    /// Defines a pair of currency and value.
    /// </summary>
    [Serializable]
    public class CurrencyValue : IEquatable<CurrencyValue>
    {
        /// <summary>
        /// The currency type.
        /// </summary>
        public string currency;
        /// <summary>
        /// The amount of money in the specified currency.
        /// </summary>
        public int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polymoney.CurrencyValue"/> struct.
        /// </summary>
        /// <param name="currency">Currency.</param>
        /// <param name="value">Value.</param>
        public CurrencyValue(string currency, int value)
        {
            this.currency = currency;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polymoney.CurrencyValue"/> struct.
        /// </summary>
        /// <param name="currency">Currency.</param>
        /// <param name="value">Value.</param>
        public CurrencyValue(Currency currency, int value)
        {
            this.currency = currency.ToString();
            this.value = value;
        }

        /// <summary>
        /// Copies another currency value.
        /// </summary>
        public CurrencyValue(CurrencyValue other)
        {
            this.currency = other.currency;
            this.value = other.value;
        }

        /// <summary>
        /// Attempts to parse the string currency value as an enum of type <see cref="Currency"/>.
        /// </summary>
        /// <returns>The currency.</returns>
        public Currency GetCurrency()
        {
            return (Currency)Enum.Parse(typeof(Currency), this.currency);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Polymoney.CurrencyValue"/> is equal to the current <see cref="Polymoney.CurrencyValue"/>.
        /// </summary>
        /// <param name="other">The <see cref="Polymoney.CurrencyValue"/> to compare with the current <see cref="Polymoney.CurrencyValue"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Polymoney.CurrencyValue"/> is equal to the current
        /// <see cref="Polymoney.CurrencyValue"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(CurrencyValue other)
        {
            return this.currency == other.currency && this.value == other.value;
        }

        public override string ToString()
        {
            return String.Format("CurrencyValue({0}, {1})", this.currency, this.value);
        }

        public static CurrencyValue operator-(CurrencyValue self)
        {
            return new CurrencyValue(self.currency, -self.value);
        }

        public static CurrencyValue operator+(CurrencyValue lhs, CurrencyValue rhs)
        {
            if (lhs.GetCurrency() == rhs.GetCurrency())
            {
                return new CurrencyValue(lhs.GetCurrency(), lhs.value + rhs.value);
            }
            else
            {
                throw new Exception("Cannot add two instances of `CurrencyValue` with different currencies");
            }
        }

        public static CurrencyValue operator-(CurrencyValue lhs, CurrencyValue rhs)
        {
            if (lhs.GetCurrency() == rhs.GetCurrency())
            {
                return new CurrencyValue(lhs.GetCurrency(), lhs.value - rhs.value);
            }
            else
            {
                throw new Exception("Cannot subtract two instances of `CurrencyValue` with different currencies");
            }
        }

        public static bool operator<(CurrencyValue lhs, int rhs)
        {
            return lhs.value < rhs;
        }

        public static bool operator<=(CurrencyValue lhs, int rhs)
        {
            return lhs.value <= rhs;
        }

        public static bool operator==(CurrencyValue lhs, int rhs)
        {
            return lhs.value == rhs;
        }

        public static bool operator!=(CurrencyValue lhs, int rhs)
        {
            return lhs.value != rhs;
        }

        public static bool operator>=(CurrencyValue lhs, int rhs)
        {
            return lhs.value >= rhs;
        }

        public static bool operator>(CurrencyValue lhs, int rhs)
        {
            return lhs.value > rhs;
        }
    }

    [Serializable]
    public class CurrencyContainer
    {
        public List<CurrencyValue> Inner;

        /// <summary>
        /// Creates a new, empty container.
        /// </summary>
        public CurrencyContainer()
        {
            this.Inner = new List<CurrencyValue>();
        }

        /// <summary>
        /// Copies another container.
        /// </summary>
        public CurrencyContainer(CurrencyContainer other)
        {
            this.Inner = new List<CurrencyValue>(other.Inner);
        }

        /// <summary>
        /// Returns <c>true</c>, if the container has an entry for the
        /// specified currency.
        /// </summary>
        public bool Contains(Currency currency)
        {
            return this.Inner.Count(c => c.GetCurrency() == currency) == 1;
        }

        /// <summary>
        /// Retrieve the value for the specified currency. If the currency is
        /// not available, this method returns <c>0</c>.
        /// </summary>
        public int Get(Currency currency)
        {
            if (this.Contains(currency))
            {
                CurrencyValue target = this.Inner.Find(c => c.GetCurrency() == currency);
                return target.value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Sets the value for the specified currency.
        /// </summary>
        public void Set(Currency currency, int value)
        {
            if (this.Contains(currency))
            {
                int target = this.Inner.FindIndex(e => e.GetCurrency() == currency);
                this.Inner[target].value = value;
            }
            else
            {
                this.Inner.Add(new CurrencyValue(currency, value));
            }
        }
    }
}
