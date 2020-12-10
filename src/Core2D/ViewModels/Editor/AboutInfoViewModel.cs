using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Editor
{
    public class AboutInfoViewModel : ViewModelBase
    {
        public string Title { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string Copyright { get; set; }

        public string License { get; set; }

        public string OperatingSystem { get; set; }

        public bool IsDesktop { get; set; }

        public bool IsMobile { get; set; }

        public bool IsCoreClr { get; set; }

        public bool IsMono { get; set; }

        public bool IsDotNetFramework { get; set; }

        public bool IsUnix { get; set; }

        public string WindowingSubsystemName { get; set; }

        public string RenderingSubsystemName { get; set; }

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
