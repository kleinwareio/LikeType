using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kleinware.LikeType
{
    [TestClass]
    public class LikeEnumerableTypeTests
    {
        private Foo _foo1;
        private Foo _foo2;
        private FooEnumerable _list;

        [TestInitialize]
        public void Setup()
        {
            _foo1 = new Foo(11);
            _foo2 = new Foo(22);
            _list = MakeFooEnumerable(_foo1, _foo2);
        }

        #region Constructor

        [TestMethod]
        public void Constructor_WhenGivenEnumerable_MakesCopyOfEnumerable()
        {
            var data = new[] { _foo1, _foo2 }.ToList();
            var sut = new FooEnumerable(data);

            data.Add(new Foo(42));

            Assert.IsFalse(sut.SequenceEqual(data));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WhenIsNullAllowedIsFalseAndGivenNull_ThrowsArgumentNullException()
        {
            new NullNotAllowedFooEnumerable(null);
        }

        #endregion

        #region Equals

        [TestMethod]
        public void Equals_WithTwoNullLists_ReturnsTrue()
        {
            TestEqualsInBothDirections(true, new FooEnumerable(null), new FooEnumerable(null));
        }

        [TestMethod]
        public void Equals_WithOneListNull_ReturnsFalse()
        {
            TestEqualsInBothDirections(false, _list, new FooEnumerable(null));
        }

        [TestMethod]
        public void Equals_WithMissingElement_ReturnsFalse()
        {
            TestEqualsInBothDirections(false, _list, MakeFooEnumerable(_foo1));
        }

        [TestMethod]
        public void Equals_WithSameElementsInDifferentOrder_ReturnsFalse()
        {
            TestEqualsInBothDirections(false, _list, MakeFooEnumerable(_foo2, _foo1));
        }

        [TestMethod]
        public void Equals_WithSameElementsInSameOrder_ReturnsTrue()
        {
            TestEqualsInBothDirections(true, _list, MakeFooEnumerable(_foo1, _foo2));
        }

        [TestMethod]
        public void Equals_WithMismatchedNullItem_ReturnsFalse()
        {
            TestEqualsInBothDirections(false, _list, MakeFooEnumerable(_foo1, null));
        }

        [TestMethod]
        public void Equals_WithMatchingNullItems_ReturnsTrue()
        {
            TestEqualsInBothDirections(true, MakeFooEnumerable(_foo1, null, _foo2), MakeFooEnumerable(_foo1, null, _foo2));
        }

        [TestMethod]
        public void Equals_WithDifferentTypeWithSameBackingAndSameValues_ReturnsFalse()
        {
            TestEqualsInBothDirections(false, _list, new OtherFooEnumerable(_foo1, _foo2));
        }

        private static void TestEqualsInBothDirections(bool expected, object item1, object item2)
        {
            Assert.AreEqual(expected, item1.Equals(item2), "item1.Equals(item2)");
            Assert.AreEqual(expected, item2.Equals(item1), "item2.Equals(item1)");
        }

        #endregion

        #region EqualsOperator

        [TestMethod]
        public void EqualsOperator_WhenNotEqual_ReturnsFalse()
        {
            Assert.IsFalse(_list == MakeFooEnumerable(_foo2, _foo1));
        }

        [TestMethod]
        public void EqualsOperator_WhenEqual_ReturnsTrue()
        {
            Assert.IsTrue(_list == MakeFooEnumerable(_foo1, _foo2));
        }

        [TestMethod]
        public void EqualsOperator_WhenSameValuesButDifferentType_ReturnsFalse()
        {
            Assert.IsFalse(_list == new OtherFooEnumerable(_foo1, _foo2));
        }

        #endregion

        #region GetHashCode

        [TestMethod]
        public void GetHashCode_WithElementsNull_EqualsHashCodeWithNoItems()
        {
            Assert.AreEqual(MakeFooEnumerable().GetHashCode(), MakeFooEnumerable(null).GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_WithSameElement_AreEqual()
        {
            Assert.AreEqual(_list.GetHashCode(), MakeFooEnumerable(_foo1, _foo2).GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_WithNoItems_ReturnsNonZero()
        {
            Assert.AreNotEqual(0, MakeFooEnumerable().GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_WithSingleNullItem_ReturnsNonZero()
        {
            Assert.AreNotEqual(0, new FooEnumerable(new Foo[] { null }).GetHashCode());
        }

        #endregion

        #region GetEnumerator

        [TestMethod]
        public void GetEnumerator_WhenIteratedOver_ReturnsSameEnumerationAsUnderlyingType()
        {
            Assert.IsTrue(_list.SequenceEqual(new[] { _foo1, _foo2 }));
        }

        #endregion

        #region ToString

        [TestMethod]
        public void ToString_GivenCountOnlyStrategy_ContainsTypeNameAndCount()
        {
            var result = MakeToStringFooEnumerable(ToStringStrategy.CountOnly, _foo1, _foo2);

            VerifyContainsTypeNameAndCount(result, 2);
            VerifyContainsNewLine(result, 0);
            Assert.IsFalse(result.Contains(_foo1.ToString()), "foo1");
            Assert.IsFalse(result.Contains(_foo2.ToString()), "foo2");
        }

        [TestMethod]
        public void ToString_GivenAllValuesSingleLineStrategy_ContainsAllValuesAndNoLineBreaks()
        {
            var result = MakeToStringFooEnumerable(ToStringStrategy.AllValuesSingleLine, _foo1, _foo2);

            VerifyContainsNewLine(result, 0);
            VerifyContainsString(result, _foo1.ToString(), _foo2.ToString());
            VerifyContainsTypeNameAndCount(result, 2);
        }

        [TestMethod]
        public void ToString_GivenAllValuesSingleLineStrategyAndNullValue_ReturnsString()
        {
            var result = MakeToStringFooEnumerable(ToStringStrategy.AllValuesSingleLine, _foo1, null, _foo2);

            VerifyContainsNewLine(result, 0);
            VerifyContainsString(result, _foo1.ToString(), "null", _foo2.ToString());
            VerifyContainsTypeNameAndCount(result, 2);
        }

        [TestMethod]
        public void ToString_GivenAllValuesMultiLineStrategy_ContainsAllValuesAndNewLines()
        {
            var result = MakeToStringFooEnumerable(ToStringStrategy.AllValuesMultiLine, _foo1, _foo2);

            VerifyContainsNewLine(result, 2);
            VerifyContainsString(result, _foo1.ToString(), _foo2.ToString());
            VerifyContainsTypeNameAndCount(result, 2);
        }

        [TestMethod]
        public void ToString_GivenAllValuesMultiLineStrategyAndNullBacking_LooksSameAsWithEmptyBacking()
        {
            var expected = MakeToStringFooEnumerable(ToStringStrategy.AllValuesMultiLine);

            var actual = MakeToStringFooEnumerable(ToStringStrategy.AllValuesMultiLine, null);

            Assert.AreEqual(expected, actual);
        }

        private static void VerifyContainsString(string result, params string[] items)
        {
            foreach (var item in items)
                Assert.IsTrue(result.Contains(item), item);
        }

        private static void VerifyContainsNewLine(string result, int expected)
        {
            Assert.AreEqual(expected, result.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length - 1, "new line");
        }

        private static void VerifyContainsTypeNameAndCount(string result, int count)
        {
            Assert.IsTrue(result.Contains(typeof(ToStringFooEnumerable).Name), "name");
            Assert.IsTrue(result.Contains(count.ToString()), "count");
        }

        #endregion

        #region Count

        [TestMethod]
        public void Count_WithNullBacking_Returns0()
        {
            Assert.AreEqual(0, MakeFooEnumerable(null).Count);
        }

        [TestMethod]
        public void Count_WithTwoItems_Returns2()
        {
            Assert.AreEqual(2, _list.Count);
        }

        #endregion

        #region Index

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Index_At0WithNoItems_ThrowsArgumentOutOfRangeException()
        {
            var _ = MakeFooEnumerable()[0];
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Index_At0WithNullBacking_ThrowsArgumentOfOfRangeException()
        {
            var _ = MakeFooEnumerable(null)[0];
        }

        [TestMethod]
        public void Index_WithTwoItems_ReturnsCorrectItemAtEachIndex()
        {
            Assert.AreEqual(_foo1, _list[0]);
            Assert.AreEqual(_foo2, _list[1]);
        }

        #endregion

        #region Arrange

        private static FooEnumerable MakeFooEnumerable(params Foo[] foos)
        {
            return new FooEnumerable(foos);
        }

        private static string MakeToStringFooEnumerable(ToStringStrategy strategy, params Foo[] values)
        {
            return new ToStringFooEnumerable(strategy, values).ToString();
        }

        #endregion

        private class Foo : LikeType<int> { public Foo(int value) : base(value) { } }
        private class FooEnumerable : LikeEnumerableType<Foo> { public FooEnumerable(IEnumerable<Foo> value) : base(value, isNullAllowed: true) { } }
        private class OtherFooEnumerable : LikeEnumerableType<Foo> { public OtherFooEnumerable(params Foo[] value) : base(value, isNullAllowed: true) { } }
        private class ToStringFooEnumerable : LikeEnumerableType<Foo> { public ToStringFooEnumerable(ToStringStrategy toStringStrategy, params Foo[] value) : base(value, toStringStrategy, true) { } }
        private class NullNotAllowedFooEnumerable : LikeEnumerableType<Foo> { public NullNotAllowedFooEnumerable(params Foo[] value) : base(value) { } }
    }
}
