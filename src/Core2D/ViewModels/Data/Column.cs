using System.Collections.Generic;

namespace Core2D.Data
{
    public class Column : ObservableObject
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set => RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new Column()
            {
                Name = this.Name,
                IsVisible = this.IsVisible
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

        public virtual bool ShouldSerializeIsVisible() => _isVisible != default;
    }
}
