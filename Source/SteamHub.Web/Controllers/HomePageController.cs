using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SteamHub.Services.Interfaces;
namespace SteamHub.Web.Controllers
{
    public class HomePageController : Controller
    {
        private readonly IGameService gameService;


        public HomePageController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        // GET: HomePageController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HomePageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomePageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomePageController/Create
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

        // GET: HomePageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomePageController/Edit/5
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

        // GET: HomePageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomePageController/Delete/5
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
