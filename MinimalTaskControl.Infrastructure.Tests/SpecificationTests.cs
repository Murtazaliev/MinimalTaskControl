using MinimalTaskControl.Infrastructure.Specifications;
using System.Linq.Expressions;

namespace MinimalTaskControl.Infrastructure.Tests;

public class SpecificationTests
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    [Fact]
    public void Constructor_WithCriteria_SetsCriteriaCorrectly()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> criteria = x => x.Id > 0;

        // Act
        var spec = new Specification<TestEntity>(criteria);

        // Assert
        Assert.NotNull(spec.Criteria);
        Assert.Equal(criteria.ToString(), spec.Criteria.ToString());
    }

    [Fact]
    public void AddInclude_WithExpression_AddsToIncludesList()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> include = x => x.Name;

        // Act
        spec.AddInclude(include);

        // Assert
        Assert.Single(spec.Includes);
        Assert.Equal(include.ToString(), spec.Includes[0].ToString());
    }

    [Fact]
    public void AddInclude_WithString_AddsToIncludeStringsList()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        var includeString = "TestInclude";

        // Act
        spec.AddInclude(includeString);

        // Assert
        Assert.Single(spec.IncludeStrings);
        Assert.Equal(includeString, spec.IncludeStrings[0]);
    }

    [Fact]
    public void AddSelect_WithExpression_SetsSelectProperty()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> select = x => new { x.Id, x.Name };

        // Act
        spec.AddSelect(select);

        // Assert
        Assert.NotNull(spec.Select);
        Assert.Equal(select.ToString(), spec.Select.ToString());
    }

    [Fact]
    public void AddOrderBy_WithExpression_SetsOrderByProperty()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> orderBy = x => x.CreatedAt;

        // Act
        spec.AddOrderBy(orderBy);

        // Assert
        Assert.NotNull(spec.OrderBy);
        Assert.Equal(orderBy.ToString(), spec.OrderBy.ToString());
    }

    [Fact]
    public void AddOrderByDescending_WithExpression_SetsOrderByDescendingProperty()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> orderByDesc = x => x.CreatedAt;

        // Act
        spec.AddOrderByDescending(orderByDesc);

        // Assert
        Assert.NotNull(spec.OrderByDescending);
        Assert.Equal(orderByDesc.ToString(), spec.OrderByDescending.ToString());
    }

    [Fact]
    public void ApplyPaging_WithSkipAndTake_SetsPagingProperties()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        int skip = 10;
        int take = 5;

        // Act
        spec.ApplyPaging(skip, take);

        // Assert
        Assert.Equal(skip, spec.Skip);
        Assert.Equal(take, spec.Take);
    }

    [Fact]
    public void ApplyPaging_WithZeroValues_SetsPagingProperties()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);

        // Act
        spec.ApplyPaging(0, 0);

        // Assert
        Assert.Equal(0, spec.Skip);
        Assert.Equal(0, spec.Take);
    }

    [Fact]
    public void MultipleIncludes_AreAddedCorrectly()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> include1 = x => x.Name;
        Expression<Func<TestEntity, object>> include2 = x => x.CreatedAt;

        // Act
        spec.AddInclude(include1);
        spec.AddInclude(include2);

        // Assert
        Assert.Equal(2, spec.Includes.Count);
        Assert.Equal(include1.ToString(), spec.Includes[0].ToString());
        Assert.Equal(include2.ToString(), spec.Includes[1].ToString());
    }

    [Fact]
    public void MultipleIncludeStrings_AreAddedCorrectly()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        var include1 = "Include1";
        var include2 = "Include2";

        // Act
        spec.AddInclude(include1);
        spec.AddInclude(include2);

        // Assert
        Assert.Equal(2, spec.IncludeStrings.Count);
        Assert.Equal(include1, spec.IncludeStrings[0]);
        Assert.Equal(include2, spec.IncludeStrings[1]);
    }

    [Fact]
    public void OrderBy_OverwritesPreviousValue()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> orderBy1 = x => x.Name;
        Expression<Func<TestEntity, object>> orderBy2 = x => x.CreatedAt;

        // Act
        spec.AddOrderBy(orderBy1);
        spec.AddOrderBy(orderBy2);

        // Assert
        Assert.NotNull(spec.OrderBy);
        Assert.Equal(orderBy2.ToString(), spec.OrderBy.ToString());
    }

    [Fact]
    public void OrderByDescending_OverwritesPreviousValue()
    {
        // Arrange
        var spec = new Specification<TestEntity>(x => true);
        Expression<Func<TestEntity, object>> orderByDesc1 = x => x.Name;
        Expression<Func<TestEntity, object>> orderByDesc2 = x => x.CreatedAt;

        // Act
        spec.AddOrderByDescending(orderByDesc1);
        spec.AddOrderByDescending(orderByDesc2);

        // Assert
        Assert.NotNull(spec.OrderByDescending);
        Assert.Equal(orderByDesc2.ToString(), spec.OrderByDescending.ToString());
    }
}
