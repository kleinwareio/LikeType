using System;

namespace Kleinware.LikeType
{
    public abstract class LikeType<T>
    {
        private readonly T _value;
        public T Value { get { return _value; } }

        private readonly bool _isValueNull;
        private readonly Type _type;

        protected LikeType(T value)
        {
            _value = value;
            _isValueNull = ReferenceEquals(value, null);
            _type = GetType();
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

        public override bool Equals(object obj)
        {
            var other = obj as LikeType<T>;

            if (other == null)
                return false;

            if (_type != other._type)
                return false;

            if (_isValueNull)
                return other._isValueNull;

            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            if (_isValueNull)
                return 0;

            return Value.GetHashCode();
        }

        public override string ToString()
        {
            if (_isValueNull)
                return string.Empty;

            return Value.ToString();
        }
    }
}