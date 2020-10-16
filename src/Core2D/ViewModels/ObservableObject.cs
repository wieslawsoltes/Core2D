using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core2D
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private bool _isDirty;
        private ObservableObject _owner = null;
        private string _name = "";

        public virtual ObservableObject Owner
        {
            get => _owner;
            set => RaiseAndSetIfChanged(ref _owner, value);
        }

        public virtual string Name
        {
            get => _name;
            set => RaiseAndSetIfChanged(ref _name, value);
        }

        public virtual bool IsDirty()
        {
            return _isDirty;
        }

        public virtual void Invalidate()
        {
            _isDirty = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract object Copy(IDictionary<object, object> shared);

        public void RaisePropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = default)
        {
            if (!Equals(field, value))
            {
                field = value;
                _isDirty = true;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        public virtual bool ShouldSerializeOwner() => _owner != null;

        public virtual bool ShouldSerializeName() => !string.IsNullOrWhiteSpace(_name);

        public virtual bool ShouldSerializeIsDirty() => false;
    }
}
