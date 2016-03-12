[![Build status](https://ci.appveyor.com/api/projects/status/3sp5o4fuhspo616s/branch/master?svg=true)](https://ci.appveyor.com/project/mattklein999/liketype/branch/master)
# LikeType

Provides 'typedef' like behavior to simple C# classes.

The syntax to use is:

    class MyClass : LikeType<BackingType>
    {
        public MyClass(BackingType value) : base(value) { }
    }

Example:

    class CustomerId : LikeType<string>
    {
        public CustomerId(string id) : base(id) { }
    }

    
## Behavior

Here is how the type will behave:

    void ShowTypeBehavior()
    {
        var customerId = new CustomerId("cust-001"); // create instance with given backing value
        string custIdValue = customerId; // implicit cast from class to backing type, sets 'custIdValue' to "cust-001"
        
        var otherCustomerId = new CustomerId("cust-002");
        var areEqual = customerId == otherCustomerId; // false
        var areNotEqual = customerId != otherCustomerId; // true
        var areEqualUsingMethod = customerId.Equals(otherCustomerId); // false
        
        var customerIdCopy = new CustomerId("cust-001"); // create separate instance with same backing value
        var isCopyEqual = customerId == customerIdCopy; // true. Instances are considered equal if their backing values are equal.
    }

The backing field is immutable and your class should probably also be immutable. If you add additional properties to you class, then make sure to override the `.Equals()` method and `.GetHashCode()` method to first delegate to the base class.

## Example

Lets say that you have an order printing service that takes in a `CustomerId` and prints `Order`s for that customer. You want to test the interaction between the printing sevice, the order service, and the printer.

    using System.Collections.Generic;
    using System.Linq;
    using Kleinware.LikeType;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using Ploeh.AutoFixture;

    namespace Example
    {
        public class CustomerId : LikeType<string>
        {
            public CustomerId(string value) : base(value) { }
        }

        public class OrderId : LikeType<int>
        {
            public OrderId(int value) : base(value) { }
        }

        public interface IOrderService
        {
            IEnumerable<OrderId> GetOrders(CustomerId customerId);
        }

        public interface IOrderPrinter
        {
            void PrintOrders(IEnumerable<OrderId> orderIds);
        }

        [TestClass]
        public class OrderPrintingServiceTests
        {
            [TestMethod]
            public void PrintOrders_GivenCustomerId_ThenGetsOrderIdsAndSendsToOrderPrinter()
            {
                // Arrange
                // fixture creates test data for us
                var fixture = new Ploeh.AutoFixture.Fixture();
                // Substutite creates mock objects for us to interact with
                var orderServiceMock = NSubstitute.Substitute.For<IOrderService>();
                var orderPrinterMock = NSubstitute.Substitute.For<IOrderPrinter>();
                // test data
                var customerId = fixture.Create<CustomerId>();
                var orderIds = fixture.CreateMany<OrderId>().ToList();
                //when GetOrders() called with value equaling customerId, then return orderIds
                orderServiceMock.GetOrders(customerId).Returns(orderIds); 
                // "System Under Test" (sut)
                var sut = new OrderPrintingService(orderServiceMock, orderPrinterMock); 

                // Act
                sut.PrintOrders(customerId);

                // Assert
                orderPrinterMock.Received(1).PrintOrders(orderIds);
            }
        }

        public class OrderPrintingService
        {
            private readonly IOrderService _orderService;
            private readonly IOrderPrinter _orderPrinter;

            public OrderPrintingService(IOrderService orderService, IOrderPrinter orderPrinter)
            {
                _orderService = orderService;
                _orderPrinter = orderPrinter;
            }

            public void PrintOrders(CustomerId customerId)
            {
                var orderIds = _orderService.GetOrders(customerId);
                _orderPrinter.PrintOrders(orderIds);
            }
        }
    }
