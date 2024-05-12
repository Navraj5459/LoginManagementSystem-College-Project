CREATE OR ALTER  PROCEDURE SETUP.PROC_USERS
(
@Flag					VARCHAR(250)
,@User					VARCHAR(250)	= NULL
,@Id					NVARCHAR(250)	= NULL
,@Message				NVARCHAR(500)	= NULL
,@ErrorLine				INT				= NULL
,@UserId				VARCHAR(250)	= NULL
,@Email					VARCHAR(150)	= NULL
,@Password				NVARCHAR(250)	= NULL
,@IPAddress				VARCHAR(250)	= NULL
,@FullName				VARCHAR(250)	= NULL
,@UserType				VARCHAR(250)	= NULL
,@OTP					VARCHAR(150)	= NULL
,@Token					VARCHAR(250)	= NULL
,@NewPassword			VARCHAR(250)	= NULL
)
WITH ENCRYPTION
AS
BEGIN TRY
IF (@Flag = 'UserLogin')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email AND IsActive = 1)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User is in-active.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email AND IsDeleted = 1)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Account has been deleted.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email AND PasswordExpiryDate < GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Password has been expired. Please change your password.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email AND PWDCOMPARE(@Password, Password) = 1
					AND IsActive = 1 AND IsDeleted = 0)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'User login successful.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User

		SELECT @UserId = Id 
		FROM SETUP.USERDETAIL WITH(NOLOCK)
		WHERE Email = @Email

		INSERT INTO SETUP.USERLOGINLOGDETAIL(UserId,LoginDate,IPAddress,CreatedBy,CreatedDateLocal,CreatedDateUTC)
		SELECT @UserId,GETDATE(),@IPAddress,@UserId,GETDATE(),GETUTCDATE()

		SELECT Id,Email,FullName,UserType,ForcePasswordChange,PasswordExpiryDate
		FROM SETUP.USERDETAIL WITH(NOLOCK)
		WHERE Email = @Email AND PWDCOMPARE(@Password, Password) = 1
	END
	ELSE
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Email and Password does not matched.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
END
ELSE IF(@Flag = 'UserRegister')
BEGIN
	IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User already exists.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
		INSERT INTO SETUP.USERDETAIL(Email,Password,FullName,UserType,PasswordExpiryDate,CreatedBy,CreatedDateLocal,CreatedDateUTC)
		SELECT @Email,PWDENCRYPT(@Password),@FullName,@UserType,DATEADD(DAY,30,GETDATE()),@User,GETDATE(),GETUTCDATE()
	IF @@ERROR=0
	BEGIN
		COMMIT TRANSACTION 
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'USER ADDED SUCCESSFULLY'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
		RETURN
	END
	END
END
ELSE IF(@Flag = 'UpdateOTP')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
		UPDATE SETUP.USERDETAIL
		SET OTP = @OTP
		,OTPExpiryDate = DATEADD(MINUTE,5,GETDATE())
		,UpdatedBy = @User
		,UpdatedDateLocal = GETDATE()
		,UpdatedDateUTC = GETUTCDATE()
		WHERE Id = @Id

	IF @@ERROR=0
	BEGIN
		COMMIT TRANSACTION 
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'OTP UPDATED SUCCESSFULLY'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
		RETURN
	END
	END
END
ELSE IF(@Flag = 'VerifyOTP')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND OTP = @OTP)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Invalid OTP.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND OTP = @OTP AND OTPExpiryDate < GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'OTP expired.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND OTP = @OTP AND OTPExpiryDate > GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'Valid OTP.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User

		SELECT Id,Email,FullName,UserType,ForcePasswordChange,PasswordExpiryDate
		FROM SETUP.USERDETAIL WITH(NOLOCK)
		WHERE Id = @Id
	END
	ELSE
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Invalid OTP.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
END
ELSE IF(@Flag = 'UpdateToken')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
		UPDATE SETUP.USERDETAIL
		SET Token = @Token
		,TokenExpiryDate = DATEADD(DAY,1,GETDATE())
		,UpdatedBy = @User
		,UpdatedDateLocal = GETDATE()
		,UpdatedDateUTC = GETUTCDATE()
		WHERE Id = @Id

	IF @@ERROR=0
	BEGIN
		COMMIT TRANSACTION 
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'Token UPDATED SUCCESSFULLY'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
		RETURN
	END
	END
