// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
