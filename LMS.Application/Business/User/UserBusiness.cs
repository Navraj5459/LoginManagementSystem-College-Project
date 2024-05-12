using LMS.Domain.Entities.Domain;
using LMS.Domain.Entities.Domain.User.RequestModel;
using LMS.Domain.Interfaces.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Business.User
{
    public class UserBusiness : IUserBusiness
    {
        IUserRepository _repo;
        public UserBusiness(IUserRepository _repo)
        {
            this._repo = _repo;
        }
        public async Task<ResponseCommon> UserLogin(LoginRequestModel req)
        {
            return await _repo.UserLogin(req);
        }
        public async Task<ResponseCommon> UserRegister(RegisterRequestModel req)
        {
            return await _repo.UserRegister(req);
        }
        public async Task<ResponseCommon> VerifyOTP(OTPRequestModel req)
        {
            return await _repo.VerifyOTP(req);
        }
        public async Task<ResponseCommon> UpdateOTP(OTPRequestModel req)
        {
            return await _repo.UpdateOTP(req);
        }
        public async Task<ResponseCommon> GetUserDetailForForgotPassword(ForgotPasswordRequestModel req)
        {
            return await _repo.GetUserDetailForForgotPassword(req);
        }
        public async Task<ResponseCommon> UpdateToken(TokenRequestModel req)
        {
            return await _repo.UpdateToken(req);
        }
        public async Task<ResponseCommon> GetResetPasswordUserDetail(ResetPasswordDetailRequestModel req)
        {
            return await _repo.GetResetPasswordUserDetail(req);
        }
        public async Task<ResponseCommon> ResetPassword(ResetPasswordRequestModel req)
        {
            return await _repo.ResetPassword(req);
        }
        public async Task<ResponseCommon> GetTotalUser()
        {
            return await _repo.GetTotalUser();
        }
        public async Task<ResponseCommon> ChangePassword(ChangePasswordRequestModel req)
        {
            return await _repo.ChangePassword(req);
        }
    }
}
