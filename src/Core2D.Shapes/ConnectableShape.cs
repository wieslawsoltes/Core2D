// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Renderer;
using Core2D.Shapes.Interfaces;

namespace Core2D.Shapes
{
    /// <summary>
    /// Connectable shape.
    /// </summary>
    public abstract class ConnectableShape : BaseShape, IConnectableShape
    {
        private ImmutableArray<IPointShape> _connectors;

        /// <summary>
        /// Gets or sets connectors collection.
        /// </summary>
        public ImmutableArray<IPointShape> Connectors
        {
            get => _connectors;
            set => Update(ref _connectors, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectableShape"/> class.
        /// </summary>
        public ConnectableShape()
            : base()
        {
            _connectors = ImmutableArray.Create<IPointShape>();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = Data?.Record ?? r;

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    foreach (var connector in Connectors)
                    {
                        connector.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
                else
                {
                    foreach (var connector in Connectors)
                    {
                        if (connector == renderer.State.SelectedShape)
                        {
                            connector.Draw(dc, renderer, dx, dy, db, record);
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
                        connector.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            foreach (var connector in Connectors)
            {
                connector.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<IBaseShape> selected)
        {
            base.Select(selected);

            foreach (var connector in Connectors)
            {
                connector.Select(selected);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IBaseShape> selected)
        {
            base.Deselect(selected);

            foreach (var connector in Connectors)
            {
                connector.Deselect(selected);
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
        public virtual bool ShouldSerializeConnectors() => _connectors.IsEmpty == false;
    }
}
