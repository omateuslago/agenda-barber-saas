using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.BarberShop;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BarberShopsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BarberShopsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BarberShopResponseDto>>> GetAll()
    {
        var userId = GetUserId();

        var barberShops = await _context.BarberShops
            .AsNoTracking()
            .Where(x => x.OwnerUserId == userId)
            .OrderBy(x => x.Id)
            .Select(x => new BarberShopResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(barberShops);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BarberShopResponseDto>> GetById(int id)
    {
        var userId = GetUserId();

        var barberShop = await _context.BarberShops
            .AsNoTracking()
            .Where(x => x.Id == id && x.OwnerUserId == userId)
            .Select(x => new BarberShopResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        return Ok(barberShop);
    }

    [HttpPost]
    public async Task<ActionResult<BarberShopResponseDto>> Create([FromBody] CreateBarberShopDto dto)
    {
        var userId = GetUserId();

        var barberShop = new BarberShop
        {
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BarberShops.Add(barberShop);
        await _context.SaveChangesAsync();

        var response = new BarberShopResponseDto
        {
            Id = barberShop.Id,
            Name = barberShop.Name,
            Phone = barberShop.Phone,
            CreatedAt = barberShop.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = barberShop.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBarberShopDto dto)
    {
        var userId = GetUserId();

        var barberShop = await _context.BarberShops
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == userId);

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        barberShop.Name = dto.Name.Trim();
        barberShop.Phone = dto.Phone.Trim();

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var barberShop = await _context.BarberShops
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == userId);

        if (barberShop is null)
            return NotFound(new { message = "Barbearia não encontrada." });

        _context.BarberShops.Remove(barberShop);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}