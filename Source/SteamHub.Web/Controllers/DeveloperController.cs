using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Collections.ObjectModel;

namespace SteamHub.Web.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {
        private readonly IDeveloperService developerService;

        public DeveloperController(IDeveloperService developerService)
        {
            this.developerService = developerService;
        }
        // GET: Developer/MyGames
        public async Task<IActionResult> MyGames()
        {
            var games = await developerService.GetDeveloperGamesAsync();
            return View(games); // View expects a list of Game
        }
        // GET: /Developer/UnvalidatedGames
        public async Task<IActionResult> UnvalidatedGames()
        {
            var games = await developerService.GetUnvalidatedAsync();
            return View(games); // View expects a list of Game
        }
        public async Task<IActionResult> Create()
        {
            var tags = (await developerService.GetAllTagsAsync()).ToList(); // Explicit conversion to List<Tag>
            var viewModel = new CreateGameViewModel { AllTags = tags };
            return View(viewModel);
        }

        // POST: /Developer/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateGameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = (await developerService.GetAllTagsAsync()).ToList();
                return View(model);
            }

            var game = await developerService.CreateValidatedGameAsync(
                model.GameId,
                model.Name,
                model.Price,
                model.Description,
                model.ImageUrl,
                model.TrailerUrl,
                model.GameplayUrl,
                model.MinimumRequirement,
                model.RecommendedRequirement,
                model.Discount,
                model.SelectedTags
            );

            return RedirectToAction("MyGames");
        }


        //Fix for CS1503: Convert the List<Game> to ObservableCollection<Game> before passing it to the method.
        public async Task<IActionResult> Edit(int id)
        {
            var allGames = new ObservableCollection<Game>((await developerService.GetDeveloperGamesAsync()).ToList());
            var game = developerService.FindGameInObservableCollectionById(id, allGames);
            if (game == null) return NotFound();


            var tags = (await developerService.GetAllTagsAsync()).ToList();
            var selectedTags = (await developerService.GetGameTagsAsync(id))
                   .Select(t => t.TagId)
                   .ToList();

            var model = new EditGameViewModel
            {
                GameId = game.GameId.ToString(),
                Name = game.GameTitle,
                Price = game.Price.ToString(),
                Description = game.GameDescription,
                ImageUrl = game.ImagePath,
                TrailerUrl = game.TrailerPath,
                GameplayUrl = game.GameplayPath,
                MinimumRequirement = game.MinimumRequirements,
                RecommendedRequirement = game.RecommendedRequirements,
                Discount = game.Discount.ToString(),
                AllTags = tags,
                SelectedTags = selectedTags
            };

            return View(model);
        }

        //// POST: /Developer/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditGameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTags = (await developerService.GetAllTagsAsync()).ToList(); // Explicit conversion to List<Tag>

                return View(model);
            }
            var allTags = await developerService.GetAllTagsAsync();
            // Fix for CS1503: Convert the string GameId to an integer before passing it to the method.
            var selectedTags = model.SelectedTags;
            var selectedTagObjects = allTags
        .Where(tag => selectedTags.Contains(tag.TagId))  // Find tags that match the selected tag IDs
        .ToList();

            var game = developerService.ValidateInputForAddingAGame(
                model.GameId,
                model.Name,
                model.Price,
                model.Description,
                model.ImageUrl,
                model.TrailerUrl,
                model.GameplayUrl,
                model.MinimumRequirement,
                model.RecommendedRequirement,
                model.Discount,
                selectedTagObjects
            );

            await developerService.UpdateGameWithTagsAsync(game, selectedTagObjects);
            return RedirectToAction("MyGames");
        }

        // POST: /Developer/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var developerGames = new ObservableCollection<Game>((await developerService.GetDeveloperGamesAsync()).ToList());
            await developerService.DeleteGameAsync(id, developerGames);
            return RedirectToAction("MyGames");
        }

        // POST: /Developer/Validate/5
        [HttpPost]
        public async Task<IActionResult> Validate(int id)
        {
            await developerService.ValidateGameAsync(id);
            return RedirectToAction("UnvalidatedGames");
        }

        // GET: /Developer/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
            var model = new RejectGameViewModel { GameId = id };
            return View(model);
        }

        // Fix for CS1503: Convert the List<Game> to ObservableCollection<Game> before passing it to the method.
        [HttpPost]
        public async Task<IActionResult> Reject(RejectGameViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.RejectionMessage))
            {
                await developerService.RejectGameWithMessageAsync(model.GameId, model.RejectionMessage);
            }
            else
            {
                var unvalidatedGames = new ObservableCollection<Game>((await developerService.GetUnvalidatedAsync()).ToList());
                await developerService.RejectGameAndRemoveFromUnvalidatedAsync(model.GameId, unvalidatedGames);
            }

            return RedirectToAction("UnvalidatedGames");
        }

        // GET: /Developer/GameTags/5
        public async Task<IActionResult> GameTags(int id)
        {
            var tags = await developerService.GetGameTagsAsync(id);
            return Json(tags);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
