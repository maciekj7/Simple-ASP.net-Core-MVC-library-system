namespace Biblioteka.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Loan
    {
        public int Id { get; set; }
        [Display(Name = "Tytuł")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        [Display(Name = "Numer czytelnika")]
        public int ReaderId { get; set; }
        public Reader Reader { get; set; }
        [Display(Name = "Data")]
        public DateTime LoanedAt { get; set; }
        public DateTime ReturnDueDate => LoanedAt.AddDays(30);
    }
}
