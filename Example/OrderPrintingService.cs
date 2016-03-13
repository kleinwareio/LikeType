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
