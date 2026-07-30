using System.Collections.Generic;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CustomerOrdersWithItemsSpecification
{
    private readonly string _buyerId = "TestBuyerId";
    private Address _shipToAddress = new Address("Street", "City", "OH", "US", "11111");

    [Fact]
    public void ReturnsOrderWithOrderedItem()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CustomerOrdersWithItemsSpecification(_buyerId);

        var result = spec.Evaluate(GetTestCollection()).FirstOrDefault();

        Assert.NotNull(result);
        var orderItems = Assert.Single(result.OrderItems);
        Assert.NotNull(orderItems.ItemOrdered);
    }

    [Fact]
    public void ReturnsAllOrderWithAllOrderedItem()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CustomerOrdersWithItemsSpecification(_buyerId);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.Collection(result,
            first =>
            {
                var orderItem = Assert.Single(first.OrderItems);
                Assert.NotNull(orderItem.ItemOrdered);
            },
            second => Assert.Collection(second.OrderItems,
                orderItem => Assert.NotNull(orderItem.ItemOrdered),
                orderItem => Assert.NotNull(orderItem.ItemOrdered)));
    }

    public List<Order> GetTestCollection()
    {
        var ordersList = new List<Order>();

        ordersList.Add(new Order(_buyerId, _shipToAddress,
            new List<OrderItem>
            {
                    new OrderItem(new CatalogItemOrdered(1, "Product1", "testurl"), 10.50m, 1)
            }));
        ordersList.Add(new Order(_buyerId, _shipToAddress,
            new List<OrderItem>
            {
                    new OrderItem(new CatalogItemOrdered(2, "Product2", "testurl"), 15.50m, 2),
                    new OrderItem(new CatalogItemOrdered(2, "Product3", "testurl"), 20.50m, 1)
            }));

        return ordersList;
    }
}
