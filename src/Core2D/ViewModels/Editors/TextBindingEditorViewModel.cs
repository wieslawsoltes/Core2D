using System;
using System.Collections.Generic;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editors
{
    public class TextBindingEditorViewModel : ViewModelBase
    {
        private ProjectEditorViewModel _editor;
        private TextShapeViewModel _text;

        public ProjectEditorViewModel Editor
        {
            get => _editor;
            set => RaiseAndSetIfChanged(ref _editor, value);
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

        public void OnUseColumnName(ColumnViewModel column)
        {
            if (_text != null && column != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{column.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{column.Name}}}");
                }
            }
        }

        public void OnUsePageProperty(PropertyViewModel property)
        {
            if (_text != null && property != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{property.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{property.Name}}}");
                }
            }
        }

        public void OnUseShapeProperty(PropertyViewModel property)
        {
            if (_text != null && property != null)
            {
                if (string.IsNullOrEmpty(_text.Text))
                {
                    _text.Text = $"{{{property.Name}}}";
                }
                else
                {
                    var startIndex = _text.Text.Length;
                    _text.Text = _text.Text.Insert(startIndex, $"{{{property.Name}}}");
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
