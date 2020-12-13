using System;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class TemplateContainerViewModel : BaseContainerViewModel, IGrid
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;
        [AutoNotify] private BaseColorViewModel _background;
        [AutoNotify] private bool _isGridEnabled;
        [AutoNotify] private bool _isBorderEnabled;
        [AutoNotify] private double _gridOffsetLeft;
        [AutoNotify] private double _gridOffsetTop;
        [AutoNotify] private double _gridOffsetRight;
        [AutoNotify] private double _gridOffsetBottom;
        [AutoNotify] private double _gridCellWidth;
        [AutoNotify] private double _gridCellHeight;
        [AutoNotify] private BaseColorViewModel _gridStrokeColor;
        [AutoNotify] private double _gridStrokeThickness;

        public TemplateContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_background != null)
            {
                isDirty |= _background.IsDirty();
            }

            if (_gridStrokeColor != null)
            {
                isDirty |= _gridStrokeColor.IsDirty();
            }

            return isDirty;
        }
    }
}
