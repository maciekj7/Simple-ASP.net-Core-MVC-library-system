using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Models;
using Microsoft.AspNetCore.Identity;

namespace Biblioteka.Data
{
    public class BibliotekaContext : DbContext
    {
        public BibliotekaContext (DbContextOptions<BibliotekaContext> options)
            : base(options)
        {
        }

        public DbSet<Biblioteka.Models.Book> Book { get; set; } = default!;
        public DbSet<Biblioteka.Models.Reader> Reader { get; set; } = default!;
        public DbSet<Biblioteka.Models.Reservation> Reservation { get; set; } = default!;
        public DbSet<Biblioteka.Models.Loan> Loan { get; set; } = default!;
        public DbSet<Biblioteka.Models.Employee> Employee { get; set; } = default!;

    }
}
