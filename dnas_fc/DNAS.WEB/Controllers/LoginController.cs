using DNAS.Application.Common.Implementation;
using DNAS.Application.Common.Interface;
using DNAS.Application.Features.Login;
using DNAS.Application.Features.MailSend;
using DNAS.Application.IService;
using DNAS.Domain.Common;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;

using DNTCaptcha.Core;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using System.Security.Claims;
namespace DNAS.WEB.Controllers
{

    public class LoginController(ISender iSender, ICustomLogger logger, IEncryption encryption, ICaptchaService captchaService, IOptions<AppConfig> appConfig) : Controller
    {
        private readonly ISender _iSender = iSender;
        private readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly string _logfilename = "Login";
        private readonly string _recoverlogfilename = "RecoverPassword";
        private readonly string _redirecturl = "/Login";
        private readonly ICaptchaService _captchaService = captchaService;

        public async Task<IActionResult> Index(int l = 6, string bc = "Lavender", string fc = "Black", CharType t = CharType.MIX)
        {
            UserMasterModel request = new()
            {
                UserName = (TempData.ContainsKey("Username") ? TempData?["Username"].ToString() : string.Empty)!.ToString()
            };
            string randomStr = _captchaService.GenerateRandomString(l, t);
            HttpContext.Session.SetString("captcha", randomStr);
            try
            {
                if (HttpContext.Request.Query["ls"].ToString() != "")
                {
                    if (TempData["encdata"] == null)
                    {
                        return View(request);
                    }

                    UserMasterModel Request = new();
                    Request.UserId = Convert.ToInt32(_encryption.AesDecrypt(HttpContext.Request.Query["ls"].ToString(), TempData["encdata"]?.ToString() ?? ""));
                    UserMasterModel Response = await _iSender.Send(new UpdateUserTrackingCommand(Request));
                    if (Response.SessionId == "")
                    {
                        return View(request);
                    }
                    else
                    {
                        _logger.LogwriteInfo("new User tracking Update successful in case user already have cookie", $"User_{Request.UserId}");
                        string? Name = string.Concat(Response.FirstName + " ", Response.MiddleName + " ", Response.LastName);
                        List<Claim> claims =
                        [
                            new(ClaimTypes.NameIdentifier, Response.UserName),
                            new(ClaimTypes.Role,Response.Department??""),
                            new(ClaimTypes.UserData,Response.FirstName??""),
                            new(ClaimTypes.Name, Name),
                            new(ClaimTypes.Email, Response.Email??"--"),
                            new("LoginTime", DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt")),
                            new("UserId", Response.UserId.ToString()??""),
                            new("SessionId", Response.SessionId??""),
                        ];
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        _ = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                            IsPersistent = true,
                            AllowRefresh = true
                        });
                        return Redirect("/Dashboard");

                    }
                }
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    UserMasterModel Request = new();
                    Request.UserId = Convert.ToInt32(userId);
                    UserMasterModel Response = await _iSender.Send(new UpdateUserTrackingCommand(Request));
                    if (Response.SessionId != "")
                    {
                        _logger.LogwriteInfo("new User tracking Update successful in case user already have cookie", $"User_{userId}");

                        List<Claim> claims =
                        [
                            new(ClaimTypes.NameIdentifier, User.FindFirstValue(ClaimTypes.NameIdentifier)??""),
                            new(ClaimTypes.Role,User.FindFirstValue(ClaimTypes.Role)??""),
                            new(ClaimTypes.UserData,User.FindFirstValue(ClaimTypes.UserData)??""),
                            new(ClaimTypes.Name, User.Identity?.Name??""),
                            new(ClaimTypes.Email, User.FindFirstValue(ClaimTypes.Email)??""),
                            new("LoginTime", DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt")),
                            new("UserId", User.FindFirstValue("UserId")??""),
                            new("SessionId", Response.SessionId??""),
                    ];
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        _ = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                            IsPersistent = true,
                            AllowRefresh = true
                        });

                        return Redirect("/Dashboard");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during Login page load" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _logfilename);
            }
            var captchaImage = _captchaService.CreateCAPTCHAImage(randomStr, bc, getforecolor());
            string captchaBase64 = Convert.ToBase64String(captchaImage);
            ViewData["CaptchaImage"] = "data:image/png;base64," + captchaBase64;
            return View(request);
        }

