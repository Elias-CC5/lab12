using System.ComponentModel.DataAnnotations;

namespace TallerAPI.Models;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    [Required(ErrorMessage = "La placa es obligatoria")]
    [StringLength(10)]
    public string Placa { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Modelo { get; set; } = null!;

    [Range(1900, 2027, ErrorMessage = "Año fuera de rango válido")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "Debe asignar un IdCliente válido")]
    public int IdCliente { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; } // Propiedad nula permitida
}