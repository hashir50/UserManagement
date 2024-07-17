using UserManagement.Domain.Entities;
using UserManagement.Domain.Repository;
using UserManagement.Infrastructure.Enums;
using UserManagement.Interface;

namespace UserManagement.Service
{
    public class LogService : ILogService
    {
        private readonly IGenericRepository<Log> _logRepository;
        public LogService(IGenericRepository<Log> logRepository)
        {
            this._logRepository = logRepository;
        }
        public async Task TransactionLog(TransactionType type, string message, string actionBy)
        {
            Log logEntity = new Log
            {
                Type = type.ToString(),
                Time = DateTime.Now,
                Description = message,
                ActionBy = actionBy

            };
            await _logRepository.InsertAsync(logEntity);
            await _logRepository.SaveAsync();

        }

        public async Task<IEnumerable<Log>> GetAll()
        {
            try
            {
                return await _logRepository.GetAllAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
