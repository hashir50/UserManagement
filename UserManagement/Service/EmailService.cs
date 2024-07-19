using System;
using MimeKit;
using MailKit.Net.Smtp;
using UserManagement.Interface;
using UserManagement.Infrastructure.ExternalServices.Email;
namespace UserManagement.Service
{
    public class EmailService : IEmailService
    {
        private readonly IEmailHelper _emailHelper;
        public EmailService(IEmailHelper emailHelper)
        {
            this._emailHelper = emailHelper;
        }

        public void SendOtpEmail(string toEmail, string username,string otp)
        {
            try
            {

                string subject = "Email Verification";
                string path = Path.Combine("Files", "EmailTemplates", "EmailVerification_Template.html");
                if (!File.Exists(path))
                    throw new Exception($"File Not Found => {path}");
                string htmlContent = File.ReadAllText(path);

                htmlContent = htmlContent.Replace("[username]", username);
                htmlContent = htmlContent.Replace("[otp]", otp);


                _emailHelper.Send(toEmail, subject, htmlContent, true);
            }
            catch (Exception)
            {
                throw;
            }
        } 
        public void SendResetPasswordEmail(string toEmail, string username,string callbackUrl)
        {
            try
            {

                string subject = "Reset Password";
                string path = Path.Combine("Files", "EmailTemplates", "ResetPassword_Template.html");
                if (!File.Exists(path))
                    throw new Exception($"File Not Found => {path}");
                string htmlContent = File.ReadAllText(path);

                htmlContent = htmlContent.Replace("[username]", username);
                htmlContent = htmlContent.Replace("[callbackUrl]", callbackUrl);


                _emailHelper.Send(toEmail, subject, htmlContent, true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}