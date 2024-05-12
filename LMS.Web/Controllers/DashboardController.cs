using LMS.Domain.Entities.Domain;
using LMS.Domain.Entities.Domain.User.RequestModel;
using LMS.Domain.Entities.ViewModels.User;
using LMS.Domain.HelperClass;
using LMS.Domain.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        IUserBusiness _buss;
        public DashboardController(IUserBusiness _buss)
        {
            this._buss = _buss;
        }
        public async Task<IActionResult> Index()
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
            var res = await _buss.GetTotalUser();
            if(res?.Result?.ErrorCode == 200)
            {
                if(res?.ResultCommon != null)
                {
                    ViewData["TotalUser"] = res.ResultCommon;
                }
            }
            return View();
        }
        public async Task<IActionResult> ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            model.FullName = StaticHelper.GetUserFullName();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
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
                    var req = new ChangePasswordRequestModel();
                    req.Id = StaticHelper.GetUserId();
                    req.NewPassword = StaticHelper.EncryptString(model.NewPassword);
                    req.Flag = "ChangePassword";
                    var res = await _buss.ChangePassword(req);
                    if (res?.Result?.ErrorCode == 200)
                    {
                        TempData["Response"] = JsonConvert.SerializeObject(res.Result);
                        return RedirectToAction("LogOut", "Home");
                    }
                    else
                    {
                        ViewData["Response"] = new BaseResponseModel() { ErrorCode = 400, Message = string.IsNullOrEmpty(res?.Result?.Message) ? "Unable to change password" : res.Result.Message };
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
                TempData["Response"] = JsonConvert.SerializeObject(new BaseResponseModel() { ErrorCode = 500, Message = ex.Message });
                return RedirectToAction("Index");
            }
        }
    }
}
