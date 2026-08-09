using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_P.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    public int ClubId { get; set; }
    public Club? Club { get; set; }

    [Required, MaxLength(300)]
    public string Baslik { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Aciklama { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime BaslangicTarihi { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime BitisTarihi { get; set; }

    [Range(1, int.MaxValue)]
    public int Kontenjan { get; set; }

    [MaxLength(300)]
    public string? Konum { get; set; }

    [Required, MaxLength(50)]
    public string Durum { get; set; } = "Planlandi"; // Planlandý / Tamamlandý / Ýptal

    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
