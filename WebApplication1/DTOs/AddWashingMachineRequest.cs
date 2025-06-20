namespace WebApplication1.DTOs;

public class AddWashingMachineRequest
{
    public WashingMachineCreateDto WashingMachine { get; set; } = null!;
    public List<ProgramAssignDto> AvailablePrograms { get; set; } = new();
}