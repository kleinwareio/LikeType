using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kleinware.LikeType
{
    [TestClass]
    public class LikeTypeTests
    {
        private StringLikeType _main;
        private StringLikeType _same;
        private StringLikeType _different;
        private StringLikeType _null;
        private OtherStringLikeType _other;

        [TestInitialize]
        public void Setup()
        {
            _main = new StringLikeType("1");
            _same = new StringLikeType("1");
            _different = new StringLikeType("nope");
            _null = new StringLikeType(null);
            _other = new OtherStringLikeType("1");
        }

        #region Constructor

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WhenIsNullAllowedFalseAndNullValueGiven_ThrowsArgumentNullException()
        {
            new NullNotAllowedType(null);
        }

        #endregion

        #region Equals

        [TestMethod]
        public void Equals_WithSelf_ReturnsTrue()
        {
            Assert.IsTrue(_main.Equals(_main));
        }

        [TestMethod]
        public void Equals_WithSameDataButDifferentInstance_ReturnsTrue()
        {
            Assert.IsTrue(_main.Equals(_same));
        }

        [TestMethod]
        public void Equals_WithInstanceButDifferentData_ReturnsFalse()
        {
            Assert.IsFalse(_main.Equals(_different));
        }

        [TestMethod]
        public void Equals_WithNull_ReturnsFalse()
        {
            Assert.IsFalse(_main.Equals(null));
        }

        [TestMethod]
        public void Equals_WithSameTypeDefButDifferentType_ReturnsFalse()
        {
            Assert.IsFalse(_main.Equals(_other));
        }

        [TestMethod]
        public void Equals_WithThisBackingNull_ThenReturnsFalse()
        {
            Assert.IsFalse(_null.Equals(_main));
        }

        [TestMethod]
        public void Equals_WithOtherBackingNull_ThenReturnsFalse()
        {
            Assert.IsFalse(_main.Equals(_null));
        }

        [TestMethod]
        public void Equals_WithBothBackingNull_ThenReturnsTrue()
        {
            Assert.IsTrue(_null.Equals(new StringLikeType(null)));
        }

        #endregion

        #region EqualsOperator

        [TestMethod]
        public void EqualsOperator_WithSameValueButDifferentInstance_ReturnsTrue()
        {
            Assert.IsTrue(_same == _main);
        }

        [TestMethod]
        public void EqualsOperator_WithLeftNull_ReturnsFalse()
        {
            Assert.IsFalse(null == _main);
        }

        [TestMethod]
        public void EqualsOperator_WithRightNull_ReturnsFalse()
        {
            Assert.IsFalse(_main == null);
        }

        [TestMethod]
        public void EqualsOperator_WithBothNull_ReturnsTrue()
        {
            Assert.IsTrue(((StringLikeType)null) == ((StringLikeType)null));
        }

        [TestMethod]
        public void EqualsOperator_WithValueEqual_ThenReturnsTrue()
        {
            Assert.IsTrue(_same == _main);
        }

        [TestMethod]
        public void EqualsOperator_WithValueNotEqual_ThenReturnsFalse()
        {
            Assert.IsFalse(_different == _main);
        }

        [TestMethod]
        public void EqualsOperator_WithDifferentTypesButSameBackingValue_ReturnsFalse()
        {
            Assert.IsFalse(_main == _other);
        }

        #endregion

        #region NotEqualOperator

        [TestMethod]
        public void NotEqual_WithLeftNull_ThenReturnsTrue()
        {
            Assert.IsTrue(null != _main);
        }

        [TestMethod]
        public void NotEqual_WithBothNull_ThenReturnsFalse()
        {
            Assert.IsFalse(((StringLikeType)null) != ((StringLikeType)null));
        }

        [TestMethod]
        public void NotEqual_WithValuesEqual_ThenReturnsFalse()
        {
            Assert.IsFalse(_same != _main);
        }

        [TestMethod]
        public void NotEqual_WithValuesDifferent_ThenReturnsTrue()
        {
            Assert.IsTrue(_different != _main);
        }

        [TestMethod]
        public void NotEqual_WithSameBackingValuebutDifferentType_ReturnsTrue()
        {
            Assert.IsTrue(_main != _other);
        }

        #endregion

        #region GetHashCode

        [TestMethod]
        public void GetHashCode_WithTwoSameValuedInstances_AreEqual()
        {
            Assert.AreEqual(_main.GetHashCode(), _same.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_WithNullBackingValue_Returns0()
        {
            Assert.AreEqual(0, _null.GetHashCode());
        }

        #endregion

        #region Implicit Convertion

        [TestMethod]
        public void ImplicitConversion_WhenComparedToValue_ThenAreEqual()
        {
            Assert.AreEqual(_main.Value, _main);
        }

        [TestMethod]
        public void ImplicitConversion_WithNullBackingValue_ReturnsDefault()
        {
            Assert.AreEqual(default(string), _null);
        }

        #endregion

        #region ToString

        [TestMethod]
        public void ToString_WithValue_ThenReturnsValueAsString()
        {
            Assert.AreEqual("1", new IntLikeType(1).ToString());
        }

        [TestMethod]
        public void ToString_WithNullBacking_ThenReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, _null.ToString());
        }

        #endregion

        private class StringLikeType : LikeType<string> { public StringLikeType(string value) : base(value, true) { } }
        private class OtherStringLikeType : LikeType<string> { public OtherStringLikeType(string value) : base(value, true) { } }
        private class IntLikeType : LikeType<int> { public IntLikeType(int value) : base(value) { } }
        private class NullNotAllowedType : LikeType<string> { public NullNotAllowedType(string value) : base(value) { } }
    }
}
