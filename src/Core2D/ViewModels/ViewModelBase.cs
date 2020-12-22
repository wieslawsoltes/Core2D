#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core2D.ViewModels
{
    public partial class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isDirty;
        protected readonly IServiceProvider _serviceProvider;
        [AutoNotify] private ViewModelBase _owner;
        [AutoNotify] private string _name = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual bool IsDirty() => _isDirty;

        public virtual void Invalidate() => _isDirty = false;

        public virtual void MarkAsDirty() => _isDirty = true;

        public virtual object Copy(IDictionary<object, object> shared) =>  throw new NotImplementedException();

        public void RaisePropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void RaiseAndSetIfChanged<T>(ref T field, T value, PropertyChangedEventArgs e)
        {
            if (!Equals(field, value))
            {
                field = value;
                _isDirty = true;
                PropertyChanged?.Invoke(this, e);
            }
        }
    }
}
