namespace Biblioteka.Models;
using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }

    [Display(Name = "Tytuł")]
    [Required]
    public string Title { get; set; }

    [Display(Name = "Autor")]
    [Required]
    public string Author { get; set; }
    [Display(Name = "Wydawca")]
    [Required]
    public string Publisher { get; set; }
    [Display(Name = "Dostępność")]
    public bool IsAvailable { get; set; }
}