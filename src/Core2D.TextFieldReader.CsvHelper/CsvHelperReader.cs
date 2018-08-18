// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CSV = CsvHelper;

namespace Core2D.TextFieldReader.CsvHelper
{
    /// <summary>
    /// Defines the text fields to <see cref="IDatabase"/> reader.
    /// </summary>
    public sealed class CsvHelperReader : ITextFieldReader<IDatabase>
    {
        private static IEnumerable<string[]> ReadInternal(System.IO.Stream stream)
        {
            using (var reader = new System.IO.StreamReader(stream))
            {
                var configuration = new CSV.Configuration.Configuration();
                configuration.Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                configuration.CultureInfo = CultureInfo.CurrentCulture;
                configuration.AllowComments = true;
                configuration.Comment = '#';
                using (var parser = new CSV.CsvParser(reader, configuration))
                {
                    while (true)
                    {
                        var fields = parser.Read();
                        if (fields == null)
                            break;

                        yield return fields;
                    }
                }
            }
        }

        /// <summary>
        /// Read fields from text database file format.
        /// </summary>
        /// <param name="path">The fields file path.</param>
        /// <param name="fs">The file system.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class</returns>
        IDatabase ITextFieldReader<IDatabase>.Read(string path, IFileSystem fs)
        {
            using (var stream = fs.Open(path))
            {
                var fields = ReadInternal(stream).ToList();
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                return Database.FromFields(name, fields);
            }
        }
    }
}
