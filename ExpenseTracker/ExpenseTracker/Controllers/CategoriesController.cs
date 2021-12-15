using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CategoriesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Categories.Include(c => c.User).Where(p=>p.UserId == user.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CategoryID == id && m.User.Id == user.Id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,Title,UserId")] Category category)
        {
            var user = await _userManager.GetUserAsync(User);
            category.UserId = user.Id;

            if (ModelState.IsValid)
            {
                category.Title = category.Title.Trim();

                //check same name category is available or not
                var existCategory = await _context.Categories.Include(c => c.User).FirstOrDefaultAsync(m=>m.UserId == user.Id && m.Title.ToLower() == category.Title.ToLower());
                if (existCategory != null)
                {
                    ModelState.AddModelError("Title", "Category already exists");
                }
            }
            
            if (ModelState.IsValid)
            {
                category.CategoryID = Guid.NewGuid();
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories.Include(c => c.User).FirstOrDefaultAsync(m=>m.CategoryID == id && m.User.Id == user.Id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CategoryID,Title,UserId")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (category.UserId != user.Id)
            {
                ModelState.AddModelError("UserId", "Invalid User");
            }

            if (ModelState.IsValid)
            {
                category.Title = category.Title.Trim();

                //check same name category is available or not
                var existCategory = await _context.Categories.Include(c => c.User).FirstOrDefaultAsync(m=>m.CategoryID != category.CategoryID && m.UserId == user.Id && m.Title.ToLower() == category.Title.ToLower());
                if (existCategory != null)
                {
                    ModelState.AddModelError("Title", "Category already exists");
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CategoryID == id && m.User.Id == user.Id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryID == id && m.User.Id == user.Id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }
}
