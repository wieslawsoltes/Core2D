using System;
using System.Collections.Generic;

namespace Core2D.Renderer
{
    public partial class ShapeState : ViewModelBase
    {
        // TODO: AutoNotify
        [AutoNotify] private ShapeStateFlags _flags;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeState()
            {
                Flags = this._flags
            };
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

        public static ShapeState Parse(string s)
        {
            var flags = (ShapeStateFlags)Enum.Parse(typeof(ShapeStateFlags), s, true);
            return new ShapeState()
            {
                Flags = flags
            };
        }

        public override string ToString()
        {
            return _flags.ToString();
        }
    }
}
