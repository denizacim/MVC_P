using System.ComponentModel.DataAnnotations;

namespace MVC_P.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string AdSoyad { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SifreHash { get; set; } = string.Empty; // Demo amaçlý düz metin de olabilir

    [Required]
    public string Rol { get; set; } = "Ogrenci"; // Admin, KulupYoneticisi, Ogrenci
}
