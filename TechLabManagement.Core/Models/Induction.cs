using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public sealed class InductionTest : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid LabId { get; set; }
	public List<Question> Questions { get; set; } = new();
	public int PassThresholdPercentage { get; set; } = 80;
}

public sealed class Question : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Text { get; set; } = string.Empty;
	public List<string> Options { get; set; } = new();
	public int CorrectOptionIndex { get; set; }
}


