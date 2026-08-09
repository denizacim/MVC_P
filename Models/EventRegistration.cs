using System.ComponentModel.DataAnnotations;

namespace MVC_P.Models;

public class EventRegistration
{
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event? Event { get; set; }

    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(20)]
    public string OnayDurumu { get; set; } = "Beklemede"; // Beklemede / Onaylý / Reddedildi

    [MaxLength(20)]
    public string? KatilimDurumu { get; set; } // Geldi / Gelmedi
}
