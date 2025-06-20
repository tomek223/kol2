namespace WebApplication1.Models;

public class WashingMachine
{
    public int WashingMachineId { get; set; }
    public decimal MaxWeight { get; set; }
    public string SerialNumber { get; set; } = null!;

    public ICollection<AvailableProgram> AvailablePrograms { get; set; } = new List<AvailableProgram>();
}