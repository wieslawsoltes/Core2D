using System;

namespace Core2D.Editor
{
    /// <summary>
    /// About information.
    /// </summary>
    public class AboutInfo
    {
        /// <summary>
        /// Gets or sets application title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets application version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets application description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets application copyright.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Gets or sets application license.
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Gets or sets runtime operating system type.
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is Desktop.
        /// </summary>
        public bool IsDesktop { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is Mobile.
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is CoreClr.
        /// </summary>
        public bool IsCoreClr { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is Mono.
        /// </summary>
        public bool IsMono { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is DotNetFramework.
        /// </summary>
        public bool IsDotNetFramework { get; set; }

        /// <summary>
        /// Gets or sets runtime flag indicating whether is Unix.
        /// </summary>
        public bool IsUnix { get; set; }

        /// <summary>
        /// Gets or sets windowing subsystem name.
        /// </summary>
        public string WindowingSubsystemName { get; set; }

        /// <summary>
        /// Gets or sets rendering subsystem name.
        /// </summary>
        public string RenderingSubsystemName { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() =>
                $"{nameof(Title)}: {Title}{Environment.NewLine}" +
                $"{nameof(Version)}: {Version}{Environment.NewLine}" +
                $"{nameof(Description)}: {Description}{Environment.NewLine}" +
                $"{nameof(Copyright)}: {Copyright}{Environment.NewLine}" +
                $"{nameof(License)}: {License}{Environment.NewLine}" +
                $"{nameof(OperatingSystem)}: {OperatingSystem}{Environment.NewLine}" +
                $"{nameof(IsDesktop)}: {IsDesktop}{Environment.NewLine}" +
                $"{nameof(IsMobile)}: {IsMobile}{Environment.NewLine}" +
                $"{nameof(IsCoreClr)}: {IsCoreClr}{Environment.NewLine}" +
                $"{nameof(IsMono)}: {IsMono}{Environment.NewLine}" +
                $"{nameof(IsDotNetFramework)}: {IsDotNetFramework}{Environment.NewLine}" +
                $"{nameof(IsUnix)}: {IsUnix}{Environment.NewLine}" +
                $"{nameof(WindowingSubsystemName)}: {WindowingSubsystemName}{Environment.NewLine}" +
                $"{nameof(RenderingSubsystemName)}: {RenderingSubsystemName}{Environment.NewLine}";
    }
}
