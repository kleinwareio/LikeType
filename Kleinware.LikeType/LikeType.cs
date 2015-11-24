namespace Kleinware.LikeType
{
    public abstract class LikeType<TThis, TTarget>
        where TThis : LikeType<TThis, TTarget>
    {
        private readonly TTarget _value;
        public TTarget Value { get { return _value; } }

        protected LikeType(TTarget value)
        {
            _value = value;
        }

        public static bool operator ==(LikeType<TThis, TTarget> m1, LikeType<TThis, TTarget> m2)
        {
            if (ReferenceEquals(m1, m2))
                return true;
            if (ReferenceEquals(null, m1))
                return false;
            if (ReferenceEquals(null, m2))
                return false;

            return m1.Equals(m2);
        }

        public static bool operator !=(LikeType<TThis, TTarget> m1, LikeType<TThis, TTarget> m2)
        {
            return !(m1 == m2);
        }

        public static implicit operator TTarget(LikeType<TThis, TTarget> t)
        {
            return t.Value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TThis;

            if (other == null)
                return false;

            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}