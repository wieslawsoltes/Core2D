using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.Data;
using Core2D.Shapes;

namespace Core2D.Bindings
{
    /// <summary>
    /// Text binding.
    /// </summary>
    internal static class TextBinding
    {
        /// <summary>
        /// Try binding data record to <see cref="TextShape.Text"/> shape property containing column name.
        /// </summary>
        /// <param name="record">The external data record used for binding.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The output string bound to data record.</param>
        /// <returns>True if binding was successful.</returns>
        public static bool GetBindingValue(Record record, string columnName, out string value)
        {
            if (string.IsNullOrEmpty(columnName) || record == null)
            {
                value = null;
                return false;
            }

            var db = record.Owner as Database;
            var columns = db.Columns;
            var values = record.Values;
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
        /// <param name="properties">The properties database used for binding.</param>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="value">The string bound to properties.</param>
        /// <returns>True if binding was successful.</returns>
        public static bool GetBindingValue(ImmutableArray<Property> properties, string propertyName, out string value)
        {
            if (string.IsNullOrEmpty(propertyName) || properties == null)
            {
                value = null;
                return false;
            }

            var result = properties.FirstOrDefault(p => p.Name == propertyName);
            if (result != null && result.Value != null)
            {
                value = result.Value.ToString();
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Bind properties or data record to <see cref="TextShape.Text"/> property.
        /// </summary>
        /// <param name="shape">The text shape instance.</param>
        /// <param name="properties">The properties database used for binding.</param>
        /// <param name="externalRecord">The external data record used for binding.</param>
        /// <returns>The string bound to properties or data record.</returns>
        public static string Bind(TextShape shape, ImmutableArray<Property> properties, Record externalRecord)
        {
            var text = shape.Text;
            var data = shape.Data;
            var record = data?.Record ?? externalRecord;

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var bindings = BindingParser.Parse(text);
            if (bindings.Count > 0)
            {
                var sb = new StringBuilder(text);
                var count = 0;

                bindings.Reverse();

                foreach (var binding in bindings)
                {
                    // Try to bind to internal Record or external (r) data record using Text property as Column.Name name.
                    if (record != null)
                    {
                        bool success = GetBindingValue(record, binding.Path, out string value);
                        if (success)
                        {
                            sb.Replace(binding.Value, value, binding.Start, binding.Length);
                            count++;
                            continue;
                        }
                    }

                    // Try to bind to external Properties database (e.g. Container.Data.Properties) using Text property as Property.Name name.
                    if (properties != null && properties.Length > 0)
                    {
                        bool success = GetBindingValue(properties, binding.Path, out string value);
                        if (success)
                        {
                            sb.Replace(binding.Value, value, binding.Start, binding.Length);
                            count++;
                            continue;
                        }
                    }

                    // Try to bind to internal Properties database (e.g. Data.Properties) using Text property as Property.Name name.
                    if (data?.Properties != null && data.Properties.Length > 0)
                    {
                        bool success = GetBindingValue(data.Properties, binding.Path, out string value);
                        if (success)
                        {
                            sb.Replace(binding.Value, value, binding.Start, binding.Length);
                            count++;
                            continue;
                        }
                    }
                }

                if (count > 0)
                {
                    return sb.ToString();
                }
            }

            // Try to bind to Properties using Text as formatting args.
            if (data?.Properties != null && data.Properties.Length > 0)
            {
                try
                {
                    var args = data.Properties.Where(x => x != null).Select(x => x.Value).ToArray();
                    if (args != null && args.Length > 0)
                    {
                        return string.Format(text, args);
                    }
                }
                catch (FormatException) { }
            }

            return text;
        }
    }
}
