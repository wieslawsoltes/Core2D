using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core2D.Data;
using Core2D.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Core2D.TextFieldReader.OpenXml
{
    /// <summary>
    /// Defines the text fields to <see cref="IDatabase"/> reader.
    /// </summary>
    public sealed class OpenXmlReader : ITextFieldReader<IDatabase>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenXmlReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public OpenXmlReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Xlsx (OpenXml)";

        /// <inheritdoc/>
        public string Extension { get; } = "xlsx";

        public static IEnumerable<string[]> ReadFields(Stream stream)
        {
            var spreadsheetDocument = SpreadsheetDocument.Open(stream, false);

            var workbookpart = spreadsheetDocument.WorkbookPart;

            var worksheetPart = workbookpart.WorksheetParts.First();

            var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            var stringTable = workbookpart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            string ToString(Cell c)
            {
                if (c.DataType == null)
                {
                    return c.CellValue.Text;
                }

                switch (c.DataType.Value)
                {
                    case CellValues.SharedString:
                        {
                            if (stringTable != null)
                            {
                                int index = int.Parse(c.InnerText);
                                var value = stringTable.SharedStringTable.ElementAt(index).InnerText;
                                return value;
                            }
                        }
                        break;
                    case CellValues.Boolean:
                        {
                            return c.InnerText switch
                            {
                                "0" => "FALSE",
                                _ => "TRUE",
                            };
                        }
                    case CellValues.Number:
                        return c.InnerText;
                    case CellValues.Error:
                        return c.InnerText;
                    case CellValues.String:
                        return c.InnerText;
                    case CellValues.InlineString:
                        return c.InnerText;
                    case CellValues.Date:
                        return c.InnerText;
                }

                return null;
            }

            foreach (var row in sheetData.Elements<Row>())
            {
                var fields = row.Elements<Cell>().Select(c => ToString(c)).ToArray();
                yield return fields;
            }

            spreadsheetDocument.Close();
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
