// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Object representing text string shape.
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
        public override void Bind(Record r)
        {
            var record = r ?? this.Data.Record;
            _topLeft.TryToBind("TopLeft", this.Data.Bindings, record);
            _bottomRight.TryToBind("BottomRight", this.Data.Bindings, record);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

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

        /// <summary>
        /// Try binding data record to one of <see cref="XText"/> shape properties.
        /// </summary>
        /// <param name="bindings">The bindings database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="value">The output string bound to data record.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(
            ImmutableArray<Binding> bindings,
            Record r,
            string propertyName,
            out string value)
        {
            if (r == null || bindings == null || bindings.Length <= 0)
            {
                value = null;
                return false;
            }

            if (r.Columns == null || r.Values == null || r.Columns.Length != r.Values.Length)
            {
                value = null;
                return false;
            }

            var columns = r.Columns;
            foreach (var binding in bindings)
            {
                if (string.IsNullOrEmpty(binding.Property) || string.IsNullOrEmpty(binding.Path))
                    continue;

                if (binding.Property != propertyName)
                    continue;

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Name == binding.Path)
                    {
                        value = r.Values[i].Content;
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try binding properties array to one of <see cref="XText"/> shape properties.
        /// </summary>
        /// <param name="bindings">The bindings database used for binding.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="value">The string bound to properties.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(
            ImmutableArray<Binding> bindings,
            ImmutableArray<Property> db,
            string propertyName,
            out string value)
        {
            foreach (var binding in bindings)
            {
                if (string.IsNullOrEmpty(binding.Property) || string.IsNullOrEmpty(binding.Path))
                    continue;

                if (binding.Property != propertyName)
                    continue;

                var result = db.FirstOrDefault(p => p.Name == binding.Path);
                if (result != null && result.Value != null)
                {
                    value = result.Value.ToString();
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Convert shape <see cref="Property"/>'s array to arguments array.
        /// </summary>
        /// <param name="properties">The properties array.</param>
        /// <returns>The object arguments array.</returns>
        private static object[] ToArgs(ImmutableArray<Property> properties)
        {
            return properties.Where(x => x != null).Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// Bind properties or data record to <see cref="XText.Text"/> property.
        /// </summary>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <returns>The string bound to properties or data record.</returns>
        public string BindToTextProperty(ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

            // Try to bind to internal (this.Data.Record) or external (r) data record using Bindings.
            if (record != null
                && this.Data.Bindings != null
                && this.Data.Bindings.Length > 0)
            {
                string value;
                bool success = TryToBind(this.Data.Bindings, record, "Text", out value);
                if (success)
                {
                    return value;
                }
            }

            // Try to bind to external properties database using Bindings.
            if (db != null
                && this.Data.Bindings != null
                && this.Data.Bindings.Length > 0)
            {
                string value;
                bool success = TryToBind(this.Data.Bindings, db, "Text", out value);
                if (success)
                {
                    return value;
                }
            }

            // Try to bind to Properties using Text as formatting.
            if (this.Data.Properties != null
                && this.Data.Properties.Length > 0)
            {
                try
                {
                    var args = ToArgs(this.Data.Properties);
                    if (this.Text != null && args != null && args.Length > 0)
                    {
                        return string.Format(this.Text, args);
                    }
                }
                catch (FormatException) { }
            }

            return this.Text;
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
        public static XText Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isStroked = true,
            string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
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
        public static XText Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isStroked = true,
            string name = "")
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
        public static XText Create(
            XPoint topLeft,
            XPoint bottomRight,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isStroked = true,
            string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };
        }
    }
}
