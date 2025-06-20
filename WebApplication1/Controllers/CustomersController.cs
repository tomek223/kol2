using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DbContext;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;
    public CustomersController(AppDbContext context) => _context = context;

    [HttpGet("{customerId}/purchases")]
    public async Task<IActionResult> GetCustomerPurchases(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Purchases)
            .ThenInclude(p => p.AvailableProgram)
            .ThenInclude(ap => ap.WashingMachine)
            .Include(c => c.Purchases)
            .ThenInclude(p => p.AvailableProgram)
            .ThenInclude(ap => ap.Program)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null) return NotFound();

        var result = new
        {
            customer.FirstName,
            customer.LastName,
            customer.PhoneNumber,
            purchases = customer.Purchases.Select(p => new
            {
                date = p.PurchaseDate,
                rating = p.Rating,
                price = p.AvailableProgram.Price,
                washingMachine = new
                {
                    serial = p.AvailableProgram.WashingMachine.SerialNumber,
                    maxWeight = p.AvailableProgram.WashingMachine.MaxWeight
                },
                program = new
                {
                    name = p.AvailableProgram.Program.Name,
                    duration = p.AvailableProgram.Program.DurationMinutes
                }
            }).ToList()
        };

        return Ok(result);
    }
}