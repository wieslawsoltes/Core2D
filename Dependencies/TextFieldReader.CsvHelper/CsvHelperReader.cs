// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvHelperReader : ITextFieldReader<Database>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Database Read(string path)
        {
            var fields = ReadInternal(path);
            var name = System.IO.Path.GetFileNameWithoutExtension(path);
            return Database.Create(name, fields);
        }
    }
}
