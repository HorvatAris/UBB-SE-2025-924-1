using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamWeb.Data;

namespace SteamWeb.Controllers
{
    public class PointShopItemsController : Controller
    {
        private readonly DataContext _context;

        public PointShopItemsController(DataContext context)
        {
            _context = context;
        }

        // GET: PointShopItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.PointShopItems.ToListAsync());
        }

        // GET: PointShopItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointShopItem = await _context.PointShopItems
                .FirstOrDefaultAsync(m => m.PointShopItemId == id);
            if (pointShopItem == null)
            {
                return NotFound();
            }

            return View(pointShopItem);
        }

        // GET: PointShopItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PointShopItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PointShopItemId,Name,Description,ImagePath,PointPrice,ItemType")] PointShopItem pointShopItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pointShopItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pointShopItem);
        }

        // GET: PointShopItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointShopItem = await _context.PointShopItems.FindAsync(id);
            if (pointShopItem == null)
            {
                return NotFound();
            }
            return View(pointShopItem);
        }

        // POST: PointShopItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PointShopItemId,Name,Description,ImagePath,PointPrice,ItemType")] PointShopItem pointShopItem)
        {
            if (id != pointShopItem.PointShopItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pointShopItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointShopItemExists(pointShopItem.PointShopItemId))
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
            return View(pointShopItem);
        }

        // GET: PointShopItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointShopItem = await _context.PointShopItems
                .FirstOrDefaultAsync(m => m.PointShopItemId == id);
            if (pointShopItem == null)
            {
                return NotFound();
            }

            return View(pointShopItem);
        }

        // POST: PointShopItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pointShopItem = await _context.PointShopItems.FindAsync(id);
            if (pointShopItem != null)
            {
                _context.PointShopItems.Remove(pointShopItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointShopItemExists(int id)
        {
            return _context.PointShopItems.Any(e => e.PointShopItemId == id);
        }
    }
}