END
ELSE IF(@Flag = 'GetUserDetailForForgotPassword')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Email = @Email)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'User detail found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User

		SELECT Id,Email,FullName,UserType,ForcePasswordChange,PasswordExpiryDate
		FROM SETUP.USERDETAIL WITH(NOLOCK)
		WHERE Email = @Email
	END
END
ELSE IF(@Flag = 'GetUserDetailForResetPassword')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND Token = @Token)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Invalid Token.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND Token = @Token AND TokenExpiryDate < GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Token expired.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND Token = @Token AND TokenExpiryDate > GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'Valid Token.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User

		SELECT Id,Email,FullName,UserType,ForcePasswordChange,PasswordExpiryDate
		FROM SETUP.USERDETAIL WITH(NOLOCK)
		WHERE Id = @Id AND Token = @Token AND TokenExpiryDate > GETDATE()
	END
	ELSE
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Invalid Token.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
END
ELSE IF(@Flag = 'ResetPassword')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND Token = @Token)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Invalid Token.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND Token = @Token AND TokenExpiryDate < GETDATE())
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'Token expired.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND PWDCOMPARE(@NewPassword, Password) = 1)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'CANNOT HAVE SAME AS PREVIOUS PASSWORD.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
		UPDATE SETUP.USERDETAIL
		SET Password = PWDENCRYPT(@NewPassword)
		,UpdatedBy = @User
		,UpdatedDateLocal = GETDATE()
		,UpdatedDateUTC = GETUTCDATE()
		WHERE Id = @Id AND Token = @Token AND TokenExpiryDate > GETDATE()
	IF @@ERROR=0
	BEGIN
		COMMIT TRANSACTION 
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'PASSWORD RESET SUCCESSFULLY'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
		RETURN
	END
	END
END
ELSE IF(@Flag = 'GetTotalUser')
BEGIN
	EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'Detail found'
	,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User

	SELECT COUNT(*) AS TotalUser FROM SETUP.USERDETAIL WITH(NOLOCK)

END
ELSE IF(@Flag = 'ChangePassword')
BEGIN
	IF NOT EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'User not found.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE IF EXISTS(SELECT 'A' FROM SETUP.USERDETAIL WITH(NOLOCK) WHERE Id = @Id AND PWDCOMPARE(@NewPassword, Password) = 1)
	BEGIN
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '400',@ErrorMessage = 'CANNOT HAVE SAME AS PREVIOUS PASSWORD.'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	END
	ELSE
	BEGIN
	BEGIN TRANSACTION
		UPDATE SETUP.USERDETAIL
		SET Password = PWDENCRYPT(@NewPassword)
		,UpdatedBy = @User
		,UpdatedDateLocal = GETDATE()
		,UpdatedDateUTC = GETUTCDATE()
		WHERE Id = @Id
	IF @@ERROR=0
	BEGIN
		COMMIT TRANSACTION 
		EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '200',@ErrorMessage = 'PASSWORD CHANGED SUCCESSFULLY'
		,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
		RETURN
	END
	END
END
END TRY
BEGIN CATCH
	SELECT @Message=ERROR_MESSAGE(),@ErrorLine=ERROR_LINE()
	IF @@TRANCOUNT<>0
	BEGIN
		ROLLBACK TRANSACTION;
	END
	EXECUTE SETUP.PROC_ERRORHANDLER @ErrorCode = '500',@ErrorMessage = @Message
	,@ErrorSource ='SETUP.PROC_USERS',@ErrorTable ='SETUP.USERDETAIL',@User =@User
	RETURN
END CATCH