namespace Analytics.Application.Exeptions;

public class BusinessRuleValidationException : Exception
{
    public  BusinessRuleValidationException(string message) : base(message)
    {
    }
}