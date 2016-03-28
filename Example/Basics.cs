using System;
using Kleinware.LikeType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example
{
    [TestClass]
    public class Basics
    {
        #region LikeType Behavior

        [TestMethod]
        public void ShowLikeTypeBehavior()
        {
            // create instance with given backing value
            var customerId = new CustomerId("cust-001");

            // implicit cast from class to backing type of string, sets 'custIdValue' to "cust-001"
            string custIdValue = customerId;
            Assert.AreEqual("cust-001", custIdValue);

            // CustomerId can be used anywhere string is needed. This makes backwards compatibility or
            // interacting with frameworks that don't know about your types easy
            VerifyStringsEqual("cust-001", customerId); // method defined below as VerifyStringsEqual(string value1, string value2)

            // instances with different backing values are not equal. ==, !=, and .Equals(...) can be used
            var otherCustomerId = new CustomerId("cust-002");
            Assert.IsFalse(customerId == otherCustomerId);
            Assert.IsTrue(customerId != otherCustomerId);
            Assert.IsFalse(customerId.Equals(otherCustomerId));

            // create separate instance with same backing value
            var customerIdCopy = new CustomerId("cust-001");

            // Instances are considered equal if their backing values are equal.
            Assert.AreEqual(customerId, customerIdCopy);
            Assert.IsTrue(customerId == customerIdCopy);
            Assert.IsFalse(customerId != customerIdCopy);
        }

        private void VerifyStringsEqual(string value1, string value2)
        {
            Assert.AreEqual(value1, value2);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsWhenGivenNullValueByDefault()
        {
            new CustomerId(null);
        }

        private class NullIsAllowedType : LikeType<string>
        {
            // note the second parameter to base class is true to allow null values. Default is false.
            public NullIsAllowedType(string value) : base(value, true) { }
        }

        [TestMethod]
        public void NullCanBeAllowed()
        {
            var nullInstance = new NullIsAllowedType(null);
            var otherNullInstance = new NullIsAllowedType(null);
            var nonNullInstance = new NullIsAllowedType("not null");

            Assert.AreEqual(nullInstance, otherNullInstance);
            Assert.AreNotEqual(nullInstance, nonNullInstance);
            Assert.AreEqual(0, nullInstance.GetHashCode());
        }

        #endregion

        #region LikeEnumerableType Behavior

        [TestMethod]
        public void ShowLikeEnumberableTypeBehavior()
        {
            var order1 = new OrderId(1);
            var order2 = new OrderId(2);
            var order3 = new OrderId(3);
            var firstOrderGroup = new Orders(order1, order2, order3);
            var copyOfFirstOrderGroup = new Orders(order1, order2, order3);

            // instances are equal if they contain the same items in the same order
            Assert.AreEqual(firstOrderGroup, copyOfFirstOrderGroup);
            Assert.IsTrue(firstOrderGroup == copyOfFirstOrderGroup);

            // different order or different count will cause the instances to not be equal
            var wrongOrder = new Orders(order3, order2, order1);
            Assert.AreNotEqual(firstOrderGroup, wrongOrder);

            // The default ToString behavior will show the name of the type and the count of items
            Assert.AreEqual("Orders[3]", firstOrderGroup.ToString());
        }

        private class ToStringOrdersExample : LikeEnumerableType<OrderId>
        {
            public ToStringOrdersExample(ToStringStrategy toStringStrategy, params OrderId[] value) : base(value, toStringStrategy) { }
        }

        [TestMethod]
        public void ShowLikeEnumerableToString()
        {
            var data = new[] { new OrderId(1), new OrderId(2), new OrderId(3) };

            // The default ToString strategy will print the type name and the number of elements
            var countOnlyOrders = new ToStringOrdersExample(ToStringStrategy.CountOnly, data);
            Assert.AreEqual("ToStringOrdersExample[3]", countOnlyOrders.ToString());

            // There is also two strategies to show all items in the instance, using each item's .ToString() method.
            // the first, shown here, presents all items without an Environment.NewLine
            // The second, not shown, inserts an Environment.NewLine between each item
            var descriptiveOrders = new ToStringOrdersExample(ToStringStrategy.AllValuesSingleLine, data);
            Assert.AreEqual("ToStringOrdersExample[3] = { '1', '2', '3' }", descriptiveOrders.ToString());
        }

        #endregion

    }
}