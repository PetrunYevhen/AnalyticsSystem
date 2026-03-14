namespace Domain;

public abstract class SmartEnum<TEnum> where TEnum : SmartEnum<TEnum>
{
    private static readonly List<TEnum> _values = new();
    
    public string Name { get; private set; }
    
    protected SmartEnum(string name)
    {
        Name = name;
    }
    

    public static IReadOnlyList<TEnum> GetAll()
    {
        if(_values == null)
            throw new ArgumentNullException($"Список пустий");
        return _values.AsReadOnly();
    }

    public static TEnum FromName(string name) => 
        _values.FirstOrDefault(v => v.Name == name)
            ?? throw new InvalidOperationException(
            $"SmartEnum '{typeof(TEnum).Name}' missing '{name}'. " +
            $"Registered: [{string.Join(", ", _values.Capacity)}]");
    
    
    public static bool TryFromName(string name, out TEnum? result)
    {
        result = _values.FirstOrDefault(v => v.Name == name);
        return result is not null;
    }
    
    protected static TEnum Add(TEnum value)
    {
        if (_values.Any(x => x.Name == value.Name))
            throw new InvalidOperationException($"Дублікат: {value.Name}");
        
        _values.Add(value);
        return value;
    }
    
    public override string ToString() => Name;
}