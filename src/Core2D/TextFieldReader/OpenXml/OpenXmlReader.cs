using System;
using System.IO;
using System.Linq;
using Core2D.Data;
using Core2D.Interfaces;

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

        /// <summary>
        /// Read fields from text database file format.
        /// </summary>
        /// <param name="stream">The fields file stream.</param>
        /// <returns>The new instance of the <see cref="IDatabase"/> class</returns>
        public IDatabase Read(Stream stream)
        {
            var fields = OpenXmlSpreadsheet.Read(stream).ToList();
            var name = "Db";
            if (stream is FileStream fileStream)
            {
                name = System.IO.Path.GetFileNameWithoutExtension(fileStream.Name);
            }
            return _serviceProvider.GetService<IFactory>().FromFields(name, fields);
        }
    }
}
