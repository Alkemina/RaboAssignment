CREATE PROCEDURE [dbo].[sp_GetRecords]
(
	@LastExecutionDateTimeUtc DateTime
)
AS
BEGIN

    SET NOCOUNT ON

    SELECT [RecordId]
      ,[UserId]
      ,[UserName]
      ,[UserEmail]
      ,[DataValue]
      ,[NotificationFlag]
      ,[CreatedAtUtc]
      ,[ModifiedAtUtc]
	FROM DBO.Record WHERE CreatedAtUtc >= @LastExecutionDateTimeUtc OR ModifiedAtUtc >= @LastExecutionDateTimeUtc
END
GO

