using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RecordRetrieverFunctionApp
{
    ///
    public interface IRecordRepository
    {
        Task<List<Record>> GetLatestRecordsByDate(DateTime lastExecutedUtc);
    }

    /// <summary>
    /// Record repository class, implements the <see cref="IRecordRepository"/> to handle database operation concerning Record table
    /// </summary>
    public class RecordRepository : IRecordRepository
    {
        private readonly ILogger _logger;
        private readonly RecordContext _recordContext;

        public RecordRepository(ILoggerFactory loggerFactory, RecordContext recordDbContext)
        {
            _logger = loggerFactory.CreateLogger<RecordRepository>();
            _recordContext = recordDbContext;
        }

        public async Task<List<Record>> GetLatestRecordsByDate(DateTime lastExecutedUtc)
        {
            try
            {
                return await _recordContext.Records.FromSql($"GetRecord {lastExecutedUtc}").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve records from database");
                throw;
            }
        }
    }
}
