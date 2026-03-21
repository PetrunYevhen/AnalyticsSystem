using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Analytics.Application.Common.Csv;

public static class ExportToCsv
{
    private static readonly UTF8Encoding Utf8NoBom = new(false);
    private static readonly byte[] Utf8Bom = Encoding.UTF8.GetPreamble();

    private static readonly CsvConfiguration Config = new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
    };

    public static async Task<byte[]> ToCsv<T>(
        IEnumerable<T> records,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(records);

        using var ms = new MemoryStream();
        await WriteAsync(records, ms, cancellationToken);
        return ms.ToArray();
    }

    public static async Task WriteAsync<T>(
        IEnumerable<T> records,
        Stream destination,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(destination);

        await destination.WriteAsync(Utf8Bom, cancellationToken);

        await using var writer = new StreamWriter(destination, Utf8NoBom, leaveOpen: true);
        await using var csv = new CsvWriter(writer, Config, leaveOpen: true);

        await csv.WriteRecordsAsync(records, cancellationToken);
        await csv.FlushAsync();
        await writer.FlushAsync(cancellationToken);
    }
}