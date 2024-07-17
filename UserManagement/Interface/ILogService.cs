using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Enums;

namespace UserManagement.Interface
{
    public interface ILogService
    {
        Task<IEnumerable<Log>> GetAll();
        Task TransactionLog(TransactionType type, string message, string actionBy);
    }
}