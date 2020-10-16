using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Shapes;

namespace Core2D.Editors
{
    /// <summary>
    /// Text binding editor.
    /// </summary>
    public class TextBindingEditor : ObservableObject
    {
        private ProjectEditor _editor;
        private TextShape _text;

        /// <summary>
        /// Gets or sets project editor.
        /// </summary>
        public ProjectEditor Editor
        {
            get => _editor;
            set => RaiseAndSetIfChanged(ref _editor, value);
        }

        /// <summary>
        /// Gets or sets text shape.
        /// </summary>
        public TextShape Text
        {
            get => _text;
            set => RaiseAndSetIfChanged(ref _text, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use column name.
        /// </summary>
        public void OnUseColumnName(Column column)
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

        /// <summary>
        /// Use page property.
        /// </summary>
        public void OnUsePageProperty(Property property)
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

        /// <summary>
        /// Use shape property.
        /// </summary>
        public void OnUseShapeProperty(Property property)
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

        /// <summary>
        /// Reset text.
        /// </summary>
        public void OnResetText()
        {
            if (_text != null)
            {
                _text.Text = "";
            }
        }
    }
}
