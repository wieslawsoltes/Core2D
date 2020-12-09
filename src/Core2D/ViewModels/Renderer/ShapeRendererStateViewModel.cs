using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public partial class ShapeRendererStateViewModel : ViewModelBase
    {
        [AutoNotify] private double _panX;
        [AutoNotify] private double _panY;
        [AutoNotify] private double _zoomX;
        [AutoNotify] private double _zoomY;
        [AutoNotify] private ShapeStateFlags _drawShapeState;
        [AutoNotify] private ISet<BaseShapeViewModel> _selectedShapes;
        [AutoNotify] private IImageCache _imageCache;
        [AutoNotify] private bool _drawDecorators;
        [AutoNotify] private bool _drawPoints;
        [AutoNotify] private ShapeStyleViewModel _pointStyleViewModel;
        [AutoNotify] private ShapeStyleViewModel _selectedPointStyleViewModel;
        [AutoNotify] private double _pointSize;
        [AutoNotify] private ShapeStyleViewModel _selectionStyleViewModel;
        [AutoNotify] private ShapeStyleViewModel _helperStyleViewModel;
        [AutoNotify] private IDecorator _decorator;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }
    }
}
