using System.Globalization;
using System.IO;
using CsvHelper;

namespace ActuarialModelApp.Infrastructure
{
    public static class ExportService
    {
        public static void ExportToCsv<T>(IEnumerable<T> data, string path)
        {
            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(data);
        }
    }
}
