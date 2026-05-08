using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.Service;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseDto>>> GetAll()
    {
        var userId = GetUserId();

        var services = await _context.Services
            .AsNoTracking()
            .Where(x => x.BarberShop!.OwnerUserId == userId)
            .OrderBy(x => x.Id)
            .Select(x => new ServiceResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                DurationMinutes = x.DurationMinutes,
                BarberShopId = x.BarberShopId
            })
            .ToListAsync();

        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponseDto>> GetById(int id)
    {
        var userId = GetUserId();

        var service = await _context.Services
            .AsNoTracking()
            .Where(x => x.Id == id && x.BarberShop!.OwnerUserId == userId)
            .Select(x => new ServiceResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                DurationMinutes = x.DurationMinutes,
                BarberShopId = x.BarberShopId
            })
            .FirstOrDefaultAsync();

        if (service is null)
            return NotFound(new { message = "Serviço não encontrado." });

        return Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponseDto>> Create([FromBody] CreateServiceDto dto)
    {
        var userId = GetUserId();

        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId && x.OwnerUserId == userId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe ou não pertence ao usuário logado." });

        var service = new Service
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            DurationMinutes = dto.DurationMinutes,
            BarberShopId = dto.BarberShopId
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        var response = new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationMinutes = service.DurationMinutes,
            BarberShopId = service.BarberShopId
        };

        return CreatedAtAction(nameof(GetById), new { id = service.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceDto dto)
    {
        var userId = GetUserId();

        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == id && x.BarberShop!.OwnerUserId == userId);

        if (service is null)
            return NotFound(new { message = "Serviço não encontrado." });

        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId && x.OwnerUserId == userId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe ou não pertence ao usuário logado." });

        service.Name = dto.Name.Trim();
        service.Price = dto.Price;
        service.DurationMinutes = dto.DurationMinutes;
        service.BarberShopId = dto.BarberShopId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == id && x.BarberShop!.OwnerUserId == userId);

        if (service is null)
            return NotFound(new { message = "Serviço não encontrado." });

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}