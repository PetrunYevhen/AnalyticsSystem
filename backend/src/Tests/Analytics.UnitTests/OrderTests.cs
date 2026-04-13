using Analytics.Domain.Entities.Order;
using Xunit;

namespace Analytics.UnitTests;

public class OrderTests
{
    [Fact]
    public void AddItem_Should_Add_Product_To_Order_Items_List()
    {
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var order = new Order(
            tenantId, 
            customerId, 
            "EXT-001", 
            totalAmount: 150m, // Клієнт заплатив 150
            currency: "USD", 
            orderDate: DateTime.UtcNow,
            orderStatus: OrderStatus.Pending
        );

        order.AddItem(
            productId: "PROD-99",
            name: "Nike Air Max",
            category: "Sneakers",
            quantity: 2,
            price: 75m,
            cost: 30m
        );

        Assert.Single(order.Items); 
        
        var addedItem = order.Items.First(); 
        
        Assert.Equal("PROD-99", addedItem.ProductExternalId);
        Assert.Equal("Nike Air Max", addedItem.ProductName);
        Assert.Equal(2, addedItem.Quantity);
        Assert.Equal(75m, addedItem.UnitPrice);
    }
}