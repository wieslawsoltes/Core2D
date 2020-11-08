using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.Data;
using Core2D.Shapes;

namespace Core2D.Bindings
{
    internal static class TextBinding
    {
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

        public static bool GetBindingValue(ImmutableArray<Property> properties, string propertyName, out string value)
        {
            if (string.IsNullOrEmpty(propertyName) || properties == null)
            {
                value = null;
                return false;
            }

            var result = properties.FirstOrDefault(p => p.Name == propertyName);
            if (result?.Value != null)
            {
                value = result.Value.ToString();
                return true;
            }

            value = null;
            return false;
        }

        public static string Bind(TextShape shape, ImmutableArray<Property> properties, Record externalRecord)
        {
            var text = shape.Text;
            var record = shape.Record ?? externalRecord;

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

                    // Try to bind to external Properties database (e.g. Container.Properties) using Text property as Property.Name name.
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

                    // Try to bind to internal Properties database (e.g. Properties) using Text property as Property.Name name.
                    if (shape.Properties != null && shape.Properties.Length > 0)
                    {
                        bool success = GetBindingValue(shape.Properties, binding.Path, out string value);
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
            if (shape.Properties != null && shape.Properties.Length > 0)
            {
                try
                {
                    var args = shape.Properties.Where(x => x != null).Select(x => x.Value).ToArray();
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
