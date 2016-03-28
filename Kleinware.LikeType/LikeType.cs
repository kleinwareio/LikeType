using System;

namespace Kleinware.LikeType
{
    /// <summary>
    /// Provides typedef-like behavior for type T. Allows the elevating of simple types (like int or string) for use in domain models, while still behaving like the underlying type.
    /// 
    /// For example, Age : LikeType&lt;int&gt; { ... } will allow you to pass around an Age while getting all of the behavior of an int. 
    /// 
    /// The type T should be immutable.
    /// </summary>
    /// <typeparam name="T">The type to behave like. This type should be immutable.</typeparam>
    public abstract class LikeType<T>
    {
        public T Value { get { return _value; } }

        protected readonly Type Type;

        private readonly T _value;
        private readonly bool _isValueNull;

        /// <summary>
        /// Tip: Name the public constructor parameter 'value' to allow serializers to automatically map the public property 'Value' to the constructor value.
        /// </summary>
        protected LikeType(T value, bool isNullAllowed = false)
        {
            _isValueNull = ReferenceEquals(value, null);
            if (_isValueNull && !isNullAllowed)
                throw new ArgumentNullException("value");

            _value = value;
            Type = GetType();
        }

        public static bool operator ==(LikeType<T> m1, LikeType<T> m2)
        {
            if (ReferenceEquals(m1, m2))
                return true;
            if (ReferenceEquals(null, m1))
                return false;
            if (ReferenceEquals(null, m2))
                return false;

            return m1.Equals(m2);
        }

        public static bool operator !=(LikeType<T> m1, LikeType<T> m2)
        {
            return !(m1 == m2);
        }

        public static implicit operator T(LikeType<T> t)
        {
            return t.Value;
        }

        public sealed override bool Equals(object obj)
        {
            var other = obj as LikeType<T>;

            if (other == null)
                return false;

            if (Type != other.Type)
                return false;

            if (_isValueNull || other._isValueNull)
                return _isValueNull == other._isValueNull;

            return AreValuesEqual(_value, other.Value);
        }

        /// <summary>
        /// Override to provide custom Equals checking. Neither 'value1' nor 'value2' will be null.
        /// </summary>
        protected virtual bool AreValuesEqual(T value1, T value2)
        {
            return value1.Equals(value2);
        }

        public sealed override int GetHashCode()
        {
            if (_isValueNull)
                return 0;

            return GetHashFromValue(_value);
        }

        /// <summary>
        /// Override to provide custom HashCode generation. 'value' will not be null.
        /// </summary>
        protected virtual int GetHashFromValue(T value)
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            if (_isValueNull)
                return string.Empty;

            return _value.ToString();
        }
    }
}