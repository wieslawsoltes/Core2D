#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Data.Bindings;

internal static class TextBinding
{
    private static bool GetBindingValue(RecordViewModel? record, string? columnName, out string? value)
    {
        if (string.IsNullOrEmpty(columnName) || record is null)
        {
            value = null;
            return false;
        }

        if (record.Owner is DatabaseViewModel db)
        {
            var columns = db.Columns;
            var values = record.Values;
            if (columns.Length != values.Length)
            {
                value = null;
                return false;
            }

            for (var i = 0; i < columns.Length; i++)
            {
                if (columns[i].Name == columnName)
                {
                    value = values[i].Content;
                    return true;
                }
            }
        }

        value = null;
        return false;
    }

    private static bool GetBindingValue(ImmutableArray<PropertyViewModel> properties, string? propertyName, out string? value)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            value = null;
            return false;
        }

        var result = properties.FirstOrDefault(p => p.Name == propertyName);
        if (result?.Value is { })
        {
            value = result.Value;
            return true;
        }

        value = null;
        return false;
    }

    private static bool TryToBind(TextShapeViewModel shape, ImmutableArray<PropertyViewModel>? properties, RecordViewModel? record, List<BindingPart> bindings, string? text, out string? result)
    {
        var sb = new StringBuilder(text);
        var count = 0;

        bindings.Reverse();

        foreach (var binding in bindings)
        {
            // Try to bind to internal Record or external (r) data record using Text property as Column.Name name.
            if (record is { })
            {
                var success = GetBindingValue(record, binding.Path, out var value);
                if (success && value is { })
                {
                    sb.Replace(binding.Value, value, binding.Start, binding.Length);
                    count++;
                    continue;
                }
            }

            // Try to bind to external Properties database (e.g. Container.Properties) using Text property as Property.Name name.
            if (properties is { Length: > 0 })
            {
                var success = GetBindingValue(properties.Value, binding.Path, out var value);
                if (success && value is { })
                {
                    sb.Replace(binding.Value, value, binding.Start, binding.Length);
                    count++;
                    continue;
                }
            }

            // Try to bind to internal Properties database (e.g. Properties) using Text property as Property.Name name.
            if (shape.Properties.Length > 0)
            {
                var success = GetBindingValue(shape.Properties, binding.Path, out var value);
                if (success && value is { })
                {
                    sb.Replace(binding.Value, value, binding.Start, binding.Length);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            result = sb.ToString();
            return true;
        }

        result = null;
        return false;
    }

    public static string? Bind(TextShapeViewModel shape, ImmutableArray<PropertyViewModel>? properties, RecordViewModel? externalRecordViewModel)
    {
        var text = shape.Text;
        var record = shape.Record ?? externalRecordViewModel;

        if (text is null || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var bindings = BindingParser.Parse(text);
        if (bindings.Count > 0)
        {
            if (TryToBind(shape, properties, record, bindings, text, out var result))
            {
                return result;
            }
        }

        // Try to bind to Properties using Text as formatting args.
        if (shape.Properties.Length > 0)
        {
            try
            {
                object?[] args = shape.Properties.Select(x => (object?)x.Value).ToArray();
                if (args.Length > 0)
                {
                    return string.Format(text, args);
                }
            }
            catch (FormatException) { }
        }

        return text;
    }
}