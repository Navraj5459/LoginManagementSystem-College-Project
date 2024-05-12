CREATE TABLE SETUP.ERRORLOG
(
 RowId						BIGINT IDENTITY(1,1)
,Id							VARCHAR(150)  DEFAULT (NEWID())
,ErrorCode					VARCHAR(500)
,ErrorMessage				VARCHAR(500)
,ErrorDescription			VARCHAR(500)
,ErrorSource				VARCHAR(500)
,ErrorLine					VARCHAR(500)
,ErrorTable					VARCHAR(500)
,CreatedBy					VARCHAR(150)
,CreatedDateLocal			DATETIME
,CreatedDateUTC				DATETIME
)