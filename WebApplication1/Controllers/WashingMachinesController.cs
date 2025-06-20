using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DbContext;
using WebApplication1.DTOs;
using WebApplication1.Models;
using ProgramModel = WebApplication1.Models.Program;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/washing-machines")]
public class WashingMachinesController : ControllerBase
{
    private readonly AppDbContext _context;
    public WashingMachinesController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> AddWashingMachine([FromBody] AddWashingMachineRequest request)
    {
        if (request.WashingMachine.MaxWeight < 8)
            return BadRequest("MaxWeight must be at least 8kg");

        if (await _context.WashingMachines.AnyAsync(w => w.SerialNumber == request.WashingMachine.SerialNumber))
            return Conflict("Washing machine with this serial number already exists");

        var machine = new WashingMachine
        {
            SerialNumber = request.WashingMachine.SerialNumber,
            MaxWeight = (decimal)request.WashingMachine.MaxWeight
        };

        var availablePrograms = new List<AvailableProgram>();

        foreach (var ap in request.AvailablePrograms)
        {
            if (ap.Price > 25)
                return BadRequest($"Program '{ap.ProgramName}' exceeds max price");

            var program = await _context.Programs
                .OfType<ProgramModel>()
                .FirstOrDefaultAsync(p => p.Name == ap.ProgramName);
            if (program == null)
                return Conflict($"Program '{ap.ProgramName}' does not exist");

            availablePrograms.Add(new AvailableProgram
            {
                WashingMachine = machine,
                Program = program,
                Price = (decimal)ap.Price
            });
        }

        machine.AvailablePrograms = availablePrograms;
        _context.WashingMachines.Add(machine);
        await _context.SaveChangesAsync();

        return Created("/api/washing-machines", null);
    }
}