[![Build status](https://ci.appveyor.com/api/projects/status/3sp5o4fuhspo616s/branch/master?svg=true)](https://ci.appveyor.com/project/mattklein999/liketype/branch/master)
# LikeType

Provides 'typedef' like behavior to simple C# classes.

The syntax to use is:

```C#
class MyClass : LikeType<BackingType>
{
    public MyClass(BackingType value) : base(value) { }
}
```

Example:

```C#
class CustomerId : LikeType<string>
{
    public CustomerId(string id) : base(id) { }
}

public class OrderId : LikeType<int>
{
    public OrderId(int value) : base(value) { }
}
```

If the backing type you want to use is a group of values (like a list, array, or similar), then use `LikeEnumerableType<T>`:

```C#
class CustomerList : LikeEnumerableType<CustomerId>
{
    public CustomerList(IEnumerable<CustomerId> value) : base(value) { }    
}
``` 

The backing field is `readonly` and the backing type should be immutable.
    
## Requirements

`LikeType` is based on .Net Standard 2.0 and supports projects using .Net 4.6.1 and above or .Net Core 2.0 and above.  The test and example projects in this repository use .Net 8.0.

## Behavior

The code for these examples are found under the `Example` project in the `Basics.cs` file.

### LikeType Behavior

```C#
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
```

### LikeEnumerableType Behavior 

```C#
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
```

## Examples

### HashSet example

`LikeType` values can be used in HashSets for fast, non-sequential lookup

```C#
public class SomeInt : LikeType<int>
{
    public SomeInt(int value) : base(value) { }
}

[TestClass]
public class HashSetExample
{
    [TestMethod]
    public void Contains_WhenInstanceAdded_ReturnsTrueWhenTestedWithDifferentInstanceHavingSameValue()
    {
        var myInt = new SomeInt(42);
        var myIntCopy = new SomeInt(42);
        var otherInt = new SomeInt(4111);

        Assert.IsTrue(myInt == myIntCopy);
        Assert.IsFalse(myInt.Equals(otherInt));

        var mySet = new HashSet<SomeInt>();
        mySet.Add(myInt);

        Assert.IsTrue(mySet.Contains(myIntCopy));
    }
}
```

## Order Service Example

Lets say that you have an order printing service that takes in a `CustomerId` and prints `Order`s for that customer. You want to test the interaction between the printing service, the order service, and the printer.

```C#
public interface IOrderService
{
        Orders GetOrders(CustomerId customerId);
}

public interface IOrderPrinter
{
    void PrintOrders(Orders orderIds);
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
        var orders = fixture.Create<Orders>();
        //when GetOrders() called with value equaling customerId, then return orderIds
        orderServiceMock.GetOrders(customerId).Returns(orders);
        // "System Under Test" (sut)
        var sut = new OrderPrintingService(orderServiceMock, orderPrinterMock);

        // Act
        sut.PrintOrders(customerId);

        // Assert
        orderPrinterMock.Received(1).PrintOrders(orders);
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
```