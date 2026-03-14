namespace ValueObjects.ValueObject;

public readonly record struct Money
{
    public decimal  Amount   { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (currency == default)
            throw new ArgumentException("Необхідна валюта", nameof(currency));
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money amount cannot be negative");
        Amount   = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Zero(Currency c) => new(0m, c);

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money m, int qty)
    {
        if (qty < 0) throw new ArgumentOutOfRangeException(nameof(qty));
        return new Money(m.Amount * qty, m.Currency);
    }

    public static Money operator *(Money m, decimal factor)
    {
        if (factor < 0) throw new ArgumentOutOfRangeException(nameof(factor));
        return new Money(m.Amount * factor, m.Currency);
    }

    public bool IsZero => Amount == 0m;

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException(
                $"Невідповідність валюти: {a.Currency.Code} vs {b.Currency.Code}. Перетворіть явно.");
    }

    public override string ToString() => $"{Amount:0.##} {Currency.Code}";
}