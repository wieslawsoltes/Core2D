// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Data;
using Core2D.Interfaces;
using System.Globalization;
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

        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="path">The fields file path.</param>
        /// <param name="fs">The file system.</param>
        /// <param name="database">The source records database.</param>
        void ITextFieldWriter<IDatabase>.Write(string path, IFileSystem fs, IDatabase database)
        {
            using (var writer = new System.IO.StringWriter())
            {
                var configuration = new CSV.Configuration.Configuration
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

                fs.WriteUtf8Text(path, writer.ToString());
            }
        }
    }
}
