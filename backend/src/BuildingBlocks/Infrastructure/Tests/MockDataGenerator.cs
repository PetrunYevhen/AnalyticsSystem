using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Order;
using Bogus;

namespace Infrastructure.Tests;

public static class MockDataGenerator
{
    public static MockEcommerceData Generate(Guid tenantId, int customerCount = 100)
    {
        // 1. Налаштовуємо локалізацію на українську (або залиш англійську, якщо хочеш)
        var channels = new[] { "Facebook Ads", "Google Ads", "Organic", "TikTok Ads", "Instagram" };
        var categories = new[] { "Взуття", "Одяг", "Аксесуари", "Електроніка" };
        var currency = "USD";

        // Використовуємо один набір даних для всього генератора
        Randomizer.Seed = new Random(42); // Фіксований seed, щоб дані були однаковими при кожному запуску (опціонально)

        var customerFaker = new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                tenantId: tenantId,
                externalId: f.Random.Replace("CUST-#####"), // Більш реальний ID
                fullName: f.Person.FirstName + " " + f.Person.LastName, // "Ivan Petrenko" замість "ivan22"
                phoneNumber: f.Phone.PhoneNumber("+380#########"),
                email: f.Person.Email,
                registrationDate: f.Date.Past().ToUniversalTime(),
                acquisitionChannel: f.PickRandom(channels),
                acquisitionCampaign: f.Random.Double() < 0.7 ? f.Commerce.ProductName() : null));

        var customers = customerFaker.Generate(customerCount).ToList();
        var orders = new List<Order>();
        var itemFaker = new Faker();

        foreach (var customer in customers)
        {
            var chance = itemFaker.Random.Double();

            int orderCount;
            if (chance < 0.60) orderCount = 1;
            else if (chance < 0.80) orderCount = 2;
            else if (chance < 0.90) orderCount = 3;
            else if (chance < 0.95) orderCount = 4;
            else orderCount = 10;

            var orderDates = new List<DateTime>();
            for (int i = 0; i < orderCount; i++)
            {
                orderDates.Add(itemFaker.Date.Between(customer.RegistrationDate, DateTime.UtcNow).ToUniversalTime());
            }

            orderDates.Sort();

            foreach (var orderDate in orderDates)
            {
                var itemsCount = itemFaker.Random.Int(1, 4);
                decimal calculatedTotal = 0;
                var currentOrderItems =
                    new List<(string id, string name, string cat, int qty, decimal price, decimal cost)>();

                for (int j = 0; j < itemsCount; j++)
                {
                    var price = Math.Round(itemFaker.Random.Decimal(20, 500), 2);
                    var cost = Math.Round(price * itemFaker.Random.Decimal(0.3m, 0.6m), 2); // Різна маржа
                    var qty = itemFaker.Random.Int(1, 2);

                    calculatedTotal += (price * qty);
                    currentOrderItems.Add((
                        itemFaker.Random.Replace("PROD-####"),
                        itemFaker.Commerce.ProductName(),
                        itemFaker.PickRandom(categories),
                        qty, price, cost));
                }

                // Генеруємо реалістичний статус замовлення у вигляді рядка (string)
                // Наприклад: "3" - Completed, "0" - Pending, "1" - Processing, "4" - Cancelled, "5" - Refunded
                var statusChance = itemFaker.Random.Double();
                OrderStatus randomStatus = statusChance switch
                {
                    < 0.70 => OrderStatus.Completed, // 70% успішних замовлень
                    < 0.80 => OrderStatus.Pending, // 10% очікують
                    < 0.90 => OrderStatus.Processing, // 10% в обробці
                    < 0.95 => OrderStatus.Cancelled, // 5% скасовано
                    _ => OrderStatus.Refunded // 5% повернень
                };

                var order = new Order(
                    tenantId: tenantId,
                    customerId: customer.Id,
                    externalOrderId: itemFaker.Random.Replace("ORD-######"),
                    totalAmount: calculatedTotal,
                    currency: currency,
                    orderDate: orderDate,
                    orderStatus: randomStatus // Передаємо згенерований рядок
                );

                if (order.Status == OrderStatus.Completed)
                {
                    customer.RecordNewOrder(calculatedTotal, orderDate);
                }


                foreach (var item in currentOrderItems)
                {
                    order.AddItem(item.id, item.name, item.cat, item.qty, item.price, item.cost);
                }

                orders.Add(order);
                customer.RecordOrder(orderDate);
            }
        }

        // 2. Маркетингові витрати
        var expenses = new List<MarketingExpense>();
        var expenseFaker = new Faker();

// Генеруємо витрати за останні 12 місяців
        for (int month = 0; month < 12; month++)
        {
            var referenceDate = DateTime.UtcNow.AddMonths(-month);
            var firstDayOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            foreach (var channel in channels.Where(c => c != "Organic"))
            {
                // 1. Спочатку генеруємо бюджет (від 3000 до 30000 для реалістичності)
                var amount = Math.Round(expenseFaker.Random.Decimal(3000, 30000), 2);

                var expense = new MarketingExpense(
                    tenantId: tenantId,
                    date: firstDayOfMonth,
                    amount: amount,
                    currency: currency,
                    adSource: channel,
                    campaignName: $"Кампанія_{referenceDate:MMMM_yyyy}"
                );

                // --- МАГІЯ РЕАЛІСТИЧНОЇ МАТЕМАТИКИ ---

                // 1. CPC (Ціна за клік). Наприклад, від 10 до 40 грн за один перехід.
                var cpc = expenseFaker.Random.Decimal(10, 40);
                var clicks = (int)(amount / cpc); // Якщо витратили 10000 і клік 20, значить було 500 кліків

                // 2. CTR (Клікабельність). Відсоток людей, які побачили рекламу і клікнули (від 1% до 4%).
                var ctr = expenseFaker.Random.Double(0.01, 0.04);
                var impressions = (int)(clicks / ctr); // Якщо 500 кліків це 2%, значить показів було 25 000

                // 3. Conversion Rate. Відсоток кліків, які стали "Лідами" (реєстраціями) (від 5% до 12%)
                var conversionRate = expenseFaker.Random.Double(0.05, 0.12);
                var leads = (int)(clicks * conversionRate);

                // Використовуємо наш метод з Domain-Driven Design!
                expense.RecordMetrics(impressions, clicks, leads);

                expenses.Add(expense);
            }
        }

        return new MockEcommerceData(customers, orders, expenses);
    }
}