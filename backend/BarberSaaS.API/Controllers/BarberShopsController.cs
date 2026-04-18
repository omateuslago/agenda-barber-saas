using BarberSaaS.API.Data;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarberShopsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BarberShopsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BarberShop>>> GetAll()
    {
        var barberShops = await _context.BarberShops
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();

        return Ok(barberShops);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BarberShop>> GetById(int id)
    {
        var barberShop = await _context.BarberShops
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        return Ok(barberShop);
    }

    [HttpPost]
    public async Task<ActionResult<BarberShop>> Create(BarberShop barberShop)
    {
        if (string.IsNullOrWhiteSpace(barberShop.Name))
            return BadRequest(new { message = "O nome da barbearia é obrigatório." });

        if (string.IsNullOrWhiteSpace(barberShop.Phone))
            return BadRequest(new { message = "O telefone da barbearia é obrigatório." });

        barberShop.Name = barberShop.Name.Trim();
        barberShop.Phone = barberShop.Phone.Trim();
        barberShop.CreatedAt = DateTime.UtcNow;

        _context.BarberShops.Add(barberShop);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = barberShop.Id }, barberShop);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, BarberShop updatedBarberShop)
    {
        var barberShop = await _context.BarberShops.FirstOrDefaultAsync(x => x.Id == id);

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        if (string.IsNullOrWhiteSpace(updatedBarberShop.Name))
            return BadRequest(new { message = "O nome da barbearia é obrigatório." });

        if (string.IsNullOrWhiteSpace(updatedBarberShop.Phone))
            return BadRequest(new { message = "O telefone da barbearia é obrigatório." });

        barberShop.Name = updatedBarberShop.Name.Trim();
        barberShop.Phone = updatedBarberShop.Phone.Trim();

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var barberShop = await _context.BarberShops.FirstOrDefaultAsync(x => x.Id == id);

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        _context.BarberShops.Remove(barberShop);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}