using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Order;
using Bogus;

namespace Application.Tests;

public static class MockDataGenerator
{
public static MockEcommerceData Generate(Guid tenantId, int customerCount = 100)
    {
        var channels = new[] { "Facebook Ads", "Google Ads", "Organic", "TikTok Ads" };
        var categories = new[] { "Sneakers", "T-Shirts", "Accessories", "Hoodies" };
        var currency = "USD";

        var customerFaker = new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                tenantId: tenantId,
                externalId: f.Random.AlphaNumeric(10).ToUpper(),
                name: f.Person.UserName,
                phoneNumber: f.Phone.PhoneNumber(),
                email: f.Person.Email,
                registrationDate: f.Date.Past(1).ToUniversalTime(), 
                acquisitionChannel: f.PickRandom(channels),
                acquisitionCampaign: f.Commerce.ProductName() 
            ));

        var customers = customerFaker.Generate(customerCount).ToList();
        var orders = new List<Order>();

        var itemFaker = new Faker(); 

        foreach (var customer in customers)
        {
            var orderCount = itemFaker.Random.Int(1, 5);

            for (int i = 0; i < orderCount; i++)
            {
                var orderDate = itemFaker.Date.Between(customer.RegistrationDate, DateTime.UtcNow).ToUniversalTime();
                
                var itemsCount = itemFaker.Random.Int(1, 3);
                var tempItems = new List<(string id, string name, string cat, int qty, decimal price, decimal cost)>();
                decimal calculatedTotal = 0;

                for (int j = 0; j < itemsCount; j++)
                {
                    var price = Math.Round(itemFaker.Random.Decimal(50, 200), 2);
                    var cost = Math.Round(price * 0.4m, 2); // Собівартість - 40% від ціни
                    var qty = itemFaker.Random.Int(1, 2);
                    
                    calculatedTotal += (price * qty);
                    tempItems.Add((itemFaker.Random.AlphaNumeric(8), itemFaker.Commerce.ProductName(), itemFaker.PickRandom(categories), qty, price, cost));
                }

                var order = new Order(
                    tenantId: tenantId,
                    customerId: customer.Id,
                    externalOrderId: itemFaker.Random.AlphaNumeric(10).ToUpper(),
                    totalAmount: calculatedTotal,
                    currency: currency,
                    orderDate: orderDate
                );

                foreach (var item in tempItems)
                {
                    order.AddItem(item.id, item.name, item.cat, item.qty, item.price, item.cost);
                }

                orders.Add(order);
            }
        }

        var expenses = new List<MarketingExpense>();
        var expenseFaker = new Faker();

        for (int month = 0; month < 12; month++)
        {
            var date = DateTime.UtcNow.AddMonths(-month);
            
            foreach (var channel in channels.Where(c => c != "Organic")) // На Organic гроші не витрачаємо
            {
                var expense = new MarketingExpense(
                    tenantId: tenantId,
                    date: new DateTime(date.Year, date.Month, 1).ToUniversalTime(), // Перше число місяця
                    amount: Math.Round(expenseFaker.Random.Decimal(500, 2000), 2),
                    currency: currency,
                    adSource: channel,
                    campaignName: $"Promo_{date:MMM_yyyy}"
                );
                expenses.Add(expense);
            }
        }

        return new MockEcommerceData(customers, orders, expenses);
    }
}

