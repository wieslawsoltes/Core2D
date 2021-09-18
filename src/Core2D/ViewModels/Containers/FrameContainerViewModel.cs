#nullable enable
using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Windows.Input;
using Core2D.Model;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Containers
{
    public abstract partial class FrameContainerViewModel : BaseContainerViewModel, IDataObject
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        [AutoNotify] protected ImmutableArray<LayerContainerViewModel> _layers;
        [AutoNotify] protected LayerContainerViewModel? _currentLayer;
        [AutoNotify] protected LayerContainerViewModel? _workingLayer;
        [AutoNotify] protected LayerContainerViewModel? _helperLayer;
        [AutoNotify] protected BaseShapeViewModel? _currentShape;
        [AutoNotify] protected ImmutableArray<PropertyViewModel> _properties;
        [AutoNotify] protected RecordViewModel? _record;
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming

        protected FrameContainerViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            AddProperty = new Command<ViewModelBase?>(x => GetProject()?.OnAddProperty(x));
            
            RemoveProperty = new Command<PropertyViewModel?>(x => GetProject()?.OnRemoveProperty(x));

            ResetRecord = new Command<IDataObject?>(x => GetProject()?.OnResetRecord(x));

            ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        }

        [IgnoreDataMember]
        public ICommand AddProperty { get; }

        [IgnoreDataMember]
        public ICommand RemoveProperty { get; }

        [IgnoreDataMember]
        public ICommand ResetRecord { get; }

        public void SetCurrentLayer(LayerContainerViewModel? layer) => CurrentLayer = layer;

        public virtual void InvalidateLayer()
        {
            foreach (var layer in _layers)
            {
                layer.RaiseInvalidateLayer();
            }

            _workingLayer?.RaiseInvalidateLayer();

            _helperLayer?.RaiseInvalidateLayer();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var layer in _layers)
            {
                isDirty |= layer.IsDirty();
            }

            if (_workingLayer is { })
            {
                isDirty |= _workingLayer.IsDirty();
            }

            if (_helperLayer is { })
            {
                isDirty |= _helperLayer.IsDirty();
            }

            foreach (var property in _properties)
            {
                isDirty |= property.IsDirty();
            }

            if (_record is { })
            {
                isDirty |= _record.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var layer in _layers)
            {
                layer.Invalidate();
            }

            _workingLayer?.Invalidate();
            _helperLayer?.Invalidate();

            foreach (var property in _properties)
            {
                property.Invalidate();
            }

            _record?.Invalidate();
        }
    }
}
