using System;
using System.Globalization;
using System.IO;
using Core2D;
using Core2D.Data;
using CSV = CsvHelper;

namespace Core2D.TextFieldWriter.CsvHelper
{
    public sealed class CsvHelperWriter : ITextFieldWriter<DatabaseViewModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public CsvHelperWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Csv (CsvHelper)";

        public string Extension { get; } = "csv";

        public void Write(Stream stream, DatabaseViewModel database)
        {
            using var writer = new StringWriter();

            var configuration = new CSV.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator,
                CultureInfo = CultureInfo.CurrentCulture
            };

            using (var csvWriter = new CSV.CsvWriter(writer, configuration))
            {
                // Columns

                csvWriter.WriteField(database.IdColumnName);
                foreach (var column in database.Columns)
                {
                    csvWriter.WriteField(column.Name);
                }
                csvWriter.NextRecord();

                // Records

                foreach (var record in database.Records)
                {
                    csvWriter.WriteField(record.Id.ToString());
                    foreach (var value in record.Values)
                    {
                        csvWriter.WriteField(value.Content);
                    }
                    csvWriter.NextRecord();
                }
            }

            var csv = writer.ToString();

            var fileSystem = _serviceProvider.GetService<IFileSystem>();
            fileSystem?.WriteUtf8Text(stream, csv);
        }
    }
}
