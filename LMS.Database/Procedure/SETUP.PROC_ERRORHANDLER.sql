CREATE OR ALTER PROCEDURE SETUP.PROC_ERRORHANDLER
(
@ErrorCode			VARCHAR(200) = NULL
,@ErrorMessage		VARCHAR(MAX) = NULL
,@ErrorSource		VARCHAR(300) = NULL
,@ErrorTable		VARCHAR(200) = NULL
,@ErrorLine			VARCHAR(200) = NULL
,@User				VARCHAR(150)	= NULL
) 
AS
BEGIN
     INSERT INTO SETUP.ERRORLOG(ErrorCode,ErrorMessage,ErrorDescription,ErrorSource,ErrorLine,
	 ErrorTable,CreatedBy,CreatedDateLocal,CreatedDateUTC)
	 SELECT @ErrorCode,UPPER(@ErrorMessage),UPPER(@ErrorMessage),@ErrorSource,@ErrorLine
	 ,@ErrorTable,@User,GETDATE(),GETUTCDATE()
		
	  SELECT @ErrorCode AS ErrorCode,UPPER(@ErrorMessage) AS [Message]
END 	