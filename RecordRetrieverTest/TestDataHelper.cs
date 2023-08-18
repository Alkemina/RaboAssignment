using RecordRetrieverFunctionApp;

namespace RecordRetrieverTest
{
    public static class TestDataHelper
    {
        public static List<Record> GetFakeRecordList()
        {
            return new List<Record>
            {
                new Record { RecordId = 2, UserId = 2, UserName = "Bill", UserEmail = "bill@email.com", DataValue = "This is a dummy data #2", NotificationFlag = false, CreatedAtUtc = DateTime.UtcNow.AddDays(-1), ModifiedAtUtc = DateTime.UtcNow },
                new Record { RecordId = 3, UserId = 3, UserName = "Cecile", UserEmail = "cecile@email.com", DataValue = "This is a dummy data #3", NotificationFlag = false, CreatedAtUtc = DateTime.UtcNow, ModifiedAtUtc = DateTime.UtcNow },
                new Record { RecordId = 4, UserId = 4, UserName = "David", UserEmail = "david@email.com", DataValue = "This is a dummy data #4", NotificationFlag = false, CreatedAtUtc = DateTime.UtcNow, ModifiedAtUtc = DateTime.UtcNow },
                new Record { RecordId = 5, UserId = 5, UserName = "Edward", UserEmail = "edward@email.com", DataValue = "This is a dummy data #5", NotificationFlag = false, CreatedAtUtc = DateTime.UtcNow, ModifiedAtUtc = DateTime.UtcNow }
            };
        }
    }
}