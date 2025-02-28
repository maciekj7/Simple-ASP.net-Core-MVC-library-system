namespace Biblioteka.Models;
using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
