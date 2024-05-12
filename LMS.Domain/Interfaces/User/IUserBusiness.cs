using LMS.Domain.Entities.Domain;
using LMS.Domain.Entities.Domain.User.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces.User
{
    public interface IUserBusiness
    {
        Task<ResponseCommon> UserLogin(LoginRequestModel req);
        Task<ResponseCommon> UserRegister(RegisterRequestModel req);
        Task<ResponseCommon> VerifyOTP(OTPRequestModel req);
        Task<ResponseCommon> UpdateOTP(OTPRequestModel req);
        Task<ResponseCommon> GetUserDetailForForgotPassword(ForgotPasswordRequestModel req);
        Task<ResponseCommon> UpdateToken(TokenRequestModel req);
        Task<ResponseCommon> GetResetPasswordUserDetail(ResetPasswordDetailRequestModel req);
        Task<ResponseCommon> ResetPassword(ResetPasswordRequestModel req);
        Task<ResponseCommon> GetTotalUser();
        Task<ResponseCommon> ChangePassword(ChangePasswordRequestModel req);
    }
}
