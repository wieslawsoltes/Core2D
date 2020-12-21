#nullable disable
using System;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentFileViewModel : ViewModelBase
    {
        [AutoNotify] private string _path;

        public RecentFileViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public static RecentFileViewModel Create(IServiceProvider serviceProvider, string name, string path)
        {
            return new RecentFileViewModel(serviceProvider)
            {
                Name = name,
                Path = path
            };
        }
    }
}
