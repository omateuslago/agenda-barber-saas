using BarberSaaS.API.Data;
using BarberSaaS.API.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberSaaS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponseDto>> Get()
    {
        var userId = GetUserId();

        var now = DateTime.UtcNow;

        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

        var totalBarbers = await _context.Barbers
            .AsNoTracking()
            .Where(x => x.BarberShop!.OwnerUserId == userId)
            .CountAsync();

        var totalServices = await _context.Services
            .AsNoTracking()
            .Where(x => x.BarberShop!.OwnerUserId == userId)
            .CountAsync();

        var appointmentsToday = await _context.Appointments
            .AsNoTracking()
            .Where(x =>
                x.Barber!.BarberShop!.OwnerUserId == userId &&
                x.StartsAt >= today &&
                x.StartsAt < tomorrow &&
                x.Status != "Cancelled")
            .CountAsync();

        var appointmentsThisMonth = await _context.Appointments
            .AsNoTracking()
            .Where(x =>
                x.Barber!.BarberShop!.OwnerUserId == userId &&
                x.StartsAt >= firstDayOfMonth &&
                x.StartsAt < firstDayOfNextMonth &&
                x.Status != "Cancelled")
            .CountAsync();

        var monthlyRevenue = await _context.Appointments
            .AsNoTracking()
            .Where(x =>
                x.Barber!.BarberShop!.OwnerUserId == userId &&
                x.StartsAt >= firstDayOfMonth &&
                x.StartsAt < firstDayOfNextMonth &&
                x.Status == "Completed")
            .SumAsync(x => x.Service!.Price);

        var response = new DashboardResponseDto
        {
            TotalBarbers = totalBarbers,
            TotalServices = totalServices,
            AppointmentsToday = appointmentsToday,
            AppointmentsThisMonth = appointmentsThisMonth,
            MonthlyRevenue = monthlyRevenue
        };

        return Ok(response);
    }
}