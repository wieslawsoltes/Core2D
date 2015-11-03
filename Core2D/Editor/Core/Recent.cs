// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Recent : ObservableObject
    {
        private ImmutableArray<RecentProject> _recentProjects = ImmutableArray.Create<RecentProject>();
        private RecentProject _currentRecentProject = default(RecentProject);
        
        /// <summary>
        ///
        /// </summary>
        public ImmutableArray<RecentProject> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }
        
        /// <summary>
        ///
        /// </summary>
        public RecentProject CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recentProjects"></param>
        /// <param name="currentRecentProject"></param>
        /// <returns></returns>
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
