namespace ValueObjects.ValueObject;

public readonly record struct Currency
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "USD",
        "EUR",
        "UAH"
    };
    
    public string Code { get; }

    public Currency(string code)
    {
        Code = code;
    }

    public static Currency Parse(string input)
    {
        var normalized = input.Trim().ToUpperInvariant()
            ?? throw new ArgumentNullException(nameof(input));
        if (!Allowed.Contains(normalized))
            throw new Exception($"Валюта не підтримується: {input}");
        return new Currency(normalized);
    }
}