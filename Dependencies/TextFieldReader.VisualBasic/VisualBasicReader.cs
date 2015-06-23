// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class VisualBasicReader : ITextFieldReader<Database>
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
            return Database.Create(name, fields);
        }
    }
}
