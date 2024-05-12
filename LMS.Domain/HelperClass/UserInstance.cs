using LMS.Domain.Entities.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.HelperClass
{
    public sealed class UserInstance
    {
        private static UserInstance Instance = new UserInstance();
        private static readonly Dictionary<string, LoggedInUser> UserList = new Dictionary<string, LoggedInUser>();
        public static UserInstance GetInstance()
        {
            if (Instance == null)
            {
                lock (UserList)
                {
                    if (Instance == null)
                    {
                        return Instance = new UserInstance();
                    }
                }
            }
            return Instance;
        }
        public BaseResponseModel AddUser(LoggedInUser user)
        {
            var dbResult = new BaseResponseModel();
            if (!string.IsNullOrEmpty(user?.UserId))
            {
                if (!IsUserExists(user.UserId))
                {
                    lock (UserList)
                    {
                        var exists = UserList.ContainsKey(user.UserId);
                        if (exists)
                        {
                            UserList.Remove(user.UserId);
                        }
                        UserList.Add(user.UserId, user);
                    }
                }
                else
                {
                    UserInstance pool = GetInstance();
                    var loggedInUsers = pool.GetLoggedInUsers();
                    lock (UserList)
                    {
                        foreach (
                            var loggedInUser in
                                loggedInUsers.ToList().Where(
                                    loggedInUser => loggedInUser.Value.UserId == user.UserId))
                        {
                            loggedInUser.Value.Browser = user.Browser;
                            loggedInUser.Value.LoginTime = user.LoginTime;
                            loggedInUser.Value.IPAddress = user.IPAddress;
                            loggedInUser.Value.UserId = user.UserId;
                            loggedInUser.Value.ProfileImage = user.ProfileImage;
                        }
                    }
                }
                dbResult.SetResponse(0, "Login Successful", user.UserId);
            }
            else
            {
                dbResult = new BaseResponseModel() { ErrorCode = 1, Message = "User not found!" };
            }
            return dbResult;
        }
        public void RemoveUser(string UserId)
        {
            if (IsUserExists(UserId))
            {
                lock (UserList)
                {
                    UserList.Remove(UserId);
                }
            }
        }
        public Dictionary<string, LoggedInUser> GetLoggedInUsers()
        {
            return UserList;
        }
        public bool IsUserExists(string UserId)
        {
            if (!string.IsNullOrEmpty(StaticHelper.GetUserId()))
            {
                if (UserList.ContainsKey(UserId))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public class LoggedInUser
        {
            public string? Email { get; set; }
            public string? MobileNo { get; set; }
            public string? UserId { get; set; }
            public string? FullName { get; set; }
            public bool IsPasswordExpired { get; set; }
            public string? PasswordExpiryDate { get; set; }
            public bool ForcePasswordChange { get; set; }
            public DateTime LoginTime { get; set; }
            public string? IPAddress { get; set; }
            public string? Browser { get; set; }
            public string? UserType { get; set; }
            public string? ProfileImage { get; set; }

        }
    }
}
