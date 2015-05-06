// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    public class XText : BaseShape
    {
        private XPoint _topLeft;
        private XPoint _bottomRight;
        private bool _isFilled;
        private string _text;
        private string _textBinding;

        public XPoint TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (value != _topLeft)
                {
                    _topLeft = value;
                    Notify("TopLeft");
                }
            }
        }

        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set
            {
                if (value != _bottomRight)
                {
                    _bottomRight = value;
                    Notify("BottomRight");
                }
            }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
        }
        
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    Notify("Text");
                }
            }
        }

        public string TextBinding
        {
            get { return _textBinding; }
            set
            {
                if (value != _textBinding)
                {
                    _textBinding = value;
                    Notify("TextBinding");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }

            if (renderer.SelectedShape != null)
            {
                if (this == renderer.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
                else if (_topLeft == renderer.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                }
                else if (_bottomRight == renderer.SelectedShape)
                {
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
            }
        }

        public override void Move(double dx, double dy)
        {
            TopLeft.Move(dx, dy);
            BottomRight.Move(dx, dy);
        }

        public string Bind(IList<KeyValuePair<string, ShapeProperty>> db)
        {
            if (db != null && !string.IsNullOrEmpty(this.TextBinding))
            {
                // try to bind to database using TextBinding key
                var result = db.Where(kvp => kvp.Key == this.TextBinding).FirstOrDefault();
                if (result.Value != null)
                {
                    return result.Value.Data.ToString();
                }
            }

            if (this.Properties != null && this.Properties.Count > 0)
            {
                try
                {
                    // try to bind to Properties using Text as formatting
                    return string.Format(this.Text, this.Properties.Select(x => x.Data).ToArray());
                }
                catch (FormatException) { }
            }
            return this.Text;
        }

        public static XText Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isFilled = false,
            string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                Properties = new ObservableCollection<ShapeProperty>(),
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                IsFilled = isFilled,
                Text = text
            };
        }

        public static XText Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isFilled = false,
            string name = "")
        {
            return Create(x, y, x, y, style, point, text, isFilled, name);
        }
    }
}
