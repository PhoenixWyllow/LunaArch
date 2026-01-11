using LunaArch.Abstractions.Common;
using Shouldly;
using Xunit;

namespace LunaArch.Abstractions.Tests.Common;

public class UnitTests
{
    [Fact]
    public void Value_ReturnsSingletonInstance()
    {
        var unit = Unit.Value;

        unit.ShouldBe(default(Unit));
    }

    [Fact]
    public void Equals_AllInstancesAreEqual()
    {
        var unit1 = Unit.Value;
        var unit2 = new Unit();
        var unit3 = default(Unit);

        unit1.ShouldBe(unit2);
        unit1.ShouldBe(unit3);
        unit2.ShouldBe(unit3);
    }

    [Fact]
    public void EqualityOperator_AllInstancesAreEqual()
    {
        var unit1 = Unit.Value;
        var unit2 = new Unit();

        (unit1 == unit2).ShouldBeTrue();
        (unit1 != unit2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_AllInstancesReturnSameHash()
    {
        var unit1 = Unit.Value;
        var unit2 = new Unit();

        unit1.GetHashCode().ShouldBe(unit2.GetHashCode());
        unit1.GetHashCode().ShouldBe(0);
    }

    [Fact]
    public void CompareTo_AllInstancesAreEqual()
    {
        var unit1 = Unit.Value;
        var unit2 = new Unit();

        unit1.CompareTo(unit2).ShouldBe(0);
    }

    [Fact]
    public void ComparisonOperators_AllReturnExpectedValues()
    {
        var unit1 = Unit.Value;
        var unit2 = new Unit();

        (unit1 < unit2).ShouldBeFalse();
        (unit1 > unit2).ShouldBeFalse();
        (unit1 <= unit2).ShouldBeTrue();
        (unit1 >= unit2).ShouldBeTrue();
    }

    [Fact]
    public void ToString_ReturnsExpectedValue()
    {
        var unit = Unit.Value;

        unit.ToString().ShouldBe("()");
    }

    [Fact]
    public void EqualsObject_WithUnit_ReturnsTrue()
    {
        var unit = Unit.Value;
        object other = new Unit();

        unit.Equals(other).ShouldBeTrue();
    }

    [Fact]
    public void EqualsObject_WithNonUnit_ReturnsFalse()
    {
        var unit = Unit.Value;
        object other = "not a unit";

        unit.Equals(other).ShouldBeFalse();
    }
}
