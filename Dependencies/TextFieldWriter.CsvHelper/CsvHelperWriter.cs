// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Globalization;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Defines <see cref="Database"/> to the text fields writer.
    /// </summary>
    public class CsvHelperWriter : ITextFieldWriter<Database>
    {
        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="path">The fields file path.</param>
        /// <param name="database">The source records database.</param>
        public void Write(string path, Database database)
        {
            try
            {
                using (var writer = new System.IO.StringWriter())
                {
                    var configuration = new CsvHelper.Configuration.CsvConfiguration();
                    configuration.Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    configuration.CultureInfo = CultureInfo.CurrentCulture;

                    using (var csv = new CsvHelper.CsvWriter(writer, configuration))
                    {
                        // columns
                        csv.WriteField(Database.IdColumnName);
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

                    using (var file = System.IO.File.CreateText(path))
                    {
                        file.Write(writer.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }
    }
}
