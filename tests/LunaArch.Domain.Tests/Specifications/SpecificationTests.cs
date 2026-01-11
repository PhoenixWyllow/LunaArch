using System.Linq.Expressions;
using LunaArch.Domain.Specifications;
using Shouldly;
using Xunit;

namespace LunaArch.Domain.Tests.Specifications;

public class SpecificationTests
{
    private sealed record TestEntity(int Id, string Name, int Age, bool IsActive);

    private sealed class TestSpecification : Specification<TestEntity>
    {
        public void SetCriteria(Expression<Func<TestEntity, bool>> criteria) => AddCriteria(criteria);
        public void SetOrderBy(Expression<Func<TestEntity, object>> orderBy) => ApplyOrderBy(orderBy);
        public void SetOrderByDescending(Expression<Func<TestEntity, object>> orderByDescending) => ApplyOrderByDescending(orderByDescending);
        public void SetInclude(Expression<Func<TestEntity, object>> include) => AddInclude(include);
        public void SetPaging(int skip, int take) => ApplyPaging(skip, take);
    }

    [Fact]
    public void Specification_CanSetCriteria()
    {
        var spec = new TestSpecification();
        spec.SetCriteria(e => e.IsActive);

        spec.Criteria.ShouldNotBeNull();
        
        // Test the compiled expression
        var compiledCriteria = spec.Criteria.Compile();
        compiledCriteria(new TestEntity(1, "Test", 25, true)).ShouldBeTrue();
        compiledCriteria(new TestEntity(2, "Test", 25, false)).ShouldBeFalse();
    }

    [Fact]
    public void Specification_CanSetOrderBy()
    {
        var spec = new TestSpecification();
        spec.SetOrderBy(e => e.Name);

        spec.OrderBy.ShouldNotBeNull();
        spec.OrderByDescending.ShouldBeNull();
    }

    [Fact]
    public void Specification_CanSetOrderByDescending()
    {
        var spec = new TestSpecification();
        spec.SetOrderByDescending(e => e.Age);

        spec.OrderByDescending.ShouldNotBeNull();
        spec.OrderBy.ShouldBeNull();
    }

    [Fact]
    public void Specification_CanSetIncludes()
    {
        var spec = new TestSpecification();
        spec.SetInclude(e => e.Name);
        spec.SetInclude(e => e.Age);

        spec.Includes.ShouldNotBeEmpty();
        spec.Includes.Count.ShouldBe(2);
    }

    [Fact]
    public void Specification_CanSetPaging()
    {
        var spec = new TestSpecification();
        spec.SetPaging(10, 20);

        spec.Skip.ShouldBe(10);
        spec.Take.ShouldBe(20);
    }

    [Fact]
    public void Specification_WithoutPaging_HasNullSkipAndTake()
    {
        var spec = new TestSpecification();

        spec.Skip.ShouldBeNull();
        spec.Take.ShouldBeNull();
    }

    [Fact]
    public void Specification_And_CombinesCriteria()
    {
        var spec1 = new TestSpecification();
        spec1.SetCriteria(e => e.IsActive);

        var spec2 = new TestSpecification();
        spec2.SetCriteria(e => e.Age > 18);

        var combined = spec1.And(spec2);
        var criteria = combined.Criteria!.Compile();

        criteria(new TestEntity(1, "Test", 25, true)).ShouldBeTrue();
        criteria(new TestEntity(2, "Test", 25, false)).ShouldBeFalse();
        criteria(new TestEntity(3, "Test", 15, true)).ShouldBeFalse();
    }

    [Fact]
    public void Specification_Or_CombinesCriteria()
    {
        var spec1 = new TestSpecification();
        spec1.SetCriteria(e => e.Age < 18);

        var spec2 = new TestSpecification();
        spec2.SetCriteria(e => e.Age > 65);

        var combined = spec1.Or(spec2);
        var criteria = combined.Criteria!.Compile();

        criteria(new TestEntity(1, "Test", 15, true)).ShouldBeTrue();
        criteria(new TestEntity(2, "Test", 70, true)).ShouldBeTrue();
        criteria(new TestEntity(3, "Test", 30, true)).ShouldBeFalse();
    }

    [Fact]
    public void Specification_Not_NegatesCriteria()
    {
        var spec = new TestSpecification();
        spec.SetCriteria(e => e.IsActive);

        var negated = spec.Not();
        var criteria = negated.Criteria!.Compile();

        criteria(new TestEntity(1, "Test", 25, true)).ShouldBeFalse();
        criteria(new TestEntity(2, "Test", 25, false)).ShouldBeTrue();
    }
}

