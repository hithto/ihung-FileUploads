using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using jQuery_Form_Plugin.Models;

namespace jQuery_Form_Plugin.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "所提供的使用者名稱或密碼不正確。");
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 嘗試註冊使用者
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // 在某些失敗狀況中，ChangePassword 會擲回例外狀況，
                // 而不是傳回 false。
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "目前密碼不正確或是新密碼無效。");
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 請參閱 http://go.microsoft.com/fwlink/?LinkID=177550 了解
            // 狀態碼的完整清單。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "使用者名稱已經存在。請輸入不同的使用者名稱。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "該電子郵件地址的使用者名稱已經存在。請輸入不同的電子郵件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "所提供的密碼無效。請輸入有效的密碼值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "所提供的電子郵件地址無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "所提供的密碼擷取解答無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "所提供的密碼擷取問題無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.InvalidUserName:
                    return "所提供的使用者名稱無效。請檢查這項值，然後再試一次。";

                case MembershipCreateStatus.ProviderError:
                    return "驗證提供者傳回錯誤。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";

                case MembershipCreateStatus.UserRejected:
                    return "使用者建立要求已取消。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";

                default:
                    return "發生未知的錯誤。請確認您的輸入，然後再試一次。如果問題仍然存在，請聯繫您的系統管理員。";
            }
        }
        #endregion
    }
}
