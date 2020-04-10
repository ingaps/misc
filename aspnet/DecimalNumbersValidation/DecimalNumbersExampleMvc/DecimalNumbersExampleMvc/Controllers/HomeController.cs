using DecimalNumbersExampleMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DecimalNumbersExampleMvc.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "ID,ItemName,Price")] ItemForSale itemForSale)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //User would be the user who is logged in
                    itemForSale.User = "user01";
                    itemForSale.Date = DateTime.Now;

                    // TODO: Add insert logic here (usually to db)
                    ModelCollections.ItemsForSaleList.Add(itemForSale);

                    return RedirectToAction("Index", "Items");
                }

                return View(itemForSale);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "An Error occurred: " + ex.Message); //ex.Message is usually written to log, not displayed to users
                return View();
            }
        }


    }
}