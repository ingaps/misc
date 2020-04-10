using DecimalNumbersExampleMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DecimalNumbersExampleMvc.Controllers
{
    public class ItemsController : Controller
    {
        // GET: Items
        public ActionResult Index()
        {


            return View(ModelCollections.ItemsForSaleList);
        }

        // GET: Items/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Items/Create
        [AllowAnonymous]
        public ActionResult CreateItem()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        public ActionResult CreateItem([Bind(Include = "ID,ItemName,Price")] ItemForSale itemForSale)
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

                    return RedirectToAction("Index","Items");
                }

                return View(itemForSale);
               
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(String.Empty, "An Error occurred: "+ex.Message); //ex.Message is usually written to log, not displayed to users
                return View();
            }
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Items/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Items/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
