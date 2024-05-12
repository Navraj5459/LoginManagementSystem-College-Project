CREATE TABLE SETUP.USERDETAIL
(
RowId						BIGINT IDENTITY(1,1)
,Id							VARCHAR(250) PRIMARY KEY  DEFAULT (NEWID())
,Email						VARCHAR(150)
,[Password]					VARBINARY(MAX)
,FullName					VARCHAR(250)
,UserType					VARCHAR(250)
,ForcePasswordChange		BIT
,PasswordExpiryDate			DATETIME
,Token						VARCHAR(250)
,TokenExpiryDate			DATETIME
,OTP						VARCHAR(250)
,OTPExpiryDate				DATETIME
,IsActive					BIT DEFAULT(1)
,IsDeleted					BIT DEFAULT(0)
,CreatedBy					VARCHAR(250)
,CreatedDateLocal			DATETIME
,CreatedDateUTC				DATETIME
,UpdatedBy					VARCHAR(150)
,UpdatedDateLocal			DATETIME
,UpdatedDateUTC				DATETIME
)
