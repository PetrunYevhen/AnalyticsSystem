using Analytics.Application.Contracts;

namespace Analytics.Application.Commands.Users.ChangePassword;

public class ChangePasswordCommand : CommandBase
{
    public ChangePasswordCommand(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public string CurrentPassword { get; set; }
    public string NewPassword { get; }
}