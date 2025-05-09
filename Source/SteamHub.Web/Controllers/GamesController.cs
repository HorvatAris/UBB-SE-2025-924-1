using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;

namespace SteamHub.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly DataContext _context;

        public GamesController(DataContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Games.Include(g => g.Publisher).Include(g => g.Status);
            return View(await dataContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Publisher)
                .Include(g => g.Status)
                .FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["PublisherUserId"] = new SelectList(_context.Users, "UserId", "Email");
            ViewData["StatusId"] = new SelectList(_context.Set<GameStatus>(), "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,Name,Description,ImagePath,Price,MinimumRequirements,RecommendedRequirements,StatusId,RejectMessage,Rating,NumberOfRecentPurchases,TrailerPath,GameplayPath,Discount,PublisherUserId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherUserId"] = new SelectList(_context.Users, "UserId", "Email", game.PublisherIdentifier);
            ViewData["StatusId"] = new SelectList(_context.Set<GameStatus>(), "Id", "Name", game.Status);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["PublisherUserId"] = new SelectList(_context.Users, "UserId", "Email", game.PublisherUserId);
            ViewData["StatusId"] = new SelectList(_context.Set<GameStatus>(), "Id", "Name", game.StatusId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameId,Name,Description,ImagePath,Price,MinimumRequirements,RecommendedRequirements,StatusId,RejectMessage,Rating,NumberOfRecentPurchases,TrailerPath,GameplayPath,Discount,PublisherUserId")] Game game)
        {
            if (id != game.GameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.GameId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherUserId"] = new SelectList(_context.Users, "UserId", "Email", game.PublisherIdentifier);
            ViewData["StatusId"] = new SelectList(_context.Set<GameStatus>(), "Id", "Name", game.Status);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Publisher)
                .Include(g => g.Status)
                .FirstOrDefaultAsync(m => m.GameId == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.GameId == id);
        }
    }
}
