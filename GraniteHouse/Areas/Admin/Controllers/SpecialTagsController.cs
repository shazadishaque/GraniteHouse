using GraniteHouse.Data;
using GraniteHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GraniteHouse.Utility;

namespace GraniteHouse.Areas.Admin.Controllers
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
            List<SpecialTags> specialTags = _db.SpecialTags.ToList();

            return View(specialTags);
        }

        //GET Create Action Method
        public IActionResult Create()
        {
            return View();
        }

        //POST Create Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTag)
        {
            if(ModelState.IsValid)
            {
                _db.Add(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        //GET Edit Action Method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecialTags specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //POST Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTags.Update(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

        //GET Details Action Method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecialTags specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //GET Delete Action Method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecialTags specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //POST Edit Action Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _db.SpecialTags.FindAsync(id);
            _db.SpecialTags.Remove(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}