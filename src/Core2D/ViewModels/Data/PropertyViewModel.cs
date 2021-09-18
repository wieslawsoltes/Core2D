#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Data
{
    public partial class PropertyViewModel : ViewModelBase
    {
        [AutoNotify] private string? _value;

        public PropertyViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            AddProperty = new Command<ViewModelBase?>(x => GetProject()?.OnAddProperty(x));
            
            RemoveProperty = new Command<PropertyViewModel?>(x => GetProject()?.OnRemoveProperty(x));

            ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        }

        [IgnoreDataMember]
        public ICommand AddProperty { get; }

        [IgnoreDataMember]
        public ICommand RemoveProperty { get; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new PropertyViewModel(ServiceProvider)
            {
                Name = Name,
                Value = Value
            };

            return copy;
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override string ToString() => _value ?? "";
    }
}
