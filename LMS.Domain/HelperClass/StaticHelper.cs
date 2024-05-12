using LMS.Domain.Entities.Domain;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LMS.Domain.HelperClass.UserInstance;

namespace LMS.Domain.HelperClass
{
    public static class StaticHelper
    {
        static HttpContextAccessor _context = new HttpContextAccessor();
        static string SaltKey = "D1604F87789443BCA89A1F7776ED16D7";
        public static string GetUserId()
        {
            var val = string.Empty;
            if (_context != null)
            {
                if (_context.HttpContext != null)
                {
                    var UserId = _context.HttpContext.Session.GetString("UserId");
                    val = UserId == null ? null : UserId.ToString();
                }
            }
            return val;
        }
        public static BaseResponseModel CheckPasswordPolicy(string password)
        {
            var result = new BaseResponseModel();
            if (!string.IsNullOrEmpty(password))
            {
                result = new BaseResponseModel
                {
                    ErrorCode = 200,
                    Message = "Password is Strong",
                };
            }
            string specialCasePattern = @"[^a-zA-Z0-9]";
            string lowerCasePattern = @"[a-z]";
            string upperCasePattern = @"[A-Z]";
            string numberPattern = @"\d";
            if (!(Regex.IsMatch(password, specialCasePattern)))
            {
                result = new BaseResponseModel
                {
                    ErrorCode = 400,
                    Message = "Password Must Contain At Least One Special Character.",
                };
            }
            if (!(Regex.IsMatch(password, lowerCasePattern)))
            {
                result = new BaseResponseModel
                {
                    ErrorCode = 400,
                    Message = "Password Must Contain At Least One lowercase Character.",
                };
            }
            if (!(Regex.IsMatch(password, upperCasePattern)))
            {
                result = new BaseResponseModel
                {
                    ErrorCode = 400,
                    Message = "Password Must Contain At Least One Uppercase Character.",
                };
            }
            if (!(Regex.IsMatch(password, numberPattern)))
            {
                result = new BaseResponseModel
                {
                    ErrorCode = 400,
                    Message = "Password Must Contain At Least One Numeric Character.",
                };
            }
            return result;
        }
        public static async Task<BaseResponseModel> SendEmailAsync(EmailHelperCommon mailRequest)
        {
            var response = new BaseResponseModel();
            try
            {
                if (mailRequest != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    MimeMessage mail = new MimeMessage();
                    mail.From.Add(new MailboxAddress(mailRequest.FromFullName, string.IsNullOrEmpty(mailRequest.FromEmailAddress) ? "" : mailRequest.FromEmailAddress));
                    mail.To.Add(new MailboxAddress(mailRequest.ToFullName, string.IsNullOrEmpty(mailRequest.ToEmailAddress) ? "" : mailRequest.ToEmailAddress));
                    if (!string.IsNullOrEmpty(mailRequest.CCEmail))
                    {
                        mail.Cc.Add(new MailboxAddress(mailRequest.CCFullName, string.IsNullOrEmpty(mailRequest.CCEmail) ? "" : mailRequest.CCEmail));
                    }
                    mail.Subject = string.IsNullOrEmpty(mailRequest.Subject) ? "" : mailRequest.Subject;
                    var builder = new BodyBuilder();
                    builder.HtmlBody = mailRequest.Message;
                    mail.Body = builder.ToMessageBody();
                    using (MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await smtp.ConnectAsync(mailRequest.SmtpServer, Convert.ToInt32(mailRequest.SmtpPort), false);
                        await smtp.AuthenticateAsync(mailRequest.SmtpUsername, mailRequest.SmtpPassword);
                        await smtp.SendAsync(mail);
                        await smtp.DisconnectAsync(true);
                    }
                    response.ErrorCode = 200;
                    response.Message = "SUCCESS!";
                    return response;
                }
                else
                {
                    response.ErrorCode = 400;
                    response.Message = "Unable send mail!";
                    return response;
                }
            }
            catch (TimeoutException ex)
            {
                response.ErrorCode = 400;
                response.Message = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
        public static string GetOTPNumber()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[6];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;
        }
        public static string GetUserEmail()
        {
            var userList = GetInstance().GetLoggedInUsers();
            var Email = "";
            foreach (var users in userList.Values)
            {
                if (!string.IsNullOrEmpty(StaticHelper.GetUserId()))
                {
                    if (StaticHelper.GetUserId() == users.UserId)
                    {
                        Email = users.Email;
                    }
                }
            }
            return Email;
        }
        public static string GetUserFullName()
        {
            var userList = GetInstance().GetLoggedInUsers();
            var FullName = "";
            foreach (var users in userList.Values)
            {
                if (!string.IsNullOrEmpty(StaticHelper.GetUserId()))
                {
                    if (StaticHelper.GetUserId() == users.UserId)
                    {
                        FullName = users.FullName;
                    }
                }
            }
            return FullName;
        }
        public static string GetUserType()
        {
            var userList = GetInstance().GetLoggedInUsers();
            var UserType = "";
            foreach (var users in userList.Values)
            {
                if (!string.IsNullOrEmpty(StaticHelper.GetUserId()))
                {
                    if (StaticHelper.GetUserId() == users.UserId)
                    {
                        UserType = users.UserType;
                    }
                }
            }
            return UserType;
        }
        public static string GetToken()
        {
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var resultToken = new string(
               Enumerable.Repeat(allChar, 20)
               .Select(token => token[random.Next(token.Length)]).ToArray());

            string authToken = resultToken.ToString();
            return authToken;
        }
        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SaltKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SaltKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
