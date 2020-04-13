using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleLoginMvc.Models
{

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


  

    public class UserAccount
    {
        public int ID { get; set; } //here it is the index of the collection in ModelCollections class

        [Required]
      //  [Remote("IsUsernameUnique", "Validation", ErrorMessage = "Username already exists")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

       

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("UserPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Last login")]
        public DateTime? LastLoginDate { get; set; }
    }


    public class UserActivityLog
    {
        public int ID { get; set; }

        public int UserAccountID { get; set; }

       
        public string Username { get; set; }

        public string Activity { get; set; }

        [Display(Name = "Activity date")]
        public DateTime ActivityDate { get; set; }

        [Display(Name = "Activity description")]
        public string ActivityDescription { get; set; }

        public string Error { get; set; }

        public string Source { get; set; }

    }

    public enum Roles
    {
        [Description("Admin")]
        Admin = 1,
        [Description("User")]
        User = 2
    }

   

    public static class ModelCollections
    {
        public static List<UserAccount> userAccounts = new List<UserAccount>();
        public static List<UserActivityLog> userActivityLogs = new List<UserActivityLog>();


        public static void InitCollections()
        {
            userAccounts.Add(new UserAccount()
            {
                ID = 0,
                Username = "admin",
                UserPassword = "admin1",
                Name = "John",
                LastName = "Doe",
                Role = Roles.Admin.ToString()
            });


            userAccounts.Add(new UserAccount()
            {
                ID = 1,
                Username = "user",
                UserPassword = "user1",
                Name = "Alice",
                LastName = "Doe",
                Role = Roles.User.ToString()
            });
        }
    }


    public struct ActivityString
    {
        public string Activity { get; set; }

        public string ActivityDescription { get; set; }

        public string ErrorMsg { get; set; }
    }

    public class LogActivity
    {
        enum Activity
        {
            [Description("Create")]
            Create,
            [Description("Delete")]
            Delete,
            [Description("Login")]
            Login,
            [Description("Logout")]
            Logout,         
            [Description("Error")]
            Error
        }

      

        public static ActivityString LoginUser()
        {
            return new ActivityString() { Activity = Activity.Login.ToString(), ActivityDescription = "Successful user login" };
        }

        public static ActivityString LogoutUser()
        {
            return new ActivityString() { Activity = Activity.Logout.ToString(), ActivityDescription = "Successful logout" };
        }


        public static ActivityString CreateUser()
        {
            return new ActivityString() { Activity = Activity.Create.ToString(), ActivityDescription = "New user created" };
        }

        public static ActivityString DeleteUser()
        {
            return new ActivityString() { Activity = Activity.Delete.ToString(), ActivityDescription = "User deleted" };
        }

    }

   

}