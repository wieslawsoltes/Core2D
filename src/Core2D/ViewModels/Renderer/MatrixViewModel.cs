#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Renderer;

public partial class MatrixViewModel : ViewModelBase
{
    [AutoNotify] private double _m11;
    [AutoNotify] private double _m12;
    [AutoNotify] private double _m13;
    [AutoNotify] private double _m21;
    [AutoNotify] private double _m22;
    [AutoNotify] private double _m23;
    [AutoNotify] private double _m31;
    [AutoNotify] private double _m32;
    [AutoNotify] private double _m33;

    public MatrixViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new MatrixViewModel(ServiceProvider)
        {
            Name = Name,
            M11 = _m11,
            M12 = _m12,
            M13 = _m13,
            M21 = _m21,
            M22 = _m22,
            M23 = _m23,
            M31 = _m31,
            M32 = _m32,
            M33 = _m33,
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();
        return isDirty;
    }
}
