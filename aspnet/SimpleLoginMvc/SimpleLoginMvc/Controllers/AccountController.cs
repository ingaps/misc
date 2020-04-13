using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleLoginMvc.Helpers;
using SimpleLoginMvc.Models;

namespace SimpleLoginMvc.Controllers
{
    public class AccountController : Controller
    {
        [CustomAuthorize("Admin")]
        [HandleError]
        // GET: Account
        public ActionResult Index()
        {
            try
            {
                return View(ModelCollections.userAccounts);
            }
            catch (Exception ex)
            {
                //TODO: write to log, report error
                return View();

            }
        }

        [CustomAuthorize("Admin")]
        [HandleError]
        // GET: Account
        public ActionResult UserActivityLogView()
        {
            try
            {
                return View(ModelCollections.userActivityLogs);
            }
            catch (Exception ex)
            {
                //TODO: write to log, report error
                return View();

            }
        }


        // GET: Account/Create
        [CustomAuthorize("Admin")]
             [HandleError]
             public ActionResult Create()
             {
                 return View();
             }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Username,UserPassword,ConfirmPassword,Name,LastName,Role,LastLoginDate")] UserAccount userAccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //it is advisable to encrypt password before storing it
                    //in this example no encryption will be used

                    //if db is used, insert record to db here
                    userAccount.ID = ModelCollections.userAccounts.Count + 1;
                    ModelCollections.userAccounts.Add(userAccount);

                    //log user activity
                    //any logger can be used as well, this a custom example
                    //adds data to a collection, this can be written to db directly or to a log file
                    ActivityString activity = LogActivity.CreateUser();
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

                    return RedirectToAction("Index");
                }
                return View(userAccount);
            }
            catch (Exception ex)
            {
                return View();
            }
        }



        // GET: Account/Delete/5
        [CustomAuthorize("Admin")]
        [HandleError]
        public ActionResult Delete(int id)
        {
            //TODO: add try catch
            UserAccount userAccount = ModelCollections.userAccounts.Find(x => x.ID == id);
            if (userAccount == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)

        {
            try
            {

                ModelCollections.userAccounts.RemoveAt(id);

                //log user activity
                //any logger can be used as well, this a custom example
                //adds data to a collection, this can be written to db directly or to a log file
                ActivityString activity = LogActivity.DeleteUser();
                int.TryParse(Session["UserID"].ToString(), out int ID);
                int recordID = ModelCollections.userActivityLogs.Count + 1;
                ModelCollections.userActivityLogs.Add(new UserActivityLog()
                {
                    ID = recordID,
                    UserAccountID = ID,
                    Username = Session["Username"].ToString(),
                    Activity = activity.Activity,
                    ActivityDate = DateTime.Now,
                    ActivityDescription = activity.ActivityDescription,
                    Error = "", //if there was an error, you can report it here as well
                    Source = System.Environment.MachineName
                });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
         
         
    }
}
