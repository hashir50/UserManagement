namespace UserManagement.Interface
{
    public interface IOtpService
    {
        Task Send(int userId);
        Task<bool> verify(int userId, string otp);
    }
}