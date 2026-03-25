namespace Analytics.Application.Commands.Users.CreateUser.Dtos;

public class CreateUserDto
{
    public CreateUserDto(string fullName, string email, string password, string role)
    {
        FullName = fullName;
        Email = email;
        Password = password;
        Role = role;
    }

    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "Analyst"; 

}