#nullable disable
using System.Collections.Generic;
using System.Linq;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Containers
{
    public static class ProjectContainerViewModelExtensions
    {
        public static IEnumerable<T> GetAllShapes<T>(this ProjectContainerViewModel project)
        {
            var shapes = project?.Documents
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return shapes.GetAllShapes()?.Where(s => s is T).Cast<T>();
        }
        
        public static IEnumerable<BaseShapeViewModel> GetAllShapes(this ProjectContainerViewModel project)
        {
            return project?.Documents
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);
        }
    }
}
