using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Renderer
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
        [AutoNotify] private ShapeStyleViewModel _pointStyle;
        [AutoNotify] private ShapeStyleViewModel _selectedPointStyle;
        [AutoNotify] private double _pointSize;
        [AutoNotify] private ShapeStyleViewModel _selectionStyle;
        [AutoNotify] private ShapeStyleViewModel _helperStyle;
        [AutoNotify] private IDecorator _decorator;

        public ShapeRendererStateViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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
