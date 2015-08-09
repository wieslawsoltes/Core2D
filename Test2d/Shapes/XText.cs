// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XText : BaseShape
    {
        private XPoint _topLeft;
        private XPoint _bottomRight;
        private string _text;

        /// <summary>
        /// 
        /// </summary>
        public XPoint TopLeft
        {
            get { return _topLeft; }
            set { Update(ref _topLeft, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set { Update(ref _bottomRight, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { Update(ref _text, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Record;
            _topLeft.TryToBind("TopLeft", this.Bindings, record);
            _bottomRight.TryToBind("BottomRight", this.Bindings, record);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (State.HasFlag(ShapeState.Visible))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public override void Move(double dx, double dy)
        {
            if (!TopLeft.State.HasFlag(ShapeState.Connector))
            {
                TopLeft.Move(dx, dy);
            }

            if (!BottomRight.State.HasFlag(ShapeState.Connector))
            {
                BottomRight.Move(dx, dy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="r"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryToBind(
            ImmutableArray<ShapeBinding> bindings, 
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
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="db"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryToBind(
            ImmutableArray<ShapeBinding> bindings, 
            ImmutableArray<ShapeProperty> db,
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
                    value =  result.Value.ToString();
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static object[] ToArgs(ImmutableArray<ShapeProperty> properties)
        {
            return properties.Where(x => x != null).Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public string BindToTextProperty(ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            // try to bind to internal (this.Record) or external (r) data record using Bindings
            if (record != null 
                && this.Bindings != null 
                && this.Bindings.Length > 0)
            {
                string value;
                bool success = TryToBind(this.Bindings, record, "Text", out value);
                if (success)
                {
                    return value;
                }
            }

            // try to bind to external properties database using Bindings
            if (db != null 
                && this.Bindings != null 
                && this.Bindings.Length > 0)
            {
                string value;
                bool success = TryToBind(this.Bindings, db, "Text", out value);
                if (success)
                {
                    return value;
                }
            }

            // try to bind to Properties using Text as formatting
            if (this.Properties != null 
                && this.Properties.Length > 0)
            {
                try
                {
                    var args = ToArgs(this.Properties);
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
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                Text = text
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="text"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };
        }
    }
}
