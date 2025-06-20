namespace WebApplication1.Models;

public class Program
{
    public int ProgramId { get; set; }
    public string Name { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public int TemperatureCelsius { get; set; }

    public ICollection<AvailableProgram> AvailablePrograms { get; set; } = new List<AvailableProgram>();
}