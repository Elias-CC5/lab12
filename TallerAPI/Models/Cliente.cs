using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TallerAPI.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El documento es obligatorio")]
    [StringLength(20)]
    public string Documento { get; set; } = null!;

    [StringLength(15)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [JsonIgnore] // Evita ciclos recursivos en JSON (Exigido en el lab)
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}