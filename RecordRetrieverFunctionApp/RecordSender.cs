using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace RecordRetrieverFunctionApp
{
    public interface IRecordSender
    {
        Task<int> RetrieveAndSendRecords(DateTime lastExecutedUtc);
    }
    public class RecordSender : IRecordSender
    {
        private readonly ILogger _logger;
        private readonly IRecordRepository _recordRepository;
        private readonly ServiceBusClient _sbClient;

        public RecordSender(ILoggerFactory loggerFactory, IRecordRepository recordRepository, ServiceBusClient serviceBusClient)
        {
            _logger = loggerFactory.CreateLogger<RecordSender>();
            _recordRepository = recordRepository;
            _sbClient = serviceBusClient;
        }

        public async Task<int> RetrieveAndSendRecords(DateTime lastExecutedUtc)
        {
            int recordSent = 0;
            try
            {
                var records = await _recordRepository.GetLatestRecordsByDate(lastExecutedUtc);
                _logger.LogInformation($"Found {records.Count} new records");

                var sender = _sbClient.CreateSender("recordQueue");
                foreach (var record in records)
                {
                    var message = new ServiceBusMessage(JsonSerializer.Serialize(record));
                    await sender.SendMessageAsync(message);
                    recordSent++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured executing RetrieveAndSendRecords");
                throw;
            }
            return recordSent;
        }
    }
}
