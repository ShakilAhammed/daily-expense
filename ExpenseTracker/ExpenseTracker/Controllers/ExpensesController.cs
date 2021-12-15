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
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ExpensesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expenses
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Expenses.Include(e => e.Category).Include(e => e.User).Where(m=>m.UserId == user.Id && m.Date_Added.Date > DateTime.Now.Date.AddDays(-2));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var expense = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ExpenseID == id && m.UserId == user.Id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public async Task<IActionResult> CreateAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            //get all category
            var allCategory = await _context.Categories.Where(m=>m.UserId == user.Id).ToListAsync();
            ViewBag.Categories = allCategory;

            ViewData["CategoryID"] = new SelectList(_context.Categories.Include(p => p.User).Where(m => m.UserId == user.Id), "CategoryID", "Title");
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseID,Amount,CategoryID,Date_Added,UserId")] Expense expense)
        {
            var user = await _userManager.GetUserAsync(User);
            expense.UserId = user.Id;
            if (ModelState.IsValid)
            {
                expense.ExpenseID = Guid.NewGuid();
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories.Include(p=>p.User).Where(m=>m.UserId == user.Id), "CategoryID", "Title", expense.CategoryID);
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            //get all category
            var allCategory = await _context.Categories.Where(m => m.UserId == user.Id).ToListAsync();
            ViewBag.Categories = allCategory;

            var expense = await _context.Expenses.FirstOrDefaultAsync(m=>m.UserId == user.Id && m.ExpenseID == id.Value);
            if (expense == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories.Include(p => p.User).Where(m => m.UserId == user.Id), "CategoryID", "Title", expense.CategoryID);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ExpenseID,Amount,CategoryID,Date_Added,UserId")] Expense expense)
        {
            if (id != expense.ExpenseID)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories.Include(p => p.User).Where(m => m.UserId == user.Id), "CategoryID", "Title", expense.CategoryID);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var expense = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ExpenseID == id && m.UserId == user.Id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var expense = await _context.Expenses.FirstOrDefaultAsync(m=>m.ExpenseID == id && m.UserId == user.Id);
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(Guid id)
        {
            return _context.Expenses.Any(e => e.ExpenseID == id);
        }
    }
}
