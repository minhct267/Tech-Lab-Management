using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public abstract class Equipment : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid LabId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Model { get; set; } = string.Empty;
	public string Manufacturer { get; set; } = string.Empty;
	public bool IsBookable { get; set; } = true;
	public string MaintenanceInfo { get; set; } = string.Empty;
	public List<string> SafetyTags { get; set; } = new();
	public Lab LabRef { get; set; } = null!;

	public virtual bool RequiresSupervisor() => false;
}

public sealed class SolderingStation : Equipment
{
	public override bool RequiresSupervisor() => true; // Electrical work requires supervision in this lab
}

public sealed class MotionPlatform : Equipment
{
	public override bool RequiresSupervisor() => true;
}

public sealed class RobotArm : Equipment
{
	public override bool RequiresSupervisor() => true;
}

public sealed class ThreeDPrinter : Equipment
{
}

public sealed class Oscilloscope : Equipment
{
}

public sealed class SpectrumAnalyzer : Equipment
{
}

public sealed class VRHeadset : Equipment
{
}

public sealed class MicroManipulator : Equipment
{
}

public sealed class SoilShearBox : Equipment
{
}


