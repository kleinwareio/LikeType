using System;
using System.Collections;
using System.Collections.Generic;

namespace Kleinware.LikeType
{
    /// <summary>
    /// An immutable enumerable collection of Ts. Equals, ==, !=, GetHashCode, and ToString have already been implemented for structural equality. If two instances contain an equal collection of items (as determined by item.Equals(...)), in the same order, then they are considered equal.
    /// </summary>
    /// <typeparam name="T">The type in the collection. This should be a value type or an immutable class</typeparam>
    public abstract class LikeEnumerableType<T> : LikeType<IEnumerable<T>>, IEnumerable<T>
    {
        public int Count { get { return _items.Count; } }
        public T this[int i] { get { return _items[i]; } }

        private readonly List<T> _items;
        private readonly ToStringStrategy _toStringStrategy;

        /// <summary>
        /// Tip: Name the public constructor parameter 'value' to allow serializers to automatically map the public property 'Value' to the constructor value.
        /// </summary>
        /// <param name="value">The enumerable will be evaluated when given and the results stored in this object.</param>
        /// <param name="toStringStrategy">The desired ToString behavior.</param>
        /// <param name="isNullAllowed">If false and null is given for 'value', then an ArgumentNullException will be thrown. If true and null is given, then it will be treated like an empty enumerable.</param>
        protected LikeEnumerableType(IEnumerable<T> value, ToStringStrategy toStringStrategy = ToStringStrategy.CountOnly, bool isNullAllowed = false)
            : base(value == null ? (isNullAllowed ? new List<T>(0) : null) : new List<T>(value))
        {
            _items = (List<T>)Value;
            _toStringStrategy = toStringStrategy;
        }

        protected override bool AreValuesEqual(IEnumerable<T> value1, IEnumerable<T> value2)
        {
            var value1Enum = value1.GetEnumerator();
            var value2Enum = value2.GetEnumerator();

            while (value1Enum.MoveNext())
            {
                if (!value2Enum.MoveNext())
                    return false; // item count mismatch
                if (value1Enum.Current == null)
                {
                    if (value2Enum.Current != null)
                        return false; // one item null and the other not
                }
                else if (!value1Enum.Current.Equals(value2Enum.Current))
                    return false; // items not equal
            }
            return !value2Enum.MoveNext(); // item count mismatch if MoveNext() is true
        }

        protected override int GetHashFromValue(IEnumerable<T> value)
        {
            var hash = Type.GetHashCode();
            foreach (var v in value)
                hash = hash + 9001 * (v == null ? 8999 : v.GetHashCode());
            return hash;
        }

        public override string ToString()
        {
            var nameAndCount = string.Format("{0}[{1}]", Type.Name, Count);

            if (_toStringStrategy == ToStringStrategy.CountOnly)
                return nameAndCount;

            var seperator = _toStringStrategy == ToStringStrategy.AllValuesMultiLine ? string.Format("{0}  ", Environment.NewLine) : " ";

            return string.Format("{0} = {{{1}{2} }}",
                nameAndCount,
                seperator,
                string.Join(string.Format(",{0}", seperator), _items.ConvertAll(i => i == null ? "null" : string.Format("'{0}'", i.ToString())).ToArray()));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}