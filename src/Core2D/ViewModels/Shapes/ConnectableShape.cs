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
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    foreach (var connector in Connectors)
                    {
                        connector.Draw(dc, renderer, dx, dy);
                    }
                }
                else
                {
                    foreach (var connector in Connectors)
                    {
                        if (connector == renderer.State.SelectedShape)
                        {
                            connector.Draw(dc, renderer, dx, dy);
                        }
                    }
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    foreach (var connector in Connectors)
                    {
                        connector.Draw(dc, renderer, dx, dy);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            foreach (var connector in Connectors)
            {
                connector.Bind(dataFlow, db, record);
            }
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            foreach (var connector in Connectors)
            {
                connector.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);

            foreach (var connector in Connectors)
            {
                connector.Select(selection);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            foreach (var connector in Connectors)
            {
                connector.Deselect(selection);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            return Connectors;
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
