namespace TechLabManagement.Core.Models;

public sealed record BookingVolumeByRoleHour(string Role, int Hour, int Count);
public sealed record ApprovalRateByRole(string Role, int Approved, int Total);
public sealed record AverageInductionScoreByLab(Guid LabId, string LabName, double AverageScore);
public sealed record EquipmentBookedHoursByWeek(Guid EquipmentId, string EquipmentName, int IsoWeek, double Hours);


