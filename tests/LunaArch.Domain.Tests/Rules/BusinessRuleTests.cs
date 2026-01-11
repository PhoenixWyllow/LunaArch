using LunaArch.Domain.Rules;
using Shouldly;
using Xunit;

namespace LunaArch.Domain.Tests.Rules;

public class BusinessRuleTests
{
    private sealed class AlwaysValidRule : IBusinessRule
    {
        public bool IsBroken() => false;
        public string Message => "This rule is never broken";
        public string Code => "ALWAYS_VALID";
    }

    private sealed class AlwaysBrokenRule : IBusinessRule
    {
        public bool IsBroken() => true;
        public string Message => "This rule is always broken";
        public string Code => "ALWAYS_BROKEN";
    }

    private sealed class MaxItemsRule : IBusinessRule
    {
        private readonly int _currentCount;
        private readonly int _maxCount;

        public MaxItemsRule(int currentCount, int maxCount)
        {
            _currentCount = currentCount;
            _maxCount = maxCount;
        }

        public bool IsBroken() => _currentCount > _maxCount;
        public string Message => $"Cannot exceed {_maxCount} items. Current: {_currentCount}";
        public string Code => "MAX_ITEMS_EXCEEDED";
    }

    [Fact]
    public void CheckRule_WhenRuleIsValid_DoesNotThrow()
    {
        var rule = new AlwaysValidRule();

        Should.NotThrow(() => rule.CheckRule());
    }

    [Fact]
    public void CheckRule_WhenRuleIsBroken_ThrowsException()
    {
        var rule = new AlwaysBrokenRule();

        var exception = Should.Throw<BusinessRuleValidationException>(() => rule.CheckRule());
        exception.Message.ShouldContain("This rule is always broken");
        exception.BrokenRule.ShouldBe(rule);
    }

    [Fact]
    public void CheckRule_WithCustomRule_ValidatesCorrectly()
    {
        var validRule = new MaxItemsRule(currentCount: 5, maxCount: 10);
        Should.NotThrow(() => validRule.CheckRule());

        var brokenRule = new MaxItemsRule(currentCount: 15, maxCount: 10);
        var exception = Should.Throw<BusinessRuleValidationException>(() => brokenRule.CheckRule());
        exception.Message.ShouldContain("Cannot exceed 10 items");
    }

    [Fact]
    public void CheckRules_WhenAllRulesValid_DoesNotThrow()
    {
        var rules = new IBusinessRule[]
        {
            new AlwaysValidRule(),
            new MaxItemsRule(5, 10),
        };

        Should.NotThrow(() => rules.CheckRules());
    }

    [Fact]
    public void CheckRules_WhenAnyRuleBroken_ThrowsException()
    {
        var rules = new IBusinessRule[]
        {
            new AlwaysValidRule(),
            new AlwaysBrokenRule(),
            new MaxItemsRule(5, 10),
        };

        var exception = Should.Throw<BusinessRuleValidationException>(() => rules.CheckRules());
        exception.Message.ShouldContain("This rule is always broken");
    }

    [Fact]
    public void CheckRules_StopsAtFirstBrokenRule()
    {
        var firstBroken = new MaxItemsRule(15, 10);
        var secondBroken = new AlwaysBrokenRule();

        var rules = new IBusinessRule[] { firstBroken, secondBroken };

        var exception = Should.Throw<BusinessRuleValidationException>(() => rules.CheckRules());
        exception.Message.ShouldContain("Cannot exceed 10 items");
        exception.Message.ShouldNotContain("This rule is always broken");
    }
}
