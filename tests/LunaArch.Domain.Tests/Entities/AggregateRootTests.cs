using Ardalis.GuardClauses;
using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Primitives;
using Shouldly;
using Xunit;

namespace LunaArch.Domain.Tests.Entities;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId From(Guid value) => new(value);
}

public readonly record struct ProductName(string Value)
{
    public static ProductName From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > 200)
        {
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(value));
        }

        return new(value);
    }
}

public readonly record struct Price(decimal Value)
{
    public static Price From(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(value));
        }

        return new(value);
    }
}

/// <summary>
/// Example domain event for product creation.
/// </summary>
public sealed record ProductCreatedEvent(ProductId ProductId, ProductName Name, Price Price) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Example domain event for price changes.
/// </summary>
public sealed record ProductPriceChangedEvent(ProductId ProductId, Price OldPrice, Price NewPrice) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Example aggregate root with domain events.
/// </summary>
public sealed class Product : AggregateRoot<ProductId>
{
    private ProductName _name;
    private Price _price;

    public ProductName Name => _name;
    public Price Price => _price;

    private Product(ProductId id, ProductName name, Price price)
    {
        Id = id;
        _name = Guard.Against.Null(name);
        _price = Guard.Against.Null(price);
    }

    public static Product Create(ProductId id, ProductName name, Price price)
    {
        var product = new Product(id, name, price);
        product.RaiseDomainEvent(new ProductCreatedEvent(id, name, price));
        return product;
    }

    public void ChangePrice(Price newPrice)
    {
        Guard.Against.Null(newPrice);

        if (newPrice.Value == _price.Value)
        {
            return;
        }

        var oldPrice = _price;
        _price = newPrice;
        RaiseDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
    }
}

public class AggregateRootTests
{
    [Fact]
    public void Create_ValidData_CreatesProduct()
    {
        var id = ProductId.From(Guid.NewGuid());
        var name = ProductName.From("Test Product");
        var price = Price.From(99.99m);

        var product = Product.Create(id, name, price);

        product.Id.ShouldBe(id);
        product.Name.ShouldBe(name);
        product.Price.ShouldBe(price);
    }

    [Fact]
    public void Create_RaisesProductCreatedEvent()
    {
        var id = ProductId.From(Guid.NewGuid());
        var name = ProductName.From("Test Product");
        var price = Price.From(99.99m);

        var product = Product.Create(id, name, price);

        var events = product.DomainEvents;
        events.ShouldNotBeEmpty();
        events.ShouldHaveSingleItem();

        var @event = events.First();
        @event.ShouldBeOfType<ProductCreatedEvent>();
        
        var createdEvent = (ProductCreatedEvent)@event;
        createdEvent.ProductId.ShouldBe(id);
        createdEvent.Name.ShouldBe(name);
        createdEvent.Price.ShouldBe(price);
    }

    [Fact]
    public void ChangePrice_DifferentPrice_UpdatesPriceAndRaisesEvent()
    {
        var product = Product.Create(
            ProductId.From(Guid.NewGuid()),
            ProductName.From("Test Product"),
            Price.From(99.99m));

        product.ClearDomainEvents();

        var newPrice = Price.From(149.99m);
        product.ChangePrice(newPrice);

        product.Price.ShouldBe(newPrice);
        
        var events = product.DomainEvents;
        events.Count.ShouldBe(1);

        var @event = events.First();
        @event.ShouldBeOfType<ProductPriceChangedEvent>();
        
        var changedEvent = (ProductPriceChangedEvent)@event;
        changedEvent.OldPrice.Value.ShouldBe(99.99m);
        changedEvent.NewPrice.ShouldBe(newPrice);
    }

    [Fact]
    public void ChangePrice_SamePrice_DoesNotRaiseEvent()
    {
        var product = Product.Create(
            ProductId.From(Guid.NewGuid()),
            ProductName.From("Test Product"),
            Price.From(99.99m));

        product.ClearDomainEvents();

        product.ChangePrice(Price.From(99.99m));

        product.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var product = Product.Create(
            ProductId.From(Guid.NewGuid()),
            ProductName.From("Test Product"),
            Price.From(99.99m));

        product.DomainEvents.ShouldNotBeEmpty();

        product.ClearDomainEvents();

        product.DomainEvents.ShouldBeEmpty();
    }
}
