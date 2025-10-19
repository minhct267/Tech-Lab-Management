using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IAnalyticsService
{
	IEnumerable<BookingVolumeByRoleHour> GetBookingVolumeByRoleAndHour(DateTime from, DateTime to);
	IEnumerable<ApprovalRateByRole> GetAccessApprovalRateByRole(DateTime from, DateTime to);
	IEnumerable<AverageInductionScoreByLab> GetAverageInductionScoreByLab();
	IEnumerable<EquipmentBookedHoursByWeek> GetEquipmentBookedHoursByWeek(DateTime from, DateTime to);
}


