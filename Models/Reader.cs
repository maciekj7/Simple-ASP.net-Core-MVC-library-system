namespace Biblioteka.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
public class Reader
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Imię i nazwisko")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Numer karty")]
    public string CardNumber { get; set; }

    [Required]
    [Display(Name = "Hasło")]
    public string Password { get; set; }
}