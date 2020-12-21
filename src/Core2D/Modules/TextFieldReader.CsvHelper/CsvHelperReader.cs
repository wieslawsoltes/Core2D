#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using CSV = CsvHelper;

namespace Core2D.Modules.TextFieldReader.CsvHelper
{
    public sealed class CsvHelperReader : ITextFieldReader<DatabaseViewModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public CsvHelperReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Csv (CsvHelper)";

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
                if (fields is null)
                {
                    break;
                }
                yield return fields;
            }
        }

        public DatabaseViewModel Read(Stream stream)
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
