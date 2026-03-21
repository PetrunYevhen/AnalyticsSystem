using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Analytics.Application.Common.Csv;

public static class ImportCsv
{
    public static Result<List<T>> ParseCsv<T>(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null, 
                HeaderValidated = null, 
                DetectDelimiter = true, 

            });

            return Result.Ok(csv.GetRecords<T>().ToList());
        }
        catch (Exception ex)
        {
            return Result.Fail($"Не вдалося розпарсити CSV: {ex.Message}");
        }
    }
}