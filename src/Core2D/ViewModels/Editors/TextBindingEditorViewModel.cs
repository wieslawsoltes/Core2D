using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;

namespace Core2D.Editors
{
    public class TextBindingEditorViewModel : ViewModelBase
    {
        private ProjectEditorViewModel _editorViewModel;
        private TextShapeViewModel _text;

        public ProjectEditorViewModel EditorViewModel
        {
            get => _editorViewModel;
            set => RaiseAndSetIfChanged(ref _editorViewModel, value);
        }

        public TextShapeViewModel Text
        {
            get => _text;
            set => RaiseAndSetIfChanged(ref _text, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void OnUseColumnName(ColumnViewModel columnViewModel)
        {
            if (_text != null && columnViewModel != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{columnViewModel.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{columnViewModel.Name}}}");
                }
            }
        }

        public void OnUsePageProperty(PropertyViewModel propertyViewModel)
        {
            if (_text != null && propertyViewModel != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{propertyViewModel.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{propertyViewModel.Name}}}");
                }
            }
        }

        public void OnUseShapeProperty(PropertyViewModel propertyViewModel)
        {
            if (_text != null && propertyViewModel != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{propertyViewModel.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{propertyViewModel.Name}}}");
                }
            }
        }

        public void OnResetText()
        {
            if (_text != null)
            {
                _text.Text = "";
            }
        }
    }
}
