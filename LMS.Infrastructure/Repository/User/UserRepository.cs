using LMS.Domain.Entities.Domain;
using LMS.Domain.Entities.Domain.User.RequestModel;
using LMS.Domain.Entities.Domain.User.ResponseModel;
using LMS.Domain.Interfaces.Dapper;
using LMS.Domain.Interfaces.User;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository.User
{
    public class UserRepository : IUserRepository
    {
        IDapperDAO _dao;
        private readonly ILogger _log = Log.ForContext<UserRepository>();
        private static string StoredProcedure = "SETUP.PROC_USERS";
        public UserRepository(IDapperDAO dao)
        {
            _dao = dao;
        }
        public async Task<ResponseCommon> UserLogin(LoginRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for user login {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel, LoginResponseModel>(StoredProcedure, req);
                _log.Information("response returned for user login as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    if (((List<BaseResponseModel>)result[0])?.Count > 0)
                    {
                        var res = ((List<BaseResponseModel>)result[0])[0];
                        response.Result = res;
                        if (response.Result?.ErrorCode == 200)
                        {
                            var model = new LoginResponseModel();
                            if (result?.Count > 1)
                            {
                                if (((List<LoginResponseModel>)result[1]).Count > 0)
                                {
                                    model = ((List<LoginResponseModel>)result[1])[0];
                                }
                            }
                            response.ResultCommon = model;
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> UserRegister(RegisterRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for user register {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel>(StoredProcedure, req);
                _log.Information("response returned for user register as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    response.Result = result[0];
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> VerifyOTP(OTPRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for verify OTP {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel, LoginResponseModel>(StoredProcedure, req);
                _log.Information("response returned for verify OTP as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    if (((List<BaseResponseModel>)result[0])?.Count > 0)
                    {
                        var res = ((List<BaseResponseModel>)result[0])[0];
                        response.Result = res;
                        if (response.Result?.ErrorCode == 200)
                        {
                            var model = new LoginResponseModel();
                            if (result?.Count > 1)
                            {
                                if (((List<LoginResponseModel>)result[1]).Count > 0)
                                {
                                    model = ((List<LoginResponseModel>)result[1])[0];
                                }
                            }
                            response.ResultCommon = model;
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> UpdateOTP(OTPRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for update OTP {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel>(StoredProcedure, req);
                _log.Information("response returned for update OTP as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    response.Result = result[0];
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> GetUserDetailForForgotPassword(ForgotPasswordRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for user detail for forgot password {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel, UserResponseModel>(StoredProcedure, req);
                _log.Information("response returned for user detail for forgot password as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    if (((List<BaseResponseModel>)result[0])?.Count > 0)
                    {
                        var res = ((List<BaseResponseModel>)result[0])[0];
                        response.Result = res;
                        if (response.Result?.ErrorCode == 200)
                        {
                            var model = new UserResponseModel();
                            if (result?.Count > 1)
                            {
                                if (((List<UserResponseModel>)result[1]).Count > 0)
                                {
                                    model = ((List<UserResponseModel>)result[1])[0];
                                }
                            }
                            response.ResultCommon = model;
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> UpdateToken(TokenRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for update token {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel>(StoredProcedure, req);
                _log.Information("response returned for update token as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    response.Result = result[0];
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> GetResetPasswordUserDetail(ResetPasswordDetailRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for reset password user detail {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel, UserResponseModel>(StoredProcedure, req);
                _log.Information("response returned for reset password user detail as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    if (((List<BaseResponseModel>)result[0])?.Count > 0)
                    {
                        var res = ((List<BaseResponseModel>)result[0])[0];
                        response.Result = res;
                        if (response.Result?.ErrorCode == 200)
                        {
                            var model = new UserResponseModel();
                            if (result?.Count > 1)
                            {
                                if (((List<UserResponseModel>)result[1]).Count > 0)
                                {
                                    model = ((List<UserResponseModel>)result[1])[0];
                                }
                            }
                            response.ResultCommon = model;
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> ResetPassword(ResetPasswordRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for reset password {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel>(StoredProcedure, req);
                _log.Information("response returned for reset password as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    response.Result = result[0];
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> GetTotalUser()
        {
            var response = new ResponseCommon();
            try
            {
                dynamic req = new ExpandoObject();
                req.Flag = "GetTotalUser";
                _log.Information("sp call for reset password user detail {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel, int>(StoredProcedure, req);
                _log.Information("response returned for reset password user detail as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    if (((List<BaseResponseModel>)result[0])?.Count > 0)
                    {
                        var res = ((List<BaseResponseModel>)result[0])[0];
                        response.Result = res;
                        if (response.Result?.ErrorCode == 200)
                        {
                            if (result?.Count > 1)
                            {
                                if (((List<int>)result[1]).Count > 0)
                                {
                                    response.ResultCommon = ((List<int>)result[1])[0];
                                }
                            }
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
        public async Task<ResponseCommon> ChangePassword(ChangePasswordRequestModel req)
        {
            var response = new ResponseCommon();
            try
            {
                _log.Information("sp call for change password {0} {1}", "EXEC " + StoredProcedure, JsonConvert.SerializeObject(req));
                var result = await _dao.ExecuteQueryAsync<BaseResponseModel>(StoredProcedure, req);
                _log.Information("response returned for change password as {0}", JsonConvert.SerializeObject(result));
                if (result?.Count > 0)
                {
                    response.Result = result[0];
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return response;
            }
        }
    }
}
