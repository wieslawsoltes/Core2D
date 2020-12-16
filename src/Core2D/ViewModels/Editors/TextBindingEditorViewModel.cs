using System;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editors
{
    public partial class TextBindingEditorViewModel : ViewModelBase
    {
        [AutoNotify] private ProjectEditorViewModel _editor;
        [AutoNotify] private TextShapeViewModel _text;

        public TextBindingEditorViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void OnUseColumnName(ColumnViewModel column)
        {
            if (_text is { } && column is { })
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
            if (_text is { } && property is { })
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
            if (_text is { } && property is { })
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
            if (_text is { })
            {
                _text.Text = "";
            }
        }
    }
}
