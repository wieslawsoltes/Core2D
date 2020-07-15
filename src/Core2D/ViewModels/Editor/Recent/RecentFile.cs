
using System;
using System.Collections.Generic;

namespace Core2D.Editor.Recent
{
    /// <summary>
    /// Recent file.
    /// </summary>
    public class RecentFile : ObservableObject
    {
        private string _path;

        /// <summary>
        /// Gets or sets recent file path.
        /// </summary>
        public string Path
        {
            get => _path;
            set => Update(ref _path, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="RecentFile"/> instance.
        /// </summary>
        /// <param name="name">The recent file name.</param>
        /// <param name="path">The recent file path.</param>
        /// <returns>The new instance of the <see cref="RecentFile"/> class.</returns>
        public static RecentFile Create(string name, string path)
        {
            return new RecentFile()
            {
                Name = name,
                Path = path
            };
        }
    }
}
