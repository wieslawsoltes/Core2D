using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Core2D.Modules.TextFieldReader.OpenXml
{
    public sealed class OpenXmlReader : ITextFieldReader<DatabaseViewModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenXmlReader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Xlsx (OpenXml)";

        public string Extension { get; } = "xlsx";

        private static string ToString(Cell c, SharedStringTablePart stringTable)
        {
            if (c.DataType is null)
            {
                return c.CellValue?.Text;
            }

            switch (c.DataType.Value)
            {
                case CellValues.SharedString:
                    {
                        if (stringTable is { })
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

        public static IEnumerable<string[]> ReadFields(Stream stream)
        {
            var spreadsheetDocument = SpreadsheetDocument.Open(stream, false);

            var workbookpart = spreadsheetDocument.WorkbookPart;

            var worksheetPart = workbookpart.WorksheetParts.First();

            var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            var stringTable = workbookpart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            foreach (var row in sheetData.Elements<Row>())
            {
                var fields = row.Elements<Cell>().Select(c => ToString(c, stringTable)).ToArray();
                yield return fields;
            }

            spreadsheetDocument.Close();
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
