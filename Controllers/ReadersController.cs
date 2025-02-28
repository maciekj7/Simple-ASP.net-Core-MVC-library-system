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
    public class ReadersController : Controller
    {
        private readonly BibliotekaContext _context;

        public ReadersController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }
            return View(await _context.Reader.ToListAsync());
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Reader
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // GET: Readers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            return View();
        }

        // POST: Readers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CardNumber,Password")] Reader reader)
        {
            if (_context.Reader.Any(r => r.CardNumber == reader.CardNumber))
            {
                ViewData["ErrorMessage"] = "Numer karty już istnieje w bazie";
                return View(reader);
            }

            if (ModelState.IsValid)
            {
                _context.Add(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(reader);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Reader.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }
            return View(reader);
        }

        // POST: Readers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CardNumber,Password")] Reader reader)
        {
            if (id != reader.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderExists(reader.Id))
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
            return View(reader);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserType") != "Employee")
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Reader
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Reader.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservation
                .Where(r => r.ReaderId == id)
                .ToListAsync();

            var loans = await _context.Loan
                .Where(l => l.ReaderId == id)
                .ToListAsync();

            if (reservations.Any() || loans.Any())
            {
                ViewData["ErrorMessage"] = "Nie można zamknąć konta czytelnika, który posiada aktywne rezerwacje lub wypożyczenia. Aby kontynuować przyjmij zwroty książek i anuluj rezerwacje czytelnika.";
                return View(reader);
            }

            _context.Reader.Remove(reader);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ReaderExists(int id)
        {
            return _context.Reader.Any(e => e.Id == id);
        }
    }
}
