using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Connectable shape.
    /// </summary>
    public abstract class ConnectableShape : BaseShape, IConnectableShape
    {
        private ImmutableArray<IPointShape> _connectors;

        /// <inheritdoc/>
        public ImmutableArray<IPointShape> Connectors
        {
            get => _connectors;
            set => Update(ref _connectors, value);
        }

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (renderer.State.SelectedShapes != null && renderer.State.DrawPoints == true)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    foreach (var connector in _connectors)
                    {
                        connector.DrawShape(dc, renderer, dx, dy);
                    }
                }
                else
                {
                    foreach (var connector in _connectors)
                    {
                        if (renderer.State.SelectedShapes.Contains(connector))
                        {
                            connector.DrawShape(dc, renderer, dx, dy);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            foreach (var connector in _connectors)
            {
                connector.Bind(dataFlow, db, record);
            }
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            foreach (var connector in _connectors)
            {
                connector.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);

            foreach (var connector in _connectors)
            {
                connector.Select(selection);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            foreach (var connector in _connectors)
            {
                connector.Deselect(selection);
            }
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<IPointShape> points)
        {
            foreach (var connector in _connectors)
            {
                points.Add(connector);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Connectors"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeConnectors() => true;
    }
}
