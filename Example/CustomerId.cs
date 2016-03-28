using Kleinware.LikeType;

namespace Example
{
    public class CustomerId : LikeType<string>
    {
        public CustomerId(string value) : base(value) { }
    }
}