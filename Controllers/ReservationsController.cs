using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Data;
using Biblioteka.Models;
using System.Net;

namespace Biblioteka.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly BibliotekaContext _context;

        public ReservationsController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            int? readerId = HttpContext.Session.GetInt32("UserId");
            if (readerId == null)
            {
                return Forbid();
            }

            var reservations = await _context.Reservation
                .Where(r => r.ReaderId == readerId.Value)
                .Include(r => r.Book)
                .ToListAsync();

            return View(reservations);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Book)
                .Include(r => r.Reader)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            var availableBooks = await _context.Book.Where(b => b.IsAvailable).ToListAsync();
            ViewBag.AvailableBooks = availableBooks;
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId)
        {
            int? readerId = HttpContext.Session.GetInt32("UserId");
            if (readerId == null)
            {
                return Forbid();
            }

            var book = await _context.Book.FindAsync(bookId);
            if (book == null || !book.IsAvailable)
            {
                return NotFound();
            }

            book.IsAvailable = false;
            _context.Book.Update(book);

            var reservation = new Reservation
            {
                BookId = bookId,
                ReaderId = readerId.Value,
                ReservedAt = DateTime.Now
            };

            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Author", reservation.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Reader, "Id", "CardNumber", reservation.ReaderId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,ReaderId,ReservedAt")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Author", reservation.BookId);
            ViewData["ReaderId"] = new SelectList(_context.Reader, "Id", "CardNumber", reservation.ReaderId);
            return View(reservation);
        }

        // POST: Reservations/Loan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loan(int id)
        {
            var reservation = await _context.Reservation.Include(r => r.Book).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            var loan = new Loan
            {
                BookId = reservation.BookId,
                ReaderId = reservation.ReaderId,
                LoanedAt = DateTime.Now
            };

            var book = await _context.Book.FindAsync(reservation.BookId);
            book.IsAvailable = false;

            _context.Loan.Add(loan);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("EmployeeIndex");
        }

        public async Task<IActionResult> EmployeeIndex()
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            var reservations = await _context.Reservation.Include(r => r.Book).Include(r => r.Reader).ToListAsync();
            return View(reservations);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _context.Reservation.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            var book = await _context.Book.FindAsync(reservation.BookId);
            book.IsAvailable = true;
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
