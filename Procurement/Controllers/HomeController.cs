using System.Web.Mvc;
using Procurement.DAL;


namespace Procurement.Controllers
{
    public class HomeController : Controller
    {
        private ProContext db = new ProContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Orders");
            //return View();
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}