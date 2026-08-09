using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_P.Models;

public class Club
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Ad { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Aciklama { get; set; }

    [DataType(DataType.Date)]
    public DateTime KurulusTarihi { get; set; } = DateTime.UtcNow;

    public bool AktifMi { get; set; } = true;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
