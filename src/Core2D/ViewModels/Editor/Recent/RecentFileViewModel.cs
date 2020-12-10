using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentFileViewModel : ViewModelBase
    {
        [AutoNotify] private string _path;

        public static RecentFileViewModel Create(string name, string path)
        {
            return new RecentFileViewModel()
            {
                Name = name,
                Path = path
            };
        }
    }
}
