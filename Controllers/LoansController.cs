using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Data;
using Biblioteka.Models;

namespace Biblioteka.Controllers
{
    public class LoansController : Controller
    {
        private readonly BibliotekaContext _context;

        public LoansController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            int? readerId = HttpContext.Session.GetInt32("UserId");
            if (HttpContext.Session.GetString("UserType") == "Reader" && readerId == null)
            {
                return Forbid();
            }

            if (HttpContext.Session.GetString("UserType") == "Reader")
            {
                var loans = await _context.Loan
                    .Where(l => l.ReaderId == readerId.Value)
                    .Include(l => l.Book)
                    .ToListAsync();
                return View("ReaderLoans", loans);
            }
            else if (HttpContext.Session.GetString("UserType") == "Employee")
            {
                var loans = await _context.Loan.Include(l => l.Book).Include(l => l.Reader).ToListAsync();
                return View("EmployeeLoans", loans);
            }

            return Forbid();
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .Include(l => l.Book)
                .Include(l => l.Reader)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }
            var availableBooks = await _context.Book.Where(b => b.IsAvailable).ToListAsync();
            ViewBag.AvailableBooks = availableBooks;
            ViewData["ReaderId"] = new SelectList(_context.Reader, "Id", "CardNumber");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int readerId, int bookId)
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            var loan = new Loan
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanedAt = DateTime.Now
            };

            var book = await _context.Book.FindAsync(bookId);
            book.IsAvailable = false; 
            _context.Loan.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Author", loan.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Reader, "Id", "CardNumber", loan.ReaderId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,ReaderId,LoanedAt")] Loan loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Author", loan.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Reader, "Id", "CardNumber", loan.ReaderId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            var loan = await _context.Loan.FindAsync(id);
            if (loan != null)
            {
                var book = await _context.Book.FindAsync(loan.BookId);
                book.IsAvailable = true; 
                _context.Loan.Remove(loan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loan.FindAsync(id);
            var book = await _context.Book.FindAsync(loan.BookId);
            book.IsAvailable = true;
            if (loan != null)
            {
                _context.Loan.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loan.Any(e => e.Id == id);
        }
    }
}
