using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GranitHouse.Data;
using GranitHouse.Models;
using GranitHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GranitHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.SpecialTagTypes.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTagTypes specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTagTypes.Add(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTagTypes.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTagTypes specialTag)
        {
            if (id != specialTag.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _db.SpecialTagTypes.Update(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTagTypes.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTagTypes.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _db.SpecialTagTypes.FindAsync(id);
            _db.SpecialTagTypes.Remove(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}