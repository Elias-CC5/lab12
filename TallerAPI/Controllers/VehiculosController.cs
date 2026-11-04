using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerAPI.Models;

namespace TallerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    private readonly TallerContext _context;

    public VehiculosController(TallerContext context)
    {
        _context = context;
    }

    // 6. GET: api/vehiculos (200 OK con filtro por query string)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos([FromQuery] string? marca)
    {
        var query = _context.Vehiculos.AsNoTracking().Include(v => v.IdClienteNavigation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(marca))
        {
            query = query.Where(v => v.Marca.Contains(marca));
        }

        return Ok(await query.ToListAsync());
    }

    // 7. GET: api/clientes/{clienteId}/vehiculos (Ruta Anidada - 200 OK / 404 NotFound)
    [HttpGet("/api/clientes/{clienteId}/vehiculos")]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculosPorCliente(int clienteId)
    {
        bool existeCliente = await _context.Clientes.AnyAsync(c => c.IdCliente == clienteId);
        if (!existeCliente) return NotFound("El cliente especificado no existe.");

        var vehiculos = await _context.Vehiculos
            .AsNoTracking()
            .Where(v => v.IdCliente == clienteId)
            .ToListAsync();

        return Ok(vehiculos);
    }

    // 8. POST: api/vehiculos (201 CreatedAtAction / 400 BadRequest / 409 Conflict - Regla de Negocio)
    [HttpPost]
    public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
    {
        // Validación 400: Verificar que la clave foránea exista
        bool existeCliente = await _context.Clientes.AnyAsync(c => c.IdCliente == vehiculo.IdCliente);
        if (!existeCliente) return BadRequest("La clave foránea (IdCliente) no existe en la base de datos.");

        // Regla 409 Conflict: Placa duplicada
        bool existePlaca = await _context.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa);
        if (existePlaca) return Conflict(new { mensaje = "Ya existe un vehículo registrado con la misma placa." });

        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehiculos), new { id = vehiculo.IdVehiculo }, vehiculo);
    }

    // 9. PUT: api/vehiculos/5 (204 NoContent / 400 BadRequest / 404 NotFound / 409 Conflict)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutVehiculo(int id, Vehiculo vehiculo)
    {
        if (id != vehiculo.IdVehiculo) return BadRequest("El ID de la ruta no coincide con el cuerpo.");

        bool placaDuplicada = await _context.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa && v.IdVehiculo != id);
        if (placaDuplicada) return Conflict(new { mensaje = "La placa pertenece a otro vehículo registrado." });

        _context.Entry(vehiculo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Vehiculos.AnyAsync(e => e.IdCliente == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // 10. DELETE: api/vehiculos/5 (204 NoContent / 404 NotFound)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehiculo(int id)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id);
        if (vehiculo == null) return NotFound();

        _context.Vehiculos.Remove(vehiculo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}