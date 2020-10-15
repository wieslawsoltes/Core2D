using System;
using System.Globalization;
using System.IO;
using Core2D;
using Core2D.Data;
using CSV = CsvHelper;

namespace Core2D.TextFieldWriter.CsvHelper
{
    /// <summary>
    /// Defines <see cref="Database"/> to the text fields writer.
    /// </summary>
    public sealed class CsvHelperWriter : ITextFieldWriter<Database>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvHelperWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CsvHelperWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Csv (CsvHelper)";

        /// <inheritdoc/>
        public string Extension { get; } = "csv";

        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="stream">The fields file stream.</param>
        /// <param name="database">The source records database.</param>
        public void Write(Stream stream, Database database)
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

            var fileIO = _serviceProvider.GetService<IFileSystem>();
            if (fileIO != null)
            {
                fileIO.WriteUtf8Text(stream, csv);
            }
        }
    }
}
