# LikeType

Provides 'typedef' like behavior to simple C# classes.

The syntax to use is:
    class *MyClass* : LikeType<*MyClass*,*BackingType*>
    {
        public *MyClass*(*BackingType* value) : base(value) { }
    }

Example:

    class CustomerId : LikeType<CustomerId, string>
    {
        public CustomerId(string id) : base(id) { }
    }

Here is how the type will behave:

    void ShowTypeBehavior()
    {
        var customerId = new CustomerId("cust-001");
        string custIdValue = customerId; // sets 'custIdValue' to 'cust-001'
        
        var otherCustomerId = new CustomerId("cust-002");
        var areEqual = customerId == otherCustomerId; // false
        var areNotEqual = customerId != otherCustomerId; // true
        
        var customerIdCopy = new CustomerId("cust-001");
        var isCopyEqual = customerId == customerIdCopy; // true
    }


The `LikeType<*MyClass*,...>` syntax is needed to prevent compile-time comparisons of two different types that may have the same backing type. For example, given the `CustomerId` class above, and this class:
    class PhoneNumber : LikeType<PhoneNumber, string>
the following will not compile:

    void CantCompareTwoDifferentTypesThatHaveSameBacking(CustomerId, customerId, PhoneNumber phoneNumber)
    {
        var areEqual = customerId == phoneNumber; // doesn't compile
    }