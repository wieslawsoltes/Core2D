// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public class VisualBasicReader : ITextFieldReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<string[]> ReadInternal(string path)
        {
            // TextFieldParser
            // reference: Microsoft.VisualBasic
            // namespace: Microsoft.VisualBasic.FileIO
            using (var parser = new TextFieldParser(path))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { ";" });
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    yield return fields;
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

            var db = Database.Create(name);

            var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(c));
            var columns = ImmutableArray.CreateRange<Column>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == "Id")
            {
                // use existing record Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                v.FirstOrDefault(),
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }
            else
            {
                // create records with new Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }

            return db;
        }
    }
}
