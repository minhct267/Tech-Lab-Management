using System.Text;

namespace TechLabManagement.Utils;

public static class CsvExporter
{
	public static string ToCsv(IEnumerable<IDictionary<string, object?>> rows)
	{
		var sb = new StringBuilder();
		if (!rows.Any()) return string.Empty;
		var headers = rows.First().Keys.ToList();
		sb.AppendLine(string.Join(',', headers));
		foreach (var row in rows)
		{
			var values = headers.Select(h => Escape(row.TryGetValue(h, out var v) ? v : null));
			sb.AppendLine(string.Join(',', values));
		}
		return sb.ToString();
	}

	private static string Escape(object? value)
	{
		var s = value?.ToString() ?? string.Empty;
		if (s.Contains('"') || s.Contains(',') || s.Contains('\n'))
		{
			s = '"' + s.Replace("\"", "\"\"") + '"';
		}
		return s;
	}
}


