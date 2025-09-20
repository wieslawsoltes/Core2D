// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Shapes;

public abstract partial class BaseShapeViewModel : ViewModelBase, IDataObject, ISelectable, IConnectable, IDrawable
{
    private readonly IDictionary<string, object?> _propertyCache = new Dictionary<string, object?>();

    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBePrivate.Global
    [AutoNotify] protected ShapeStateFlags _state;
    [AutoNotify] protected ShapeStyleViewModel? _style;
    [AutoNotify] protected bool _isStroked;
    [AutoNotify] protected bool _isFilled;
    [AutoNotify] protected ImmutableArray<PropertyViewModel> _properties;
    [AutoNotify] protected RecordViewModel? _record;
    [AutoNotify(SetterModifier = AccessModifier.None)] protected readonly Type _targetType;
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore InconsistentNaming

    protected BaseShapeViewModel(IServiceProvider? serviceProvider, Type targetType) : base(serviceProvider)
    {
        _targetType = targetType;

        AddProperty = new RelayCommand<ViewModelBase?>(x => GetProject()?.OnAddProperty(x));
            
        RemoveProperty = new RelayCommand<PropertyViewModel?>(x => GetProject()?.OnRemoveProperty(x));

        ResetRecord = new RelayCommand<IDataObject?>(x => GetProject()?.OnResetRecord(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand AddProperty { get; }

    [IgnoreDataMember]
    public ICommand RemoveProperty { get; }

    [IgnoreDataMember]
    public ICommand ResetRecord { get; }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        foreach (var property in _properties)
        {
            isDirty |= property.IsDirty();
        }

        if (Record is { })
        {
            isDirty |= Record.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        foreach (var property in _properties)
        {
            property.Invalidate();
        }

        Record?.Invalidate();
    }

    public virtual void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        throw new NotImplementedException();
    }

    public virtual void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        throw new NotImplementedException();
    }

    public virtual bool Invalidate(IShapeRenderer? renderer)
    {
        return false;
    }

    public virtual void Bind(DataFlow dataFlow, object? db, object? r)
    {
        throw new NotImplementedException();
    }

    public virtual void SetProperty(string name, object? value)
    {
        _propertyCache[name] = value;
    }

    public virtual object? GetProperty(string name)
    {
        if (_propertyCache.ContainsKey(name))
        {
            return _propertyCache[name];
        }
        return null;
    }

    public virtual void Move(ISelection? selection, decimal dx, decimal dy)
    {
        throw new NotImplementedException();
    }

    public virtual void Select(ISelection? selection)
    {
        if (selection?.SelectedShapes is null)
        {
            return;
        }

        if (!selection.SelectedShapes.Contains(this))
        {
            selection.SelectedShapes.Add(this);
        }
    }

    public virtual void Deselect(ISelection? selection)
    {
        if (selection?.SelectedShapes is null)
        {
            return;
        }

        if (selection.SelectedShapes.Contains(this))
        {
            selection.SelectedShapes.Remove(this);
        }
    }

    public virtual bool Connect(PointShapeViewModel? point, PointShapeViewModel? target)
    {
        throw new NotImplementedException();
    }

    public virtual bool Disconnect(PointShapeViewModel? point, out PointShapeViewModel? result)
    {
        throw new NotImplementedException();
    }

    public virtual bool Disconnect()
    {
        throw new NotImplementedException();
    }

    public virtual void GetPoints(IList<PointShapeViewModel> points)
    {
        throw new NotImplementedException();
    }

    public void ToggleDefaultShapeState()
    {
        State ^= ShapeStateFlags.Default;
    }

    public void ToggleVisibleShapeState()
    {
        State ^= ShapeStateFlags.Visible;
    }

    public void TogglePrintableShapeState()
    {
        State ^= ShapeStateFlags.Printable;
    }

    public void ToggleLockedShapeState()
    {
        State ^= ShapeStateFlags.Locked;
    }

    public void ToggleSizeShapeState()
    {
        State ^= ShapeStateFlags.Size;
    }

    public void ToggleThicknessShapeState()
    {
        State ^= ShapeStateFlags.Thickness;
    }

    public void ToggleConnectorShapeState()
    {
        State ^= ShapeStateFlags.Connector;
    }

    public void ToggleNoneShapeState()
    {
        State ^= ShapeStateFlags.None;
    }

    public void ToggleStandaloneShapeState()
    {
        State ^= ShapeStateFlags.Standalone;
    }

    public void ToggleInputShapeState()
    {
        State ^= ShapeStateFlags.Input;
    }

    public void ToggleOutputShapeState()
    {
        State ^= ShapeStateFlags.Output;
    }
}
