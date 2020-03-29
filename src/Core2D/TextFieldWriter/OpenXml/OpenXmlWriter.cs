using System;
using System.IO;
using Core2D.Data;
using Core2D.Interfaces;

namespace Core2D.TextFieldWriter.OpenXml
{
    /// <summary>
    /// Defines <see cref="IDatabase"/> to the text fields writer.
    /// </summary>
    public sealed class OpenXmlWriter : ITextFieldWriter<IDatabase>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenXmlWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public OpenXmlWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Xlsx (OpenXml)";

        /// <inheritdoc/>
        public string Extension { get; } = "xlsx";

        private void ToValues(IDatabase database, out object[,] values)
        {
            int nRows = database.Records.Length + 1;
            int nColumns = database.Columns.Length + 1;
            values = new object[nRows, nColumns];
            values[0, 0] = database.IdColumnName;

            // Columns

            for (int i = 0; i < database.Columns.Length; i++)
            {
                var column = database.Columns[i];
                values[0, i + 1] = column.Name;
            }

            // Rows

            for (int i = 0; i < database.Records.Length; i++)
            {
                var record = database.Records[i];

                values[i + 1, 0] = record.Id.ToString();

                for (int j = 0; j < record.Values.Length; j++)
                {
                    var value = record.Values[j];
                    values[i + 1, j + 1] = value.Content;
                }    
            }
        }

        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="stream">The fields file stream.</param>
        /// <param name="database">The source records database.</param>
        public void Write(Stream stream, IDatabase database)
        {
            object[,] values;
            ToValues(database, out values);

            uint nRows = (uint)database.Records.Length + 1U;
            uint nColumns = (uint)database.Columns.Length + 1U;
            OpenXmlSpreadsheet.Write(stream, values, nRows, nColumns, "Database");
        }
    }
}
