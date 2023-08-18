using Azure.Messaging.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecordRetrieverFunctionApp;

namespace RecordRetrieverTest
{
    [TestClass]
    public class RecordRetrieverFunctionTests
    {
        private RecordSender sut;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger<RecordSender>> _recordSenderLoggerMock;
        private Mock<ServiceBusClient> _sbClientMock;
        private Mock<ServiceBusSender> _sbSenderMock;
        private Mock<IRecordRepository> _recordRepositoryMock;

        private DateTime lastExecutionDate = DateTime.UtcNow.AddMinutes(-5);

        [TestInitialize]
        public void Init()
        {
            _recordSenderLoggerMock = new Mock<ILogger<RecordSender>>();
            _recordSenderLoggerMock.Setup(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));

            _recordSenderLoggerMock.Setup(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));

            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerFactoryMock.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(_recordSenderLoggerMock.Object);

            _sbSenderMock = new Mock<ServiceBusSender>();
            _sbSenderMock.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()));

            _sbClientMock = new Mock<ServiceBusClient>();
            _sbClientMock.Setup(c => c.CreateSender(It.IsAny<string>())).Returns(_sbSenderMock.Object);

            _recordRepositoryMock = new Mock<IRecordRepository>();
            _recordRepositoryMock.Setup(r => r.GetLatestRecordsByDate(lastExecutionDate)).ReturnsAsync(TestDataHelper.GetFakeRecordList());

            sut = new RecordSender(_loggerFactoryMock.Object, _recordRepositoryMock.Object, _sbClientMock.Object);
        }


        [TestMethod]
        public void CreateRecordRetrieverFunction_should_succeed()
        {
            //Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public async Task RetrieveAndSendRecords_should_succeed()
        {
            //Arrange

            //Act
            var response = await sut.RetrieveAndSendRecords(lastExecutionDate);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(4, response);

            _recordRepositoryMock.Verify(c => c.GetLatestRecordsByDate(lastExecutionDate), Times.Once);
            _sbSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
            _sbClientMock.Verify(c => c.CreateSender(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task RetrieveAndSendRecords_should_throw_whenErrorOccuredDuringRetrievingData()
        {
            //Arrange
            _recordRepositoryMock.Setup(r => r.GetLatestRecordsByDate(lastExecutionDate)).Throws(new Exception("a dummy exception"));

            //Act
            var response = Assert.ThrowsExceptionAsync<Exception>(() => sut.RetrieveAndSendRecords(lastExecutionDate));

            //Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task RetrieveAndSendRecords_should_throw_whenErrorOccuredDuringSendingData()
        {
            //Arrange
            _sbSenderMock.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>())).Throws(new Exception("a dummy exception")); ;

            //Act
            var response = Assert.ThrowsExceptionAsync<Exception>(() => sut.RetrieveAndSendRecords(lastExecutionDate));

            //Assert
            Assert.IsNotNull(response);
        }
    }
}