using SimpleLoginMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleLoginMvc.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.    
                //if (Request.IsAuthenticated)
                if (Session["Username"] != null)
                {
                    // Info.    
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {

            }
            // Info.    
            return View();
        }






        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel user, string returnUrl)
        {
            ViewResult viewResult = View(user);
            try
            {

                UserAccount userAccount = null;
                // this action is for handle post (login)
                if (ModelState.IsValid) // this is check validity
                {
                    if (user != null)
                    {
                        //validate user
                        userAccount = ModelCollections.userAccounts.Find(x => x.Username == user.Username);
                        // var index = ModelCollections.userAccounts.FindIndex(x => x.Username == user.Username); 
                        //here index is ID in userAccount so the line above is not necessary

                        if (userAccount != null)
                        {
                            if (userAccount.Username != null)
                            {
                                if (userAccount.UserPassword != null)
                                {
                                    //decrypt found password if necessary and compare it to the one provided in login
                                    if (user.Password == userAccount.UserPassword)
                                    {
                                        Session["UserRole"] = userAccount.Role;
                                        Session["Username"] = userAccount.Username;
                                        Session["UserID"] = userAccount.ID;
                                        Session["FullName"] = userAccount.Name + " " + userAccount.LastName;
                                       
                                        ActivityString activity = LogActivity.LoginUser();
                                        int.TryParse(Session["UserID"].ToString(), out int id);

                                        int recordID = ModelCollections.userActivityLogs.Count + 1;
                                        ModelCollections.userActivityLogs.Add(new UserActivityLog()
                                        {
                                            ID = recordID,
                                            UserAccountID = id,
                                            Username = Session["Username"].ToString(),
                                            Activity = activity.Activity,
                                            ActivityDate = DateTime.Now,
                                            ActivityDescription = activity.ActivityDescription,
                                            Error = "", //if there was an error, you can report it here as well
                                            Source = System.Environment.MachineName
                                        });

                                        //Add login date to user who logged in
                                        ModelCollections.userAccounts[userAccount.ID].LastLoginDate = DateTime.Now;

                                        return RedirectToLocal(returnUrl);
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(String.Empty, "Incorrect password");
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "Username does not exist");

                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "User account does not exist");

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(String.Empty, "Error. Check log for details."); //there should be a log

            }
            return viewResult;
        }




        /// <summary>    
        /// POST: /Account/LogOff    
        /// </summary>    
        /// <returns>Return log off action</returns>    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {

                ActivityString activity = LogActivity.LogoutUser();
                int.TryParse(Session["UserID"].ToString(), out int id);

                int recordID = ModelCollections.userActivityLogs.Count + 1;
                ModelCollections.userActivityLogs.Add(new UserActivityLog()
                {
                    ID = recordID,
                    UserAccountID = id,
                    Username = Session["Username"].ToString(),
                    Activity = activity.Activity,
                    ActivityDate = DateTime.Now,
                    ActivityDescription = activity.ActivityDescription,
                    Error = "", //if there was an error, you can report it here as well
                    Source = System.Environment.MachineName
                });

                Session["UserRole"] = null;
                Session["Username"] = null;
                Session["UserID"] = null;

            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Login", "Login");
        }

        /// <summary>    
        /// Redirect to local method.    
        /// </summary>    
        /// <param name="returnUrl">Return URL parameter.</param>    
        /// <returns>Return redirection action</returns>    
        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.    
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.    
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Home", "Home");
        }

    }
}
