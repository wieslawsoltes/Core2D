using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core2D;
using Core2D.Data;
using CSV = CsvHelper;

namespace Core2D.TextFieldReader.CsvHelper
{
    /// <summary>
    /// Defines the text fields to <see cref="IDatabase"/> reader.
    /// </summary>
    public sealed class CsvHelperReader : ITextFieldReader<IDatabase>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvHelperReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CsvHelperReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Csv (CsvHelper)";

        /// <inheritdoc/>
        public string Extension { get; } = "csv";

        private static IEnumerable<string[]> ReadFields(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var configuration = new CSV.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator,
                CultureInfo = CultureInfo.CurrentCulture,
                AllowComments = true,
                Comment = '#'
            };

            using var csvParser = new CSV.CsvParser(reader, configuration);
            while (true)
            {
                var fields = csvParser.Read();
                if (fields == null)
                {
                    break;
                }
                yield return fields;
            }
        }

        /// <summary>
        /// Read fields from text database file format.
        /// </summary>
        /// <param name="stream">The fields file stream.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class</returns>
        public IDatabase Read(Stream stream)
        {
            var fields = ReadFields(stream).ToList();

            var name = "Db";
            if (stream is FileStream fileStream)
            {
                name = System.IO.Path.GetFileNameWithoutExtension(fileStream.Name);
            }

            return _serviceProvider.GetService<IFactory>().FromFields(name, fields);
        }
    }
}
