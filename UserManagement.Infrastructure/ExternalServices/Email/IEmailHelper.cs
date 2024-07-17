namespace UserManagement.Infrastructure.ExternalServices.Email
{
    public interface IEmailHelper
    {
        void Send(string toEmail, string subject, string body, bool isHtmlBody);
    }
}