using AspNetCore.ReCaptcha;
using LMS.Domain.Entities.Domain;
using LMS.Domain.Entities.Domain.User.RequestModel;
using LMS.Domain.Entities.Domain.User.ResponseModel;
using LMS.Domain.Entities.ViewModels.Login;
using LMS.Domain.Entities.ViewModels.Register;
using LMS.Domain.Entities.ViewModels.User;
using LMS.Domain.HelperClass;
using LMS.Domain.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using Newtonsoft.Json;
using NuGet.Common;
using System.Diagnostics;
using static LMS.Domain.HelperClass.UserInstance;

namespace LMS.Web.Controllers
{
    public class HomeController : Controller
    {
        IUserBusiness _buss;
        IConfiguration _config;
        public HomeController(IUserBusiness _buss, IConfiguration config)
        {
            this._buss = _buss;
            _config = config;
        }
        public IActionResult Index()
        {
            if (TempData != null)
            {
                if (TempData.ContainsKey("Response"))
                {
                    if (TempData["Response"] != null)
                    {
                        var response = JsonConvert.DeserializeObject<BaseResponseModel>(TempData["Response"].ToString());
                        ViewData["Response"] = response;
                    }
                }
            }
            if (string.IsNullOrEmpty(StaticHelper.GetUserId()))
            {
                GetInstance().RemoveUser(StaticHelper.GetUserId());
                HttpContext.Session.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateReCaptcha]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var error = StaticHelper.CheckPasswordPolicy(model.Password);
                    if(error?.ErrorCode == 400)
                    {
                        ModelState.AddModelError("Password", error.Message);
                    }
                }
                if (ModelState.IsValid)
                {
                    var req = new LoginRequestModel();
                    req.Email = model.Email;
                    req.Password = StaticHelper.EncryptString(model.Password);
                    req.IPAddress = Request.HttpContext.Connection.RemoteIpAddress != null ? Request.HttpContext.Connection.RemoteIpAddress.ToString() : "";
                    req.Flag = "UserLogin";
                    var res = await _buss.UserLogin(req);
                    if(res?.Result?.ErrorCode == 200)
                    {
                        if(res?.ResultCommon != null)
                        {
                            var common = (LoginResponseModel)res.ResultCommon;
                            var otp = StaticHelper.GetOTPNumber();
                            var emailcommon = new EmailHelperCommon();
                            emailcommon.ToEmailAddress = common.Email;
                            emailcommon.ToFullName = common.FullName;
                            emailcommon.Message = "Your OTP is " + otp;
                            emailcommon.Subject = "User Login";
                            emailcommon.FromEmailAddress = _config["MailSettings:Mail"].ToString();
                            emailcommon.SmtpUsername = _config["MailSettings:Mail"].ToString();
                            emailcommon.SmtpPassword = _config["MailSettings:Password"].ToString();
                            emailcommon.SmtpPort = _config["MailSettings:Port"].ToString();
                            emailcommon.SmtpServer = _config["MailSettings:Host"].ToString();
                            emailcommon.UseDefaultCredentials = _config["MailSettings:UseDefaultCredentials"].ToString();
                            emailcommon.EnableSsl = _config["MailSettings:EnableSsl"].ToString();
                            var mailresponse = await StaticHelper.SendEmailAsync(emailcommon);
                            if(mailresponse?.ErrorCode == 200)
                            {
                                var otpmodel = new OTPRequestModel();
                                otpmodel.Id = common.Id;
                                otpmodel.OTP = otp;
                                otpmodel.Flag = "UpdateOTP";
                                var otpres = await _buss.UpdateOTP(otpmodel);
                                return Redirect("/Home/VerifyOTP/" + common.Id);
                            }
                            else
                            {
                                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = "Unable to send otp. Please try again." };
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = "Unable to login" };
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to login" : res.Result.Message };
                        return View(model);
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(x=>x.Errors).Select(x=>x.ErrorMessage));
                    ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = errors };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return View(model);
            }
        }
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel();
            model.UserTypeList = new List<SelectListItem>() 
            { 
                new SelectListItem() { Text = "Select User Type", Value = "" },
                new SelectListItem() { Text = "Admin", Value = "Admin" },
                new SelectListItem() { Text = "User", Value = "User" }
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateReCaptcha]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var error = StaticHelper.CheckPasswordPolicy(model.Password);
                    if (error?.ErrorCode == 400)
                    {
                        ModelState.AddModelError("Password", error.Message);
                    }
                }
                if (ModelState.IsValid)
                {
                    var req = new RegisterRequestModel();
                    req.FullName = model.FullName;
                    req.UserType = model.UserType;
                    req.Email = model.Email;
                    req.Password = StaticHelper.EncryptString(model.Password);
                    req.Flag = "UserRegister";
                    var res = await _buss.UserRegister(req);
                    if (res?.Result?.ErrorCode == 200)
                    {
                        TempData["Response"] =JsonConvert.SerializeObject(res.Result);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to register" : res.Result.Message };
                        return View(model);
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = errors };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return View(model);
            }
        }
        public async Task<IActionResult> VerifyOTP(string id)
        {
            var model = new OTPViewModel();
            model.Id = id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOTP(OTPViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var req = new OTPRequestModel();
                    req.Id = model.Id;
                    req.OTP = model.OTP;
                    req.Flag = "VerifyOTP";
                    var res = await _buss.VerifyOTP(req);
                    if (res?.Result?.ErrorCode == 200)
                    {
                        if(res?.ResultCommon != null)
                        {
                            var common = (LoginResponseModel)res.ResultCommon;
                            var userinstance = new LoggedInUser()
                            {
                                UserId = common.Id,
                                FullName = common.FullName,
                                Email = common.Email,
                                UserType = common.UserType,
                                PasswordExpiryDate = common.PasswordExpiryDate,
                                ForcePasswordChange = Convert.ToBoolean(common.ForcePasswordChange),
                            };
                            UserInstance.GetInstance().AddUser(userinstance);
                            HttpContext.Session.SetString("UserId", common.Id);
                            TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 200, Message = "Login Successful." });
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = "Unable to verify otp" };
                            return View("VerifyOTP", model);
                        }
                    }
                    else
                    {
                        ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to verify otp" : res.Result.Message };
                        return View("VerifyOTP", model);
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = errors };
                    return View("VerifyOTP", model);
                }
            }
            catch (Exception ex)
            {
                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return View("VerifyOTP", model);
            }
        }
        public async Task<IActionResult> ForgotPassword()
        {
            if (TempData != null)
            {
                if (TempData.ContainsKey("Response"))
                {
                    if (TempData["Response"] != null)
                    {
                        var response = JsonConvert.DeserializeObject<BaseResponseModel>(TempData["Response"].ToString());
                        ViewData["Response"] = response;
                    }
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var req = new ForgotPasswordRequestModel();
                    req.Email = model.Email;
                    req.Flag = "GetUserDetailForForgotPassword";
                    var res = await _buss.GetUserDetailForForgotPassword(req);
                    if(res?.Result?.ErrorCode == 200)
                    {
                        if(res?.ResultCommon != null)
                        {
                            var common = (UserResponseModel)res.ResultCommon;
                            var token = StaticHelper.GetToken();
                            var link = (Request.IsHttps == true ? "https" : "http") + "://" + Request.Host + $@"/Home/ResetPassword?id={common.Id}&token={token}";
                            var emailcommon = new EmailHelperCommon();
                            emailcommon.ToEmailAddress = common.Email;
                            emailcommon.ToFullName = common.FullName;
                            emailcommon.Message = $@"Your Forgot Password link is <a href=""{link}"">Click here</a> ";
                            emailcommon.Subject = "Forgot Password";
                            emailcommon.FromEmailAddress = _config["MailSettings:Mail"].ToString();
                            emailcommon.SmtpUsername = _config["MailSettings:Mail"].ToString();
                            emailcommon.SmtpPassword = _config["MailSettings:Password"].ToString();
                            emailcommon.SmtpPort = _config["MailSettings:Port"].ToString();
                            emailcommon.SmtpServer = _config["MailSettings:Host"].ToString();
                            emailcommon.UseDefaultCredentials = _config["MailSettings:UseDefaultCredentials"].ToString();
                            emailcommon.EnableSsl = _config["MailSettings:EnableSsl"].ToString();
                            var mailresponse = await StaticHelper.SendEmailAsync(emailcommon);
                            if (mailresponse?.ErrorCode == 200)
                            {
                                var tokenmodel = new TokenRequestModel();
                                tokenmodel.Id = common.Id;
                                tokenmodel.Token = token;
                                tokenmodel.Flag = "UpdateToken";
                                var tokenres = await _buss.UpdateToken(tokenmodel);
                                TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 200, Message = "Forgot password successfully." });
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = "Unable to send token. Please try again." };
                                return View(model);
                            }

                        }
                        else
                        {
                            ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = "Unable to forgot password" };
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to forgot password" : res.Result.Message };
                        return View(model);
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = errors };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewData["Response"] = new BaseResponseModel() { ErrorCode = 500, Message = ex.Message };
                return View(model);
            }
        }
        public async Task<IActionResult> ResetPassword(string id, string token)
        {
            if (TempData != null)
            {
                if (TempData.ContainsKey("Response"))
                {
                    if (TempData["Response"] != null)
                    {
                        var response = JsonConvert.DeserializeObject<BaseResponseModel>(TempData["Response"].ToString());
                        ViewData["Response"] = response;
                    }
                }
            }
            try
            {
                if(!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(token))
                {
                    var req = new ResetPasswordDetailRequestModel();
                    req.Id = id;
                    req.Token = token;
                    req.Flag = "GetUserDetailForResetPassword";
                    var res = await _buss.GetResetPasswordUserDetail(req);
                    if(res?.Result?.ErrorCode == 200)
                    {
                        if(res?.ResultCommon != null)
                        {
                            var common = (UserResponseModel)res.ResultCommon;
                            var model = new ResetPasswordViewModel();
                            model.Id = common.Id;
                            model.Token = token;
                            model.FullName = common.FullName;
                            return View(model);
                        }
                        else
                        {
                            TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 400, Message = "Unable to get detail" });
                            return RedirectToAction("ForgotPassword");
                        }
                    }
                    else
                    {
                        TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to get detail" : res.Result.Message });
                        return RedirectToAction("ForgotPassword");
                    }
                }
                else
                {
                    TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 500, Message = "Id or token not found." });
                    return RedirectToAction("ForgotPassword");
                }
            }
            catch(Exception ex)
            {
                TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 500, Message = ex.Message });
                return RedirectToAction("ForgotPassword");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateReCaptcha]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var error = StaticHelper.CheckPasswordPolicy(model.NewPassword);
                    if (error?.ErrorCode == 400)
                    {
                        ModelState.AddModelError("NewPassword", error.Message);
                    }
                }
                if (ModelState.IsValid)
                {
                    var req = new ResetPasswordRequestModel();
                    req.Id = model.Id;
                    req.Token = model.Token;
                    req.NewPassword = StaticHelper.EncryptString(model.NewPassword);
                    req.Flag = "ResetPassword";
                    var res = await _buss.ResetPassword(req);
                    if (res?.Result?.ErrorCode == 200)
                    {
                        TempData["Response"] = JsonConvert.SerializeObject(res.Result);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to reset password" : res.Result.Message });
                        return Redirect("/Home/ResetPassword?id=" + model.Id + "&token=" + model.Token);
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 400, Message = errors });
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 500, Message = ex.Message });
                return RedirectToAction("Index");
            }
        }
        public IActionResult LogOut()
        {
            if (TempData["Response"] != null)
            {
                var response = JsonConvert.DeserializeObject<BaseResponseModel>(TempData["Response"].ToString());
                TempData["Response"] = JsonConvert.SerializeObject(response);
            }
            else
            {
                TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 400, Message = "Logout successful!" });
            }
            GetInstance().RemoveUser(StaticHelper.GetUserId());
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
