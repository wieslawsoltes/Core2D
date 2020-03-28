using System;
using System.Globalization;
using System.IO;
using Core2D.Data;
using Core2D.Interfaces;
using CSV = CsvHelper;

namespace Core2D.TextFieldWriter.CsvHelper
{
    /// <summary>
    /// Defines <see cref="IDatabase"/> to the text fields writer.
    /// </summary>
    public sealed class CsvHelperWriter : ITextFieldWriter<IDatabase>
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
        public void Write(Stream stream, IDatabase database)
        {
            using var writer = new System.IO.StringWriter();
            var configuration = new CSV.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator,
                CultureInfo = CultureInfo.CurrentCulture
            };

            using (var csv = new CSV.CsvWriter(writer, configuration))
            {
                // columns
                csv.WriteField(database.IdColumnName);
                foreach (var column in database.Columns)
                {
                    csv.WriteField(column.Name);
                }
                csv.NextRecord();

                // records
                foreach (var record in database.Records)
                {
                    csv.WriteField(record.Id.ToString());
                    foreach (var value in record.Values)
                    {
                        csv.WriteField(value.Content);
                    }
                    csv.NextRecord();
                }
            }

            var fs = _serviceProvider.GetService<IFileSystem>();
            if (fs != null)
            {
                fs.WriteUtf8Text(stream, writer.ToString());
            }
        }
    }
}
