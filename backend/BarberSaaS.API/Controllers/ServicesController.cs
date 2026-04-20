using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.Service;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponseDto>>> GetAll()
    {
        var services = await _context.Services
            .AsNoTracking()
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
        var service = await _context.Services
            .AsNoTracking()
            .Where(x => x.Id == id)
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
        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe." });

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
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service is null)
            return NotFound(new { message = "Serviço não encontrado." });

        var barberShopExists = await _context.BarberShops
            .AnyAsync(x => x.Id == dto.BarberShopId);

        if (!barberShopExists)
            return BadRequest(new { message = "A barbearia informada não existe." });

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
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service is null)
            return NotFound(new { message = "Serviço não encontrado." });

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}