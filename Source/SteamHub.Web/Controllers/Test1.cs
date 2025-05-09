using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SteamHub.Web.Controllers
{
    public class Test1 : Controller
    {
        private readonly ILogger<Test1> _logger;
        // GET: Test1
        public ActionResult Index()
        {
            return View();
        }

        // GET: Test1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Test1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Test1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Test1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Test1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Test1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Test1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
