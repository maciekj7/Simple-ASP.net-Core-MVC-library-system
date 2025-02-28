namespace Biblioteka.Models;
using System;

public class Reservation
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; }

    public int ReaderId { get; set; }
    public Reader Reader { get; set; }

    public DateTime ReservedAt { get; set; }
    public DateTime ExpirationDate => ReservedAt.AddDays(3);
}