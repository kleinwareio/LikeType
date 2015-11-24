# LikeType

Provides 'typedef' like behavior to simple C# classes.

The syntax to use is:

    class MyClass : LikeType<MyClass, BackingType>
    {
        public MyClass(BackingType value) : base(value) { }
    }

Example:

    class CustomerId : LikeType<CustomerId, string>
    {
        public CustomerId(string id) : base(id) { }
    }

    
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

## Weird Syntax

The `class MyClass : LikeType<MyClass,...>` syntax is needed to prevent compile-time comparisons of two different types that may have the same backing type. For example, given the `CustomerId` class above, and this class:

    class PhoneNumber : LikeType<PhoneNumber, string> { public PhoneNumber(string number) : base(number) {} }
    
the following will not compile:

    void CantCompareTwoDifferentTypesThatHaveSameBacking(CustomerId, customerId, PhoneNumber phoneNumber)
    {
        var areEqual = customerId == phoneNumber; // does not compile
    }