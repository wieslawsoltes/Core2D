// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Interfaces;
using System.Collections.Generic;
using System.Globalization;

namespace Dependencies
{
    /// <summary>
    /// Defines the text fields to <see cref="XDatabase"/> reader.
    /// </summary>
    public class CsvHelperReader : ITextFieldReader<XDatabase>
    {
        private IEnumerable<string[]> ReadInternal(string path)
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                var configuration = new CsvHelper.Configuration.CsvConfiguration();
                configuration.Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                configuration.CultureInfo = CultureInfo.CurrentCulture;
                configuration.AllowComments = true;
                configuration.Comment = '#';
                using (var parser = new CsvHelper.CsvParser(reader, configuration))
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
        /// <returns>The new instance of the <see cref="XDatabase"/> class</returns>
        public XDatabase Read(string path)
        {
            var fields = ReadInternal(path);
            var name = System.IO.Path.GetFileNameWithoutExtension(path);
            return XDatabase.FromFields(name, fields);
        }
    }
}
