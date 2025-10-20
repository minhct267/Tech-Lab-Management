using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace TechLabManagement.ViewModels;

public sealed class AnalyticsViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-14);
	public DateTime ToDate { get; set; } = DateTime.Today.AddDays(14);

	public ObservableCollection<BookingVolumeByRoleHour> BookingByRoleHour { get; } = new();
	public ObservableCollection<ApprovalRateByRole> ApprovalRateByRole { get; } = new();
	public ObservableCollection<AverageInductionScoreByLab> AvgInductionByLab { get; } = new();
	public ObservableCollection<EquipmentBookedHoursByWeek> EquipmentHoursByWeek { get; } = new();

	public ICommand RefreshCommand { get; }
	public ICommand ExportCommand { get; }

	// Charts
	private PlotModel _bookingByRoleHourPlot = new();
	public PlotModel BookingByRoleHourPlot
	{
		get => _bookingByRoleHourPlot;
		private set => SetProperty(ref _bookingByRoleHourPlot, value);
	}

	private PlotModel _approvalRatePlot = new();
	public PlotModel ApprovalRatePlot
	{
		get => _approvalRatePlot;
		private set => SetProperty(ref _approvalRatePlot, value);
	}

	private PlotModel _equipmentHoursPlot = new();
	public PlotModel EquipmentHoursPlot
	{
		get => _equipmentHoursPlot;
		private set => SetProperty(ref _equipmentHoursPlot, value);
	}

	/* Initializes commands and builds initial charts/tables */
	public AnalyticsViewModel()
	{
		RefreshCommand = new RelayCommand(_ => Refresh());
		ExportCommand = new RelayCommand(_ => ExportCsv());
		Refresh();
	}

	/* Reloads data from Analytics service and rebuilds plots */
	private void Refresh()
	{
		BookingByRoleHour.Clear();
		ApprovalRateByRole.Clear();
		AvgInductionByLab.Clear();
		EquipmentHoursByWeek.Clear();

		foreach (var x in _svc.Analytics.GetBookingVolumeByRoleAndHour(FromDate, ToDate))
			BookingByRoleHour.Add(x);
		foreach (var x in _svc.Analytics.GetAccessApprovalRateByRole(FromDate, ToDate))
			ApprovalRateByRole.Add(x);
		foreach (var x in _svc.Analytics.GetAverageInductionScoreByLab())
			AvgInductionByLab.Add(x);
		foreach (var x in _svc.Analytics.GetEquipmentBookedHoursByWeek(FromDate, ToDate))
			EquipmentHoursByWeek.Add(x);

		BuildCharts();
	}

	/* Exports current analytics tables to CSV files on Desktop */
	private void ExportCsv()
	{
		var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TechLabAnalytics");
		Directory.CreateDirectory(dir);

		WriteCsv(Path.Combine(dir, "booking_by_role_hour.csv"), new[] { "Role,Hour,Count" },
			BookingByRoleHour.Select(r => $"{r.Role},{r.Hour},{r.Count}"));
		WriteCsv(Path.Combine(dir, "approval_rate_by_role.csv"), new[] { "Role,Approved,Total" },
			ApprovalRateByRole.Select(r => $"{r.Role},{r.Approved},{r.Total}"));
		WriteCsv(Path.Combine(dir, "avg_induction_by_lab.csv"), new[] { "Lab,AverageScore" },
			AvgInductionByLab.Select(r => $"{r.LabName},{Math.Round(r.AverageScore,1)}"));
		WriteCsv(Path.Combine(dir, "equipment_hours_by_week.csv"), new[] { "Equipment,IsoWeek,Hours" },
			EquipmentHoursByWeek.Select(r => $"{r.EquipmentName},{r.IsoWeek},{Math.Round(r.Hours,2)}"));
	}

	/* Writes a simple CSV file with UTF-8 encoding */
	private static void WriteCsv(string path, IEnumerable<string> header, IEnumerable<string> lines)
	{
		using var sw = new StreamWriter(path, false, Encoding.UTF8);
		sw.WriteLine(string.Join('\n', header));
		foreach (var l in lines) sw.WriteLine(l);
	}

	/* Builds OxyPlot models for each analytics visualization */
	private void BuildCharts()
	{
		// BookingByRoleHour -> LineSeries per role across hour 0..23
		var pm1 = new PlotModel { Title = "Booking Volume by Role x Hour" };
		pm1.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Hour", Minimum = 0, Maximum = 23, MajorStep = 1, MinorStep = 1 });
		pm1.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Title = "Count" });
		var roles = BookingByRoleHour.Select(x => x.Role).Distinct().OrderBy(x => x).ToList();
		foreach (var role in roles)
		{
			var dict = BookingByRoleHour.Where(x => x.Role == role).ToDictionary(x => x.Hour, x => x.Count);
			var ls = new LineSeries { Title = role, MarkerType = MarkerType.Circle, MarkerSize = 3 };
			for (int h = 0; h < 24; h++)
			{
				var val = dict.TryGetValue(h, out var c) ? c : 0;
				ls.Points.Add(new DataPoint(h, val));
			}
			pm1.Series.Add(ls);
		}
		BookingByRoleHourPlot = pm1;

		// Approval rate by role -> Bar (Approved/Total)
		var pm2 = new PlotModel { Title = "Access Approvals by Role" };
		pm2.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, ItemsSource = ApprovalRateByRole.Select(x => x.Role).ToList() });
		pm2.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Title = "Count" });
		var approvedSeries = new BarSeries { Title = "Approved", FillColor = OxyColors.ForestGreen };
		foreach (var r in ApprovalRateByRole) approvedSeries.Items.Add(new BarItem(r.Approved));
		pm2.Series.Add(approvedSeries);
		ApprovalRatePlot = pm2;

		// Equipment hours by week -> Line per equipment (top 5)
		var pm3 = new PlotModel { Title = "Equipment Hours by ISO Week" };
		pm3.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "ISO Week" });
		pm3.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Hours" });
		var topEq = EquipmentHoursByWeek
			.GroupBy(x => x.EquipmentId)
			.Select(g => new { Id = g.Key, Name = g.First().EquipmentName, Hours = g.Sum(x => x.Hours) })
			.OrderByDescending(x => x.Hours)
			.Take(5)
			.ToList();
		foreach (var eq in topEq)
		{
			var ls = new LineSeries { Title = eq.Name, MarkerType = MarkerType.Circle, MarkerSize = 3 };
			foreach (var p in EquipmentHoursByWeek.Where(x => x.EquipmentId == eq.Id).OrderBy(x => x.IsoWeek))
			{
				ls.Points.Add(new DataPoint(p.IsoWeek, p.Hours));
			}
			pm3.Series.Add(ls);
		}
		EquipmentHoursPlot = pm3;
	}
}
