// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Recent project.
    /// </summary>
    public class RecentProject : ObservableObject
    {
        private string _name;
        private string _path;

        /// <summary>
        /// Gets or sets recent project name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets recent project path.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// Creates a new <see cref="RecentProject"/> instance.
        /// </summary>
        /// <param name="name">The recent project name.</param>
        /// <param name="path">The recent project path.</param>
        /// <returns>The new instance of the <see cref="RecentProject"/> class.</returns>
        public static RecentProject Create(string name, string path)
        {
            return new RecentProject()
            {
                Name = name,
                Path = path
            };
        }
    }
}
