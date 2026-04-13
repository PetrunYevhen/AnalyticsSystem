using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Order;

namespace Application.Tests;

public record MockEcommerceData(
    List<Customer> Customers, 
    List<Order> Orders, 
    List<MarketingExpense> MarketingExpenses
);