using Kleinware.LikeType;

namespace Example
{
    public class Orders : LikeEnumerableType<OrderId>
    {
        public Orders(params OrderId[] value) : base(value) { }
    }
}