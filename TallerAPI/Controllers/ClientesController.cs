using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TallerAPI.Models;

namespace TallerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
	private readonly TallerContext _context;

	public ClientesController(TallerContext context)
	{
		_context = context;
	}

	// 1. GET: api/clientes (200 OK con filtro por Query String)
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes([FromQuery] string? nombre)
	{
		var query = _context.Clientes.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(nombre))
		{
			query = query.Where(c => c.Nombre.Contains(nombre));
		}

		return Ok(await query.ToListAsync());
	}

	// 2. GET: api/clientes/5 (200 OK / 404 NotFound)
	[HttpGet("{id}")]
	public async Task<ActionResult<Cliente>> GetCliente(int id)
	{
		var cliente = await _context.Clientes.AsNoTracking()
			.FirstOrDefaultAsync(c => c.IdCliente == id);

		if (cliente == null) return NotFound();

		return Ok(cliente);
	}

	// 3. POST: api/clientes (201 CreatedAtAction / 400 BadRequest)
	[HttpPost]
	public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
	{
		_context.Clientes.Add(cliente);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
	}

	// 4. PUT: api/clientes/5 (204 NoContent / 400 BadRequest / 404 NotFound)
	[HttpPut("{id}")]
	public async Task<IActionResult> PutCliente(int id, Cliente cliente)
	{
		if (id != cliente.IdCliente) return BadRequest("El ID de la ruta no coincide con el cuerpo.");

		_context.Entry(cliente).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!await _context.Clientes.AnyAsync(e => e.IdCliente == id)) return NotFound();
			throw;
		}

		return NoContent();
	}

	// 5. DELETE: api/clientes/5 (204 NoContent / 404 NotFound / 409 Conflict - Regla de Negocio 1)
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteCliente(int id)
	{
		var cliente = await _context.Clientes.FindAsync(id);
		if (cliente == null) return NotFound();

		// Regla 409: No se puede eliminar un cliente que posee vehículos registrados
		bool tieneVehiculos = await _context.Vehiculos.AnyAsync(v => v.IdCliente == id);
		if (tieneVehiculos)
		{
			return Conflict(new { mensaje = "No se puede eliminar el cliente porque posee vehículos registrados en el sistema." });
		}

		_context.Clientes.Remove(cliente);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}