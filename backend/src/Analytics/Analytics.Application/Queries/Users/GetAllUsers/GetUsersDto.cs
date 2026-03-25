namespace Analytics.Application.Queries.Users.GetAllUsers;

public record  GetUsersDto(
        string FullName,
        string Email,
        string Role,
        DateTime CreatedAt);
