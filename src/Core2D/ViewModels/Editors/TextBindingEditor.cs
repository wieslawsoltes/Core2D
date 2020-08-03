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
        private IProjectEditor _editor;
        private ITextShape _text;

        /// <summary>
        /// Gets or sets project editor.
        /// </summary>
        public IProjectEditor Editor
        {
            get => _editor;
            set => Update(ref _editor, value);
        }

        /// <summary>
        /// Gets or sets text shape.
        /// </summary>
        public ITextShape Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use column name.
        /// </summary>
        public void OnUseColumnName(IColumn column)
        {
            if (_text != null && column != null)
            {
                _text.Text += $"{{{column.Name}}}";
            }
        }

        /// <summary>
        /// Use page property.
        /// </summary>
        public void OnUsePageProperty(IProperty property)
        {
            if (_text != null && property != null)
            {
                _text.Text += $"{{{property.Name}}}";
            }
        }

        /// <summary>
        /// Use shape property.
        /// </summary>
        public void OnUseShapeProperty(IProperty property)
        {
            if (_text != null && property != null)
            {
                _text.Text += $"{{{property.Name}}}";
            }
        }
    }
}
