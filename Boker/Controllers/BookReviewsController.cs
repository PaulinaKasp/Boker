using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boker.Data;
using Boker.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Boker.Controllers
{
    public class BookReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookReviews
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BookReview.Include(b => b.Book).Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BookReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BookReview == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReview
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookReview == null)
            {
                return NotFound();
            }

            return View(bookReview);
        }

        // GET: BookReviews/Create
        public IActionResult Create(int bookId)
        {
            var book = _context.Book.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            ViewData["BookTitle"] = book.Title;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", userId);

            var review = new BookReview { BookId = bookId };
            return View(review);
        }

        // POST: BookReviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,UserId,ReviewText,ReviewDate")] BookReview bookReview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookReview);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Books", new { id = bookReview.BookId });
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", bookReview.UserId);
            return View(bookReview);
        }

        // GET: BookReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BookReview == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReview.FindAsync(id);
            if (bookReview == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", bookReview.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bookReview.UserId);
            return View(bookReview);
        }

        // POST: BookReviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,ReviewText,ReviewDate")] BookReview bookReview)
        {
            if (id != bookReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookReviewExists(bookReview.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", bookReview.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bookReview.UserId);
            return View(bookReview);
        }

        // GET: BookReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BookReview == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReview
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookReview == null)
            {
                return NotFound();
            }

            return View(bookReview);
        }

        // POST: BookReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BookReview == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BookReview'  is null.");
            }
            var bookReview = await _context.BookReview.FindAsync(id);
            if (bookReview != null)
            {
                _context.BookReview.Remove(bookReview);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookReviewExists(int id)
        {
          return (_context.BookReview?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
