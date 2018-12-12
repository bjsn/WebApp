using System;
using System.Web.Mvc;
using Corspro.Business.External;
using Corspro.Domain.Dto;
using Corspro.Reporting.App.Models;
using System.Net.Mail;

namespace Corspro.Reporting.App.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            var userBl = new UserBL();
            var tempPwd = Utils.Encrypt(model.Password);
            var userDto = new UserDto { LoginID = model.UserName, Password = tempPwd };
            var validatedUser = userBl.ValidateUser(userDto);

            if (ModelState.IsValid && validatedUser != null)
            {
                Session["AuthenticatedUser"] = validatedUser;
                return RedirectToLocal();
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        public ActionResult LogOff()
        {
            Session["AuthenticatedUser"] = null;
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        public ActionResult Manage(LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (true)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        var authUser = (UserDto)Session["AuthenticatedUser"];
                        var userBl = new UserBL();
                        if (authUser.Password.Equals(Utils.Encrypt(model.OldPassword)))
                        {
                            authUser.Password = Utils.Encrypt(model.NewPassword);
                            changePasswordSucceeded = userBl.ResetPassword(authUser) > 0;
                        }
                        else
                        {
                            changePasswordSucceeded = false;
                        }
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            //else
            //{
            //    // User does not have a local password so remove any validation errors caused by a missing
            //    // OldPassword field
            //    ModelState state = ModelState["OldPassword"];
            //    if (state != null)
            //    {
            //        state.Errors.Clear();
            //    }

            //    if (ModelState.IsValid)
            //    {
            //        try
            //        {
            //            WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
            //            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
            //        }
            //        catch (Exception e)
            //        {
            //            ModelState.AddModelError("", e);
            //        }
            //    }
            //}

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ForgotPassword(FormCollection oCollection)
        {
            try
            {
                string email = "";
                foreach (var key in oCollection.AllKeys)
                {
                    if (key == "email")
                    {
                        email = oCollection[key];
                    }
                }
                if (!String.IsNullOrEmpty(email))
                {
                    var userBl = new UserBL();
                    userBl.updateEmailRequested(email);
                    ViewData["EmailSent"] = "An email has been successfully sent, please check.";

                    /*if (User != null)
                    {
                        var password = Utils.Encrypt(User.Password);
                        try
                        {
                            MailMessage mail = new MailMessage("support@corspro.com", email);
                            SmtpClient client = new SmtpClient("smtp.corspro.com", 25);
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.UseDefaultCredentials = false;
                            mail.Subject = "SDA Cloud password and username";
                            mail.Body = "Your SDA Cloud username is " + u.LoginID + " and your password is " + tmpPwd + ".\n\n" +
                                   "If you have not yet installed the SDA Cloud desktop application, please download and install it from http://corspro.fileburst.com/x3ph9vj8xl4b/Setup_SDA_Cloud.exe.  To register as a new SDA Cloud user – and connect your desktop with the SDA Cloud – please open SalesDoc Architect (in Excel) and click Add-Ins, Architect, Setup, 'Register as SDA Cloud user', and then enter your username (i.e., email address) and password when prompted.\n\n" +
                                   "If you’re a new SDA Cloud user, please go to http://www.corspro.com/support/sda-cloud/ for help with getting started.\n\n" +
                                   "For assistance, please contact your administrator or send an email to support@corspro.com.\n\n";
                            client.Send(mail);
                            RedirectToAction("Login");
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("CustomError", e.Message);
                            RedirectToAction("Login");
                        }
                    }
                    else 
                    {
                        ModelState.AddModelError("CustomError", "there is not user");
                    }*/
                    RedirectToAction("Login");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("CustomError", "The email should not be blank or null");
            }
            return View();
        }


        #region Helpers
        private ActionResult RedirectToLocal()
        {
            return RedirectToAction("OpportunityManagement", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}
