using Kleinware.LikeType;

namespace Example
{
    public class OrderId : LikeType<int>
    {
        public OrderId(int value) : base(value) { }
    }
}