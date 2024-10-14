using System;
using KoboldTools.Logging;

namespace KoboldTools
{
    /// <summary>
    /// Provides a serializable wrapper around System.Guid. Originally from
    /// http://druss.co/2016/04/unity3d-serialize-and-deserialize-system-guid-using-jsonutility/
    /// </summary>
    [Serializable]
    public struct SerializableGuid: IComparable, IComparable<SerializableGuid>, IEquatable<SerializableGuid>
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
}
