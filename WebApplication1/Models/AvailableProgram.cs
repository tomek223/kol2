namespace WebApplication1.Models;

public class AvailableProgram
{
    public int AvailableProgramId { get; set; }

    public int WashingMachineId { get; set; }
    public WashingMachine WashingMachine { get; set; } = null!;

    public int ProgramId { get; set; }
    public Program Program { get; set; } = null!;

    public decimal Price { get; set; }

    public ICollection<PurchaseHistory> Purchases { get; set; } = new List<PurchaseHistory>();
}