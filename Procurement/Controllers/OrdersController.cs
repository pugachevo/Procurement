using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Procurement.DAL;
using Procurement.Models;

namespace Procurement.Controllers
{
    public class OrdersController : Controller
    {
        private ProContext db = new ProContext();

        // GET: Orders
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UsersSortParm = sortOrder == "users" ? "users_desc" : "users";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            
            var users = db.Users.OrderBy(q => q.Name).ToList();

            var orders = from s in db.Orders
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                int searchId;
                Int32.TryParse(searchString, out searchId);
                
                orders = orders.Where(s => s.ID == searchId || s.Users.Name.Contains(searchString))
                    .Include(d => d.Users);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(s => s.ID);
                    break;
                case "Date":
                    orders = orders.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.Date);
                    break;
                case "users":
                    orders = orders.OrderBy(s => s.Users.Name);
                    break;
                case "users_desc":
                    orders = orders.OrderByDescending(s => s.Users.Name);
                    break;
                default:  // Name ascending 
                    orders = orders.OrderBy(s => s.ID);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        
        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sum, Date, UserID")]Orders orders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Orders.Add(orders);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(orders);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            string[] fieldsToBind = new string[] {"Sum","Date", "UserID"};
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ordersToUpdate = db.Orders.Find(id);
            if (TryUpdateModel(ordersToUpdate, fieldsToBind))
            {
                try
                {
                    db.SaveChanges();;

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(ordersToUpdate);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Orders orders = db.Orders.Find(id);
                db.Orders.Remove(orders);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
