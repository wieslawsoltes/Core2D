using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Common.UnitTests
{
    public abstract class TestBaseShape : BaseShape
    {
        public override Type TargetType => typeof(TestBaseShape);
    }
}
