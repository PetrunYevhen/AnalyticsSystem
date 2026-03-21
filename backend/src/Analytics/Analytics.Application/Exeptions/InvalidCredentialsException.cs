namespace Analytics.Application.Exeptions;

public class InvalidCredentialsException : Exception
{
    public  InvalidCredentialsException()         
        : base("Невірний email або пароль.") { }
}