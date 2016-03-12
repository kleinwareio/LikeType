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

        #region Equals

        [TestMethod]
        public void Equals_WithSelf_ReturnsTrue()
        {
            var result = _main.Equals(_main);

            Assert.IsTrue(result);
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
            var result = _null.Equals(_main);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WithOtherBackingNull_ThenReturnsFalse()
        {
            var result = _main.Equals(_null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WithBothBackingNull_ThenReturnsTrue()
        {
            var otherNull = new StringLikeType(null);

            var result = _null.Equals(otherNull);

            Assert.IsTrue(result);
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
            var result = _null.GetHashCode();

            Assert.AreEqual(0, result);
        }

        #endregion

        #region Implicit Convertion

        [TestMethod]
        public void ImplicitConversion_WhenComparedToValue_ThenAreEqual()
        {
            string test = _main;

            Assert.AreEqual(_main.Value, test);
        }

        [TestMethod]
        public void ImplicitConversion_WithNullBackingValue_ReturnsDefault()
        {
            string value = _null;

            Assert.AreEqual(default(string), value);
        }

        #endregion

        #region ToString

        [TestMethod]
        public void ToString_WithValue_ThenReturnsValueAsString()
        {
            var intType = new IntLikeType(1);

            var str = intType.ToString();

            Assert.AreEqual("1", str);
        }

        [TestMethod]
        public void ToString_WithNullBacking_ThenReturnsEmptyString()
        {
            var result = _null.ToString();

            Assert.AreEqual(string.Empty, result);
        }

        #endregion

        class StringLikeType : LikeType<StringLikeType, string> { public StringLikeType(string value) : base(value) { } }
        class OtherStringLikeType : LikeType<OtherStringLikeType, string> { public OtherStringLikeType(string value) : base(value) { } }
        class IntLikeType : LikeType<IntLikeType, int> { public IntLikeType(int value) : base(value) { } }
    }
}
