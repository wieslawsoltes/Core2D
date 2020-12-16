using System.Collections.Generic;
using System.Linq;

namespace Core2D.ViewModels.Shapes
{
    public static class BaseShapeViewModelExtensions
    {
        public static IEnumerable<BaseShapeViewModel> GetAllShapes(this IEnumerable<BaseShapeViewModel> shapes)
        {
            if (shapes == null)
            {
                yield break;
            }

            foreach (var shape in shapes)
            {
                if (shape is GroupShapeViewModel groupShape)
                {
                    foreach (var s in GetAllShapes(groupShape.Shapes))
                    {
                        yield return s;
                    }

                    yield return shape;
                }
                else
                {
                    yield return shape;
                }
            }
        }

        public static IEnumerable<T> GetAllShapes<T>(this IEnumerable<BaseShapeViewModel> shapes)
        {
            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }
    }
}
