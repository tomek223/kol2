using Microsoft.EntityFrameworkCore;
using WebApplication1.DbContext;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ICustomerService
{
    Task<CustomerPurchasesDto?> GetCustomerPurchasesAsync(int customerId);
}

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;
    public CustomerService(AppDbContext context) => _context = context;

    public async Task<CustomerPurchasesDto?> GetCustomerPurchasesAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Purchases)
                .ThenInclude(ph => ph.AvailableProgram)
                    .ThenInclude(ap => ap.Program)
            .Include(c => c.Purchases)
                .ThenInclude(ph => ph.AvailableProgram)
                    .ThenInclude(ap => ap.WashingMachine)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null) return null;

        return new CustomerPurchasesDto
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Purchases = customer.Purchases.Select(p => new PurchaseDto
            {
                Date = p.PurchaseDate,
                Rating = p.Rating,
                Price = p.AvailableProgram.Price,
                WashingMachine = new WashingMachineDto
                {
                    Serial = p.AvailableProgram.WashingMachine.SerialNumber,
                    MaxWeight = p.AvailableProgram.WashingMachine.MaxWeight
                },
                Program = new ProgramDto
                {
                    Name = p.AvailableProgram.Program.Name,
                    Duration = p.AvailableProgram.Program.DurationMinutes
                }
            }).ToList()

        };
    }
}

public interface IWashingMachineService
{
    Task<bool> AddWashingMachineAsync(AddWashingMachineRequest request);
}

public class WashingMachineService : IWashingMachineService
{
    private readonly AppDbContext _context;
    public WashingMachineService(AppDbContext context) => _context = context;

    public async Task<bool> AddWashingMachineAsync(AddWashingMachineRequest request)
    {
        if (request.WashingMachine.MaxWeight < 8)
            throw new ArgumentException("MaxWeight must be at least 8kg");

        if (await _context.WashingMachines.AnyAsync(w => w.SerialNumber == request.WashingMachine.SerialNumber))
            throw new InvalidOperationException("Washing machine with this serial number already exists");

        var washingMachine = new WashingMachine
        {
            SerialNumber = request.WashingMachine.SerialNumber,
            MaxWeight = (decimal)request.WashingMachine.MaxWeight
        };

        var availablePrograms = new List<AvailableProgram>();

        foreach (var ap in request.AvailablePrograms)
        {
            if (ap.Price > 25)
                throw new ArgumentException($"Price for '{ap.ProgramName}' exceeds maximum");

            var program = await _context.Programs.FirstOrDefaultAsync(p => p.Name == ap.ProgramName);
            if (program == null)
                throw new InvalidOperationException($"Program '{ap.ProgramName}' not found");

            availablePrograms.Add(new AvailableProgram
            {
                Program = program,
                Price = (decimal)ap.Price,
                WashingMachine = washingMachine
            });
        }

        washingMachine.AvailablePrograms = availablePrograms;
        _context.WashingMachines.Add(washingMachine);
        await _context.SaveChangesAsync();
        return true;
    }
}
