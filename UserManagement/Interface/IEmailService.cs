namespace UserManagement.Interface
{
    public interface IEmailService
    {
        void SendOtpEmail(string toEmail,string username, string otp);
        void SendResetPasswordEmail(string toEmail, string username, string callbackUrl);
    }
}