// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Text shape.
    /// </summary>
    public class XText : BaseShape
    {
        private XPoint _topLeft;
        private XPoint _bottomRight;
        private string _text;

        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        public XPoint TopLeft
        {
            get { return _topLeft; }
            set { Update(ref _topLeft, value); }
        }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set { Update(ref _bottomRight, value); }
        }

        /// <summary>
        /// Gets or sets text string.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { Update(ref _text, value); }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var state = base.BeginTransform(dc, renderer);

            var record = this.Data.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_topLeft == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_bottomRight == renderer.State.SelectedShape)
                {
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            if (!TopLeft.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                TopLeft.Move(dx, dy);
            }

            if (!BottomRight.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                BottomRight.Move(dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return TopLeft;
            yield return BottomRight;
        }

        /// <summary>
        /// Try binding data record to <see cref="XText.Text"/> shape property containing column name.
        /// </summary>
        /// <param name="r">The external data record used for binding.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The output string bound to data record.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(XRecord r, string columnName, out string value)
        {
            if (string.IsNullOrEmpty(columnName) || r == null)
            {
                value = null;
                return false;
            }

            var columns = r.Columns;
            var values = r.Values;
            if (columns == null || values == null || columns.Length != values.Length)
            {
                value = null;
                return false;
            }

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Name == columnName)
                {
                    value = values[i].Content;
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try binding properties array to one of <see cref="XText"/> shape properties.
        /// </summary>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="value">The string bound to properties.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(ImmutableArray<XProperty> db, string propertyName, out string value)
        {
            if (string.IsNullOrEmpty(propertyName) || db == null)
            {
                value = null;
                return false;
            }

            var result = db.FirstOrDefault(p => p.Name == propertyName);
            if (result != null && result.Value != null)
            {
                value = result.Value.ToString();
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Bind properties or data record to <see cref="XText.Text"/> property.
        /// </summary>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <returns>The string bound to properties or data record.</returns>
        public string BindText(ImmutableArray<XProperty> db, XRecord r)
        {
            var record = this.Data.Record ?? r;

            if (!string.IsNullOrEmpty(_text))
            {
                var text = _text.Trim();
                if (text.Length >= 3 && text.TrimStart().StartsWith("{") && text.TrimEnd().EndsWith("}"))
                {
                    var bidning = text.Substring(1, text.Length - 2);

                    // Try to bind to internal Data.Record or external (r) data record using Text property as Column.Name name.
                    if (record != null)
                    {
                        string value;
                        bool success = TryToBind(record, bidning, out value);
                        if (success)
                        {
                            return value;
                        }
                    }

                    // Try to bind to external Properties database (e.g. Container.Data.Properties) using Text property as Property.Name name.
                    if (db != null)
                    {
                        string value;
                        bool success = TryToBind(db, bidning, out value);
                        if (success)
                        {
                            return value;
                        }
                    }
                }
            }

            // Try to bind to Properties using Text as formatting.
            if (this.Data.Properties != null && this.Data.Properties.Length > 0)
            {
                try
                {
                    var args = this.Data.Properties.Where(x => x != null).Select(x => x.Value).ToArray();
                    if (_text != null && args != null && args.Length > 0)
                    {
                        return string.Format(_text, args);
                    }
                }
                catch (FormatException) { }
            }

            return _text;
        }

        /// <summary>
        /// Creates a new <see cref="XText"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XText"/> class.</returns>
        public static XText Create(double x1, double y1, double x2, double y2, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="XText"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="XText.TopLeft"/> and <see cref="XText.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="XText.TopLeft"/> and <see cref="XText.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XText"/> class.</returns>
        public static XText Create(double x, double y, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return Create(x, y, x, y, style, point, text, isStroked, name);
        }

        /// <summary>
        /// Creates a new <see cref="XText"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XText"/> class.</returns>
        public static XText Create(XPoint topLeft, XPoint bottomRight, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };
        }
    }
}
