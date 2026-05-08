using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.Appointment;
using BarberSaaS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] int? barberId)
    {
        var userId = GetUserId();

        var query = _context.Appointments
            .AsNoTracking()
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .Where(x => x.Barber!.BarberShop!.OwnerUserId == userId)
            .AsQueryable();

        if (date.HasValue)
        {
            var startDate = date.Value.Date;
            var endDate = startDate.AddDays(1);

            query = query.Where(x => x.StartsAt >= startDate && x.StartsAt < endDate);
        }

        if (barberId.HasValue)
        {
            query = query.Where(x => x.BarberId == barberId.Value);
        }

        var appointments = await query
            .OrderBy(x => x.StartsAt)
            .Select(x => new AppointmentResponseDto
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                StartsAt = x.StartsAt,
                Status = x.Status,
                BarberId = x.BarberId,
                BarberName = x.Barber != null ? x.Barber.Name : string.Empty,
                ServiceId = x.ServiceId,
                ServiceName = x.Service != null ? x.Service.Name : string.Empty,
                ServicePrice = x.Service != null ? x.Service.Price : 0,
                ServiceDurationMinutes = x.Service != null ? x.Service.DurationMinutes : 0
            })
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetById(int id)
    {
        var userId = GetUserId();

        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .Where(x => x.Id == id && x.Barber!.BarberShop!.OwnerUserId == userId)
            .Select(x => new AppointmentResponseDto
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                StartsAt = x.StartsAt,
                Status = x.Status,
                BarberId = x.BarberId,
                BarberName = x.Barber != null ? x.Barber.Name : string.Empty,
                ServiceId = x.ServiceId,
                ServiceName = x.Service != null ? x.Service.Name : string.Empty,
                ServicePrice = x.Service != null ? x.Service.Price : 0,
                ServiceDurationMinutes = x.Service != null ? x.Service.DurationMinutes : 0
            })
            .FirstOrDefaultAsync();

        if (appointment is null)
            return NotFound(new { message = "Agendamento não encontrado." });

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        var userId = GetUserId();

        if (dto.StartsAt <= DateTime.UtcNow)
            return BadRequest(new { message = "Não é possível agendar no passado." });

        var barber = await _context.Barbers
            .AsNoTracking()
            .Include(x => x.BarberShop)
            .FirstOrDefaultAsync(x => x.Id == dto.BarberId && x.BarberShop!.OwnerUserId == userId);

        if (barber is null)
            return BadRequest(new { message = "Barbeiro não encontrado ou não pertence ao usuário logado." });

        var service = await _context.Services
            .AsNoTracking()
            .Include(x => x.BarberShop)
            .FirstOrDefaultAsync(x => x.Id == dto.ServiceId && x.BarberShop!.OwnerUserId == userId);

        if (service is null)
            return BadRequest(new { message = "Serviço não encontrado ou não pertence ao usuário logado." });

        if (barber.BarberShopId != service.BarberShopId)
            return BadRequest(new { message = "Barbeiro e serviço não pertencem à mesma barbearia." });

        var newAppointmentEnd = dto.StartsAt.AddMinutes(service.DurationMinutes);

        var barberAppointments = await _context.Appointments
            .AsNoTracking()
            .Where(x => x.BarberId == dto.BarberId && x.Status == "Scheduled")
            .Include(x => x.Service)
            .ToListAsync();

        var hasConflict = barberAppointments.Any(existing =>
        {
            var existingEnd = existing.StartsAt.AddMinutes(existing.Service?.DurationMinutes ?? 0);

            return dto.StartsAt < existingEnd && newAppointmentEnd > existing.StartsAt;
        });

        if (hasConflict)
            return BadRequest(new { message = "Já existe um agendamento conflitante para esse barbeiro." });

        var appointment = new Appointment
        {
            CustomerName = dto.CustomerName.Trim(),
            CustomerPhone = dto.CustomerPhone.Trim(),
            StartsAt = dto.StartsAt,
            Status = "Scheduled",
            BarberId = dto.BarberId,
            ServiceId = dto.ServiceId
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var response = await _context.Appointments
            .AsNoTracking()
            .Include(x => x.Barber)
            .Include(x => x.Service)
            .Where(x => x.Id == appointment.Id)
            .Select(x => new AppointmentResponseDto
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                StartsAt = x.StartsAt,
                Status = x.Status,
                BarberId = x.BarberId,
                BarberName = x.Barber != null ? x.Barber.Name : string.Empty,
                ServiceId = x.ServiceId,
                ServiceName = x.Service != null ? x.Service.Name : string.Empty,
                ServicePrice = x.Service != null ? x.Service.Price : 0,
                ServiceDurationMinutes = x.Service != null ? x.Service.DurationMinutes : 0
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        var userId = GetUserId();

        var allowedStatuses = new[] { "Scheduled", "Completed", "Cancelled" };

        if (!allowedStatuses.Contains(dto.Status))
            return BadRequest(new { message = "Status inválido. Use Scheduled, Completed ou Cancelled." });

        var appointment = await _context.Appointments
            .Include(x => x.Barber)
            .ThenInclude(x => x!.BarberShop)
            .FirstOrDefaultAsync(x => x.Id == id && x.Barber!.BarberShop!.OwnerUserId == userId);

        if (appointment is null)
            return NotFound(new { message = "Agendamento não encontrado." });

        appointment.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var appointment = await _context.Appointments
            .Include(x => x.Barber)
            .ThenInclude(x => x!.BarberShop)
            .FirstOrDefaultAsync(x => x.Id == id && x.Barber!.BarberShop!.OwnerUserId == userId);

        if (appointment is null)
            return NotFound(new { message = "Agendamento não encontrado." });

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}