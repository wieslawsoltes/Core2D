#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.ViewModels.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Core2D.Modules.TextFieldWriter.OpenXml;

public sealed class OpenXmlWriter : ITextFieldWriter<DatabaseViewModel>
{
    private readonly IServiceProvider? _serviceProvider;

    public OpenXmlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Xlsx (OpenXml)";

    public string Extension => "xlsx";

    private void ToValues(DatabaseViewModel? database, out object[,]? values)
    {
        if (database is null)
        {
            values = null;
            return;
        }
            
        var nRows = database.Records.Length + 1;
        var nColumns = database.Columns.Length + 1;
        values = new object[nRows, nColumns];
        values[0, 0] = database.IdColumnName ?? "Id";

        // Columns

        for (var i = 0; i < database.Columns.Length; i++)
        {
            var column = database.Columns[i];
            values[0, i + 1] = column.Name;
        }

        // Rows

        for (var i = 0; i < database.Records.Length; i++)
        {
            var record = database.Records[i];

            values[i + 1, 0] = record.Id.ToString();

            for (var j = 0; j < record.Values.Length; j++)
            {
                var value = record.Values[j];
                values[i + 1, j + 1] = value.Content ?? "";
            }
        }
    }

    private static void WriteValues(SheetData sheetData, object[,] values, uint nRows, uint nColumns)
    {
        for (var r = 0; r < nRows; r++)
        {
            var row = new Row();
            sheetData.Append(new OpenXmlElement[] { row });

            Cell? previous = null;
            for (int c = 0; c < nColumns; c++)
            {
                Cell cell = new Cell();
                row.InsertAfter(cell, previous);
                cell.CellValue = new CellValue(values[r, c].ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                previous = cell;
            }
        }
    }

    private static string ToRange(uint columnNumber)
    {
        var end = columnNumber;
        var name = string.Empty;
        while (end > 0)
        {
            var modulo = (end - 1) % 26;
            name = Convert.ToChar(65 + modulo) + name;
            end = (end - modulo) / 26;
        }
        return name;
    }

    private static string ToRange(uint startRow, uint endRow, uint startColumn, uint endColumn)
    {
        return ToRange(startColumn) + startRow.ToString() + ":" + ToRange(endColumn) + endRow.ToString();
    }

    private static void WriteTable(WorksheetPart worksheetPart, uint tableID, object[,] values, uint nRows, uint nColumns)
    {
        string range = ToRange(1, nRows, 1, nColumns);

        var autoFilter = new AutoFilter
        {
            Reference = range
        };

        var tableColumns = new TableColumns()
        {
            Count = nColumns
        };

        for (int c = 0; c < nColumns; c++)
        {
            var column = new TableColumn()
            {
                Id = (UInt32Value)(c + 1U),
                Name = values[0, c].ToString()
            };
            tableColumns.Append(column);
        }

        var tableStyleInfo = new TableStyleInfo()
        {
            Name = "TableStyleMedium2",
            ShowFirstColumn = true,
            ShowLastColumn = false,
            ShowRowStripes = true,
            ShowColumnStripes = true
        };

        var table = new Table()
        {
            Id = tableID,
            Name = $"Table{tableID}",
            DisplayName = $"Table{tableID}",
            Reference = range,
            TotalsRowShown = false
        };

        table.Append(autoFilter);
        table.Append(tableColumns);
        table.Append(tableStyleInfo);

        var tableDefinitionPart = worksheetPart.AddNewPart<TableDefinitionPart>($"rId{tableID}");
        tableDefinitionPart.Table = table;
    }

    public static void Write(Stream stream, object[,] values, uint nRows, uint nColumns, string sheetName)
    {
        var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        if (spreadsheetDocument.WorkbookPart is null)
        {
            return;
        }
        var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

        Sheet sheet = new ()
        {
            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = sheetName
        };
        sheets.Append(sheet);

        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
        WriteValues(sheetData, values, nRows, nColumns);
        WriteTable(worksheetPart, 1U, values, nRows, nColumns);

        workbookPart.Workbook.Save();
        spreadsheetDocument.Close();
    }

    public void Write(Stream stream, DatabaseViewModel? database)
    {
        if (database is null)
        {
            return;
        }

        ToValues(database, out var values);
        if (values is { })
        {
            var nRows = (uint)database.Records.Length + 1U;
            var nColumns = (uint)database.Columns.Length + 1U;
            Write(stream, values, nRows, nColumns, "Database");
        }
    }
}