# Simple Login using Session variable in ASP.NET MVC 5

This Visual studio project is an example of a simple login using Session variables and custom authorization.
After user autentication user information is stored in the Session variable:

```
Session["Username"]
Session["UserID"]
Session["UserRole"]
Session["FullName"]
```

There are two levels of user authorization:
- User
- Admin

User account that has a role `User` assigned can access only the Welcome home page.
User account with role `Admin` can access information about users and thei activities, create and delete new users.

Roles are stored in enum:

```C#
public enum Roles
    {
        [Description("Admin")]
        Admin = 1,
        [Description("User")]
        User = 2
    }
```

## Storing login data in special model

When the user finds himself/herself on the Login page, they are asked to fill in their username and password.
This data is stored in class `LoginViewModel`:

```
 public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }
    }

```


## Checking user credentials

After clicking Login button, the method `Login(LoginViewModel user, string returnUrl)` in `LoginController` class checks
user data in the local storage (in this example it is a List of UserAccount items, usually it is a database).
Here is the code:

```
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
									//check user credentials
                                    //decrypt found password if necessary and compare it to the one provided in login
                                    if (user.Password == userAccount.UserPassword)
                                    {
                                        Session["UserRole"] = userAccount.Role;
                                        Session["Username"] = userAccount.Username;
                                        Session["UserID"] = userAccount.ID;
                                        Session["FullName"] = userAccount.Name + " " + userAccount.LastName;
                                       
									   //add record to user activity log
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
```

After checking user crdentials, Session variable is initialized and a record about user login is added to user 
activity log (in this case a List<T>, in real world it would be a database or a log file).

If the user has been authenticated, he/she is redirected to the Welcome home page. From there the user has access
to application features based on the Role variable (authorization).

## Achieving authorization using custom roles

Authorization is achieved by costumizing the attribute Authorize. A custom attribute authorize has been implemented in
class `CustomAuthorizeAttribute`:

```C#
 public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (HttpContext.Current.Session["UserName"] != null)
            {

                if (this.allowedroles.Length == 1)
                {
                    if (HttpContext.Current.Session["UserRole"].ToString() == allowedroles[0])
                    {
                        authorize = true;
                    }
                }
                else if (this.allowedroles.Length == 2)
                {
                    foreach (string role in allowedroles)
                    {
                        if (HttpContext.Current.Session["UserRole"].ToString() == role)
                        {
                            authorize = true;
                            break;
                        }
                    }
                }
                /*  else if (this.allowedroles.Length > 2)
                  {
                      foreach (string role in allowedroles)
                      {
                          //if there were more roles
                          //compare to roles stored in Session[]
                          //if ok authorize = true
                          //not in use now so not implemeted, if necesarry, can be implemeted     
                          break;
                      }
                  }   */
            }

            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
```

Usage:

```C#

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
```

## Summary
This is a simple example of usage of the Session variable to achieve autentication and authorization.
If a more complex and safer method of authorization is necessary, have a look at [Forms Autentication](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/security/authenticating-users-with-forms-authentication-cs)
and [OWIN autentication](https://devblogs.microsoft.com/aspnet/understanding-owin-forms-authentication-in-mvc-5/)





