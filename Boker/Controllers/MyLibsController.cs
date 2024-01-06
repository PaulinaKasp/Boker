using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boker.Data;
using Boker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Boker.Controllers
{
    [Authorize]
    public class MyLibsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyLibsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
        public async Task<IActionResult> AddToMyLib(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            var myLib = new MyLib
            {
                UserId = user.Id
            };

            _context.MyLib.Add(myLib);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: MyLibs
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var userMyLibs = await _context.MyLib
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            return View(userMyLibs);
        }

        // GET: MyLibs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MyLib == null)
            {
                return NotFound();
            }

            var myLib = await _context.MyLib
                .Include(m => m.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myLib == null)
            {
                return NotFound();
            }

            return View(myLib);
        }

        // GET: MyLibs/Create
        public async Task<IActionResult> Create(int bookId)
        {
            var book = _context.Book.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            ViewData["BookList"] = new SelectList(_context.Book, "Title", "Title", book.Title);

            var currentUser = await GetCurrentUserAsync();

            if (currentUser != null)
            {
                var users = _context.Users.Select(u => new { Id = u.Id, Email = u.Email }).ToList();
                ViewData["UserId"] = new SelectList(users, "Id", "Email", currentUser.Id);

                var myLib = new MyLib { Title = book.Title, UserId = currentUser.Id };
                return View(myLib);
            }
            else
            {
                var myLib = new MyLib { Title = book.Title };
                return View(myLib);
            }
        }


        // POST: MyLibs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IsFavorite,UserId,StartedReading,FinishedReading,Notes")] MyLib myLib)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser != null)
                {
                    myLib.UserId = currentUser.Id;
                }

                var selectedBook = await _context.Book.FirstOrDefaultAsync(b => b.Title == myLib.Title);

                if (selectedBook != null)
                {
                    myLib.Title = selectedBook.Title;
                    _context.Add(myLib);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["BookList"] = new SelectList(_context.Book, "Title", "Title", myLib.Title);
            return View(myLib);
        }

        // GET: MyLibs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MyLib == null)
            {
                return NotFound();
            }

            var myLib = await _context.MyLib.FindAsync(id);
            if (myLib == null)
            {
                return NotFound();
            }
            ViewData["BookList"] = new SelectList(_context.Book, "Id", "Title", myLib.Title);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", myLib.UserId);
            return View(myLib);
        }

        // POST: MyLibs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsFavorite,UserId,StartedReading,FinishedReading,Notes")] MyLib myLib)
        {
            if (id != myLib.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myLib);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyLibExists(myLib.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", myLib.UserId);
            return View(myLib);
        }

        // GET: MyLibs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MyLib == null)
            {
                return NotFound();
            }

            var myLib = await _context.MyLib
                .Include(m => m.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myLib == null)
            {
                return NotFound();
            }

            return View(myLib);
        }

        // POST: MyLibs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MyLib == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MyLib'  is null.");
            }
            var myLib = await _context.MyLib.FindAsync(id);
            if (myLib != null)
            {
                _context.MyLib.Remove(myLib);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyLibExists(int id)
        {
            return (_context.MyLib?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
