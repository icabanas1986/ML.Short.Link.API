namespace ML.Short.Link.API.Utils.Interface
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmationAsync(string toEmail, string userName, string confirmationLink);

        string GenerateConfirmationToken();
    }
}
