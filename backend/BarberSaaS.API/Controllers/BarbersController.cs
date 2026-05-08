using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.Barber;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly AppDbContext _context;

    public BarbersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BarberResponseDto>>> GetAll()
    {
        var userId = GetUserId();

        var barbers = await _context.Barbers
            .AsNoTracking()
            .Where(x => x.BarberShop!.OwnerUserId == userId)
            .OrderBy(x => x.Id)
            .Select(x => new BarberResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                BarberShopId = x.BarberShopId
            })
            .ToListAsync();

        return Ok(barbers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BarberResponseDto>> GetById(int id)
    {
        var userId = GetUserId();

        var barber = await _context.Barbers
            .AsNoTracking()
            .Where(x => x.Id == id && x.BarberShop!.OwnerUserId == userId)
            .Select(x => new BarberResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                BarberShopId = x.BarberShopId
            })
            .FirstOrDefaultAsync();

        if (barber is null)
            return NotFound(new { message = "Barbeiro não encontrado." });

        return Ok(barber);
    }

    [HttpPost]
    public async Task<ActionResult<BarberResponseDto>> Create([FromBody] CreateBarberDto dto)
    {
        var userId = GetUserId();

        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId && x.OwnerUserId == userId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe ou não pertence ao usuário logado." });

        var barber = new Barber
        {
            Name = dto.Name.Trim(),
            BarberShopId = dto.BarberShopId
        };

        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();

        var response = new BarberResponseDto
        {
            Id = barber.Id,
            Name = barber.Name,
            BarberShopId = barber.BarberShopId
        };

        return CreatedAtAction(nameof(GetById), new { id = barber.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBarberDto dto)
    {
        var userId = GetUserId();

        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == id && x.BarberShop!.OwnerUserId == userId);

        if (barber is null)
            return NotFound(new { message = "Barbeiro não encontrado." });

        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId && x.OwnerUserId == userId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe ou não pertence ao usuário logado." });

        barber.Name = dto.Name.Trim();
        barber.BarberShopId = dto.BarberShopId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var barber = await _context.Barbers
            .FirstOrDefaultAsync(x => x.Id == id && x.BarberShop!.OwnerUserId == userId);

        if (barber is null)
            return NotFound(new { message = "Barbeiro não encontrado." });

        _context.Barbers.Remove(barber);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}