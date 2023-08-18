
using Microsoft.EntityFrameworkCore;

namespace RecordRetrieverFunctionApp
{
    /// <summary>
    /// Record data model class
    /// </summary>
    public class Record
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }
        public string DataValue { get; set; }
        public bool NotificationFlag { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ModifiedAtUtc { get; set; }

    }

    /// <summary>
    /// EF Core Record Context class
    /// </summary>
    public class RecordContext : DbContext
    {
        public RecordContext(DbContextOptions<RecordContext> options)
        : base(options)
        { }

        public DbSet<Record> Records { get; set; }
    }
}
