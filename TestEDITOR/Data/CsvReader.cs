// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvReader : ITextFieldReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string[]> Read(string path)
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
    }
}