        [HttpPost]
        [ValidateDNTCaptcha(ErrorMessage = "Please Enter Valid Captcha")]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Index(UserMasterModel Request)
        {
            Request.UserName = _encryption.DecryptStringAES(Request.NameAseUsr);
            Request.Password = _encryption.DecryptStringAES(Request.PasAseUsr);

            #region Captcha Validation
            var captchaCode = HttpContext.Session.GetString("captcha");
            if (captchaCode != Request.captchaCode)
            {
                TempData["Captcha"] = "Please Enter Valid Captcha";
                TempData["Username"] = Request.UserName;

                return RedirectToAction("Index", "Login");
            }
            #endregion

            if (Request.UserName == null || Request.Password == null)
            {
                TempData["LoginFailed"] = "Please fill required details";
                return View();
            }
            try
            {
                CommonResponse<UserMasterResponse> Response = new();

                Request.UserName = _encryption.DecryptStringAES(Request.NameAseUsr);
                Request.Password = _encryption.DecryptStringAES(Request.PasAseUsr);

                if (Request.LoginType == "Single")      //LDAP Login Check
                {
                    _logger.LogwriteInfo("User select Ldap login for entering into DNAS portal. UserName" + Request.UserName, _logfilename);
                    Response = await _iSender.Send(new CheckLdapLoginCommand(Request));
                }
                else          //Other Login check
                {
                    _logger.LogwriteInfo("User select General login for entering into DNAS portal. UserName" + Request.UserName, _logfilename);
                    Response = await _iSender.Send(new CheckLoginQueryCommand(Request));
                }

                if (Response.ResponseStatus.ResponseCode != 200)
                {
                    _logger.LogwriteInfo("Login attempt using invalid credential. You try with Username- " + Request.UserName, _logfilename);
                    TempData["LoginFailed"] = "Invalid Credentials";
                    TempData["Username"] = Request.UserName;
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    #region OTP Page
                    if (!string.IsNullOrWhiteSpace(Response.Data.Email))
                    {
                        TempData["MaskEmail"] = "xx" + new string('x', 2) + Response.Data.Email.Substring(4).Split('@')[0] + "@" + new string('x', Response.Data.Email.Split('@')[1].Length);
                        TempData.Keep("MaskEmail");
                    }
                    else
                    {
                        _logger.LogwriteInfo("Email not found in database of user-" + Request.UserName, _logfilename);
                    }
                    HttpContext.Session.SetString("OTPEmail", Response.Data.Email);
                    HttpContext.Session.SetString("UserName", Request.NameAseUsr);
                    HttpContext.Session.SetString("Password", Request.PasAseUsr);
                    try
                    {

                        ViewData["MailOtpValidateTime"] = Convert.ToInt64(appConfig.Value.MailOtpValidateTime);

                        OtpResponseModel Request1 = new()
                        {
                            Email = HttpContext.Session.GetString("OTPEmail") ?? throw new InvalidOperationException("Email not found.")
                        };
                        bool Response1 = await _iSender.Send(new SendOtpCommandHandler(Request1));
                        if (!Response1)
                        {
                            TempData["LoginFailed"] = "OTP Send Failed.";
                            return View();
                        }
                        _logger.LogwriteInfo("Otp send successful and" + Environment.NewLine + "Email-" + Request1.Email, _logfilename);
                    }
                    catch (Exception e)
                    {
                        _logger.LogwriteInfo("exception occur during otp send execution" + Environment.NewLine + "exception message-" + e.Message + Environment.NewLine + e.StackTrace, _logfilename);
                        return View();
                    }
                    _logger.LogwriteInfo("Email exists for the userid- " + Response.Data?.UserId.ToString() + Environment.NewLine + "Email is-" + Response.Data?.Email, _logfilename);
                    return RedirectToAction("Otp", "Login");
                    #endregion
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during login-----exception is--" + e.Message + Environment.NewLine + e.StackTrace, _logfilename);
                TempData["Username"] = Request.UserName;
                return View();
            }
        }


        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            string userId = User.FindFirstValue("UserId") ?? "";
            UserMasterModel user = new();
            user.UserId = Convert.ToInt32(userId);
            await _iSender.Send(new RemoveUserTrackingCommand(user));
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index");
        }
        [Authorize]
        public async Task<IActionResult> LogoutCurrentAuth()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index");
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordModel Request)
        {
            try
            {
                if (!ModelState.IsValid || Request == null)
                {
                    TempData["errormsg"] = CommonMsg.fillValidMail;
                    return Redirect("/Login/RecoverPassword");
                }
                CommonResponse<RecoverPasswordResponse> Response = await _iSender.Send(new FetchRecoveryPasswordCommand(Request));
                if (Response.Data.UserId != 0 && Response.ResponseStatus.ResponseCode is 200)
                {
                    CommonResponse<RecoverPasswordResponse> Response1 = await _iSender.Send(new RecoveryPasswordMailSend(Response.Data));
                    if (Response1 != null && Response1.ResponseStatus?.ResponseCode is 200)
                    {
                        TempData["ismailsend"] = true;
                        TempData["successmsg"] = $"Mail successfully send to your registered mail id : {Request.Email}";
                        _logger.LogwriteInfo("Mail successfully send along with login url and predefined password to the registered email------", _recoverlogfilename);
                    }
                    else
                    {
                        TempData["ismailsend"] = true;
                        TempData["successmsg"] = $"Mail not send to the registered mail id. :{Request.Email}";
                        _logger.LogwriteInfo("Mail not send to the registered email------", _recoverlogfilename);
                    }
                }
                else if (Response.ResponseStatus.ResponseCode == 400)
                {
                    TempData["errormsg"] = CommonMsg.RecoveryPasswordTryBeforeTime;
                    _logger.LogwriteInfo($"mail not send to Emailid --{Request.Email}", _recoverlogfilename);
                }
                else
                {
                    TempData["errormsg"] = CommonMsg.MailnotExists;
                    _logger.LogwriteInfo($"Emailid not Exists--{Request.Email}", _recoverlogfilename);
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during RecoverPassword execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _recoverlogfilename);
            }

            return View();
        }

        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                UserVerivicationReqModel Request = new();
                if (HttpContext.Request.Query["st"].ToString() != "")
                {
                    Request.UserId = HttpContext.Request.Query["st"].ToString();

                    CommonResponse<ChangePasswordModel> Response = await _iSender.Send(new UserVerificationCommand(Request));
                    if (Response != null && Response.ResponseStatus.ResponseMessage == "Data Found")
                    {
                        TempData["qsv"] = HttpContext.Request.Query["st"].ToString();
                        TempData.Keep();
                        return View(Response.Data);
                    }
                    else
                    {
                        TempData["errormsg"] = CommonMsg.PasswordLinkExpire;
                        return Redirect("/login/RecoverPassword");
                    }
                }
                else
                {
                    return Redirect("/login/RecoverPassword");
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during ChangePassword load" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _recoverlogfilename);
                return Redirect("/login/RecoverPassword");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel Request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CommonResponse<UserMasterResponse> Response = await _iSender.Send(new LoginChangePasswordCommand(Request));
                    if (Response.ResponseStatus.ResponseCode == 200)
                    {
                        UserMasterModel user = new();
                        user.UserId = Convert.ToInt32(Request.UserId);
                        await _iSender.Send(new RemoveUserTrackingCommand(user));

                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        TempData["message"] = CommonMsg.PasswordChanged;
                        return View();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(TempData["qsv"]?.ToString()))
                        {
                            TempData["message"] = "Failed";
                            return Redirect("/login/changepassword?st=" + TempData["qsv"]?.ToString()?.Replace("=", "%3D"));
                        }
                        else
                        {
                            TempData["message"] = CommonMsg.PreDefinePasswordNotMatch;
                            return View();
                        }
                    }
                }
                else
                {
                    _logger.LogwriteInfo("Incorrect Input for change password form submit------", _recoverlogfilename);
                    return View(Request);
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during ChangePassword execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _recoverlogfilename);
            }
            return View();
        }

        public async Task<JsonResult> DeleteUsertrackingFunction()
        {
            string str = "Failed";
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                UserMasterModel user = new();
                user.UserId = Convert.ToInt32(userId);
                if (await _iSender.Send(new RemoveUserTrackingCommand(user)))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    str = "success";
                }
            }
            catch (Exception ex)
            {
                str = "Failed";
                _logger.LogwriteInfo("exception occur during DeleteUsertrackingFunction execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _logfilename);
            }

            return Json(str);
        }
        public bool CreateSession(UserTrackingModel model)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during CreateSession-----exception is--" + e.Message + Environment.NewLine + e.StackTrace, _logfilename);
                return false;
            }

        }

        public string getforecolor()
        {
            Random random = new Random();
            int oneDigitNumber = random.Next(0, 9);

            string[] colors = { "Red", "Blue", "Green", "Tomato", "Teal", "Purple", "Brown", "Black", "Magenta", "Indigo" };
            return colors[oneDigitNumber];
        }
        public async Task<IActionResult> RefreshCaptcha()
        {
            try
            {
                string randomStr = _captchaService.GenerateRandomString(6, CharType.MIX); // Adjust the length and type as needed
                HttpContext.Session.SetString("captcha", randomStr);
                string forecolour = getforecolor();
                //var captchaImage = _captchaService.CreateCAPTCHAImage(randomStr, "transparent", "random");
                var captchaImage = _captchaService.CreateCAPTCHAImage(randomStr, "Lavender", forecolour);
                string captchaBase64 = Convert.ToBase64String(captchaImage);
                return Json(new { captchaImage = "data:image/png;base64," + captchaBase64 });
            }
            catch (Exception e)
            {
                // Log the exception for debugging
                _logger.LogwriteInfo
                    ($"Exception occurred during OTP verification.{Environment.NewLine}" +
                    $"Exception message: {e.Message}{Environment.NewLine}{e.StackTrace}", _logfilename);
                return Json(new { success = false, message = "An error occurred while refresh the captcha. Please try again later." });
            }

        }
        [HttpGet]
        public async Task<IActionResult> Otp()
        {
            TempData.Keep("MaskEmail");
            ViewData["MailOtpValidateTime"] = Convert.ToInt64(appConfig.Value.MailOtpValidateTime);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> VerifyOtp([FromBody] OtpResponseModel otprequest)
        {
            try
            {
                CommonResponse<OtpResponseModel> response = await _iSender.Send(new VerifyOtpCommandHandler(otprequest));
                // Log relevant information
                if (response.Data == null)
                {
                    return Json(new { success = false, message = "An error occurred while verifying the OTP. Please try again later." });
                }
                _logger.LogwriteInfo($"OTP verification execution:{Environment.NewLine}OTP: {response.Data.OTP}{Environment.NewLine}Message: {response.Data.Message}", _logfilename);
                // Check if the response indicates success
                if (response.Data.Success)
                {
                    UserMasterModel Request = new();
                    if (HttpContext.Session.GetString("UserName") != null && HttpContext.Session.GetString("Password") != null)
                    {
                        Request.Password = _encryption.DecryptStringAES(HttpContext.Session.GetString("Password"));
                        Request.UserName = _encryption.DecryptStringAES(HttpContext.Session.GetString("UserName"));
                    }
                    CommonResponse<UserMasterResponse> Response = await _iSender.Send(new FetchLoginQueryCommand(Request));
                    if (Response.ResponseStatus.ResponseCode == 300)
                    {
                        TempData["encdata"] = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        TempData.Keep("encdata");
                        string url = "/Login?ls=" + _encryption.AesEncrypt(Response.Data.UserId.ToString(), TempData["encdata"]?.ToString() ?? "");
                        TempData["LoginFailed"] = "Previous session still exists." + Environment.NewLine + Response.Data.LastloginTime + Environment.NewLine + " Please clear previous session and login. <br> <b><a href=" + url + ">Click Here</a></b>";
                        return Json(new { success = true, message = response?.Message ?? "Invalid OTP. Please try again.", redirectUrl = _redirecturl });
                    }
                    else if (Response.ResponseStatus.ResponseCode != 200)
                    {
                        TempData["LoginFailed"] = "Invalid Credentials";
                        return Json(new { success = false, message = response?.Message ?? "Invalid OTP. Please try again.", redirectUrl = _redirecturl });
                    }

                    string? Name = string.Concat(Response.Data?.FirstName + " ", Response.Data?.MiddleName + " ", Response.Data?.LastName);
                    string? Email = Response.Data?.Email != null ? "xx" + new string('x', 2) + Response.Data.Email.Substring(4).Split('@')[0] + "@" + new string('x', Response.Data.Email.Split('@')[1].Length) : null;

                    List<Claim> claims =
                    [
                        new(ClaimTypes.NameIdentifier, Response.Data!.UserName),
                    new(ClaimTypes.Role,Response.Data?.Department??""),
                    new(ClaimTypes.UserData,Response.Data?.FirstName??""),
                    new(ClaimTypes.Name, Name),
                    new(ClaimTypes.Email, Response.Data?.Email??"--"),
                    new("LoginTime", DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt")),
                    new("UserId", Response.Data?.UserId.ToString()??""),
                    new("SessionId", Response.Data?.SessionId??""),
                    new("Email", Email??"Null"),
                    new("Usermail", Response.Data?.Email??"--"),
                    new("UserName",Name),
                ];
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    _ = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                        IsPersistent = true,
                        AllowRefresh = true
                    });

                    string RedirectURL = string.Empty;
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RedirectFromView")))
                    {
                        RedirectURL = HttpContext.Session.GetString("RedirectFromView")!;
                        HttpContext.Session.SetString("RedirectFromView", "");
                        return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = "/Note" + RedirectURL });
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RedirectFromApprovalRequest")))
                    {
                        RedirectURL = HttpContext.Session.GetString("RedirectFromApprovalRequest")!;
                        HttpContext.Session.SetString("RedirectFromApprovalRequest", "");
                        return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = "/Note" + RedirectURL });
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RedirectFromWithdraw")))
                    {
                        RedirectURL = HttpContext.Session.GetString("RedirectFromWithdraw")!;
                        HttpContext.Session.SetString("RedirectFromWithdraw", "");
                        return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = "/Note" + RedirectURL });
                    }

                    return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = "/Dashboard" });
                }
                return Json(new { success = false, message = response?.Message ?? "Invalid OTP. Please try again." });
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo($"Exception during OTP verification.{Environment.NewLine}Message: {e.Message}{Environment.NewLine}{e.StackTrace}", _logfilename);
                return Json(new { success = false, message = "An error occurred while verifying the OTP. Please try again later." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> resendOtp()
        {
            TempData.Keep("MaskEmail");
            try
            {

                ViewData["MailOtpValidateTime"] = Convert.ToInt64(appConfig.Value.MailOtpValidateTime);
                if (HttpContext.Session.GetString("OTPEmail") == null)
                {
                    TempData["LoginFailed"] = "Email not found.";
                    return Redirect(_redirecturl);
                }
                OtpResponseModel Request = new()
                {
                    Email = HttpContext.Session.GetString("OTPEmail") ?? throw new InvalidOperationException("Email not found.")
                };
                bool Response = await _iSender.Send(new SendOtpCommandHandler(Request));
                if (!Response)
                {
                    TempData["LoginFailed"] = "OTP Send Failed.";
                    return Redirect(_redirecturl);
                }
                _logger.LogwriteInfo("Otp send successful and" + Environment.NewLine + "Email-" + Request.Email, _logfilename);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during otp send execution" + Environment.NewLine + "exception message-" + e.Message + Environment.NewLine + e.StackTrace, _logfilename);
                return Redirect(_redirecturl);
            }
            return Redirect("/Login/Otp");
        }

        [HttpGet]
        public IActionResult GetRegexPatternForClientSideValidation()
        {
            //return Json(new { response = appConfig.Value?.AllowedCharacterForJs });
            return Json(appConfig.Value?.AllowedCharacterForJs);
        }
    }
}
