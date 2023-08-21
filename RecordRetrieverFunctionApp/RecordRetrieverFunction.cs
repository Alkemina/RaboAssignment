using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RecordRetrieverFunctionApp
{
    /// <summary>
    /// RecordRetrieverFunction
    /// </summary>
    public class RecordRetrieverFunction
    {
        private readonly ILogger _logger;
        private readonly IRecordSender _recordSender;

        /// <summary>
        /// Constructor of <see cref="RecordRetrieverFunction"/> class
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="recordSender"></param>
        public RecordRetrieverFunction(ILoggerFactory loggerFactory, IRecordSender recordSender)
        {
            _logger = loggerFactory.CreateLogger<RecordRetrieverFunction>();
            _recordSender = recordSender;
        }

        /// <summary>
        /// Record retriever Time-Triggered Azure Function 
        /// </summary>
        /// <param name="myTimer"></param>
        /// <returns></returns>
        [Function("RecordRetrieverFunction")]
        public async Task Run([TimerTrigger("%RecordRetrieverTimeTrigger%"
#if DEBUG
            , RunOnStartup = true
#endif
            )] MyInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"RecordRetrieverFunction executed at: {DateTime.Now}");
                var lastExecutedUtc = DateTime.UtcNow;
                if (myTimer.ScheduleStatus != null)
                    lastExecutedUtc = myTimer.ScheduleStatus.Last.ToUniversalTime();

                var recordsSent = await _recordSender.RetrieveAndSendRecords(lastExecutedUtc);
                _logger.LogInformation($"Successfully sending  {recordsSent} records to ServiceBus");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in RecordRetrieverFunction");
                throw;
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
