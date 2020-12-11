using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class ConnectableShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private ImmutableArray<PointShapeViewModel> _connectors;

        protected ConnectableShapeViewModel(IServiceProvider serviceProvider, Type targetType) : base(serviceProvider, targetType)
        {
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    foreach (var connector in _connectors)
                    {
                        connector.DrawShape(dc, renderer);
                    }
                }
                else
                {
                    foreach (var connector in _connectors)
                    {
                        if (renderer.State.SelectedShapes.Contains(connector))
                        {
                            connector.DrawShape(dc, renderer);
                        }
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            foreach (var connector in _connectors)
            {
                connector.Bind(dataFlow, db, record);
            }
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            foreach (var connector in _connectors)
            {
                connector.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);

            foreach (var connector in _connectors)
            {
                connector.Select(selection);
            }
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            foreach (var connector in _connectors)
            {
                connector.Deselect(selection);
            }
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            foreach (var connector in _connectors)
            {
                points.Add(connector);
            }
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var connector in _connectors)
            {
                isDirty |= connector.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var connector in _connectors)
            {
                connector.Invalidate();
            }
        }
    }
}
