// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Shapes.Interfaces;

namespace Core2D.Shapes
{
    /// <summary>
    /// Text shape extension methods.
    /// </summary>
    public static class TextShapeExtensions
    {
        /// <summary>
        /// Try binding data record to <see cref="ITextShape.Text"/> shape property containing column name.
        /// </summary>
        /// <param name="r">The external data record used for binding.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The output string bound to data record.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(Record r, string columnName, out string value)
        {
            if (string.IsNullOrEmpty(columnName) || r == null)
            {
                value = null;
                return false;
            }

            var columns = r.Owner.Columns;
            var values = r.Values;
            if (columns == null || values == null || columns.Length != values.Length)
            {
                value = null;
                return false;
            }

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Name == columnName)
                {
                    value = values[i].Content;
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try binding properties array to one of <see cref="TextShape"/> shape properties.
        /// </summary>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="value">The string bound to properties.</param>
        /// <returns>True if binding was successful.</returns>
        private static bool TryToBind(ImmutableArray<Property> db, string propertyName, out string value)
        {
            if (string.IsNullOrEmpty(propertyName) || db == null)
            {
                value = null;
                return false;
            }

            var result = db.FirstOrDefault(p => p.Name == propertyName);
            if (result != null && result.Value != null)
            {
                value = result.Value.ToString();
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Check if text is a property binding.
        /// </summary>
        /// <param name="text">The text to check for binding.</param>
        /// <param name="binding">The binding property name.</param>
        /// <returns>True if text is a binding.</returns>
        public static bool IsBinding(string text, out string binding)
        {
            var trimmed = text.Trim();
            if (trimmed.Length >= 3 && trimmed.TrimStart().StartsWith("{") && trimmed.TrimEnd().EndsWith("}"))
            {
                binding = trimmed.Substring(1, trimmed.Length - 2);
                return true;
            }
            binding = default;
            return false;
        }

        /// <summary>
        /// Bind properties or data record to <see cref="TextShape.Text"/> property.
        /// </summary>
        /// <param name="text">The text shape instance.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <returns>The string bound to properties or data record.</returns>
        public static string BindText(this ITextShape text, ImmutableArray<Property> db, Record r)
        {
            var record = text.Data?.Record ?? r;

            if (!string.IsNullOrEmpty(text.Text))
            {
                if (IsBinding(text.Text, out string binding))
                {
                    // Try to bind to internal Record or external (r) data record using Text property as Column.Name name.
                    if (record != null)
                    {
                        bool success = TryToBind(record, binding, out string value);
                        if (success)
                        {
                            return value;
                        }
                    }

                    // Try to bind to external Properties database (e.g. Container.Data.Properties) using Text property as Property.Name name.
                    if (db != null)
                    {
                        bool success = TryToBind(db, binding, out string value);
                        if (success)
                        {
                            return value;
                        }
                    }
                }
            }

            // Try to bind to Properties using Text as formatting.
            if (text.Data?.Properties != null && text.Data.Properties.Length > 0)
            {
                try
                {
                    var args = text.Data.Properties.Where(x => x != null).Select(x => x.Value).ToArray();
                    if (text.Text != null && args != null && args.Length > 0)
                    {
                        return string.Format(text.Text, args);
                    }
                }
                catch (FormatException) { }
            }

            return text.Text;
        }
    }
}
