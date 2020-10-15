using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Core2D.Editor;
using Core2D.Views;

namespace Core2D.Editor
{
    /// <summary>
    /// File image importer.
    /// </summary>
    public class AvaloniaImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaImageImporter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private MainWindow GetWindow()
        {
            return _serviceProvider.GetService<MainWindow>();
        }

        /// <inheritdoc/>
        public async Task<string> GetImageKeyAsync()
        {
            try
            {
                var dlg = new OpenFileDialog() { Title = "Open" };
                dlg.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
                var result = await dlg.ShowAsync(GetWindow());
                if (result != null)
                {
                    var path = result.FirstOrDefault();
                    if (path != null)
                    {
                        return _serviceProvider.GetService<ProjectEditor>().OnGetImageKey(path);
                    }
                }
            }
            catch (Exception ex)
            {
                _serviceProvider.GetService<ILog>()?.LogException(ex);
            }

            return default;
        }
    }
}
