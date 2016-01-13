// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Recent projects.
    /// </summary>
    public class Recent : ObservableObject
    {
        private ImmutableArray<RecentProject> _recentProjects = ImmutableArray.Create<RecentProject>();
        private RecentProject _currentRecentProject = default(RecentProject);
        
        /// <summary>
        /// Gets or sets recent projects.
        /// </summary>
        public ImmutableArray<RecentProject> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }
        
        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        public RecentProject CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Recent"/> instance.
        /// </summary>
        /// <param name="recentProjects">The recent projects.</param>
        /// <param name="currentRecentProject">The current recent project.</param>
        /// <returns>The new instance of the <see cref="Recent"/> class.</returns>
        public static Recent Create(ImmutableArray<RecentProject> recentProjects, RecentProject currentRecentProject)
        {
            return new Recent()
            {
                RecentProjects = recentProjects,
                CurrentRecentProject = currentRecentProject
            };
        }
    }
}
