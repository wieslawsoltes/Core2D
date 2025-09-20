// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace Core2D.Controls;

public class EditableItem : TemplatedControl
{
    public static readonly StyledProperty<ContextMenu?> TextContextMenuProperty = 
        AvaloniaProperty.Register<EditableItem, ContextMenu?>(nameof(TextContextMenu));

    public static readonly StyledProperty<IBinding?> TextBindingProperty = 
        AvaloniaProperty.Register<EditableItem, IBinding?>(nameof(TextBinding));

    public static readonly StyledProperty<object?> IconContentProperty = 
        AvaloniaProperty.Register<EditableItem, object?>(nameof(IconContent));

    public ContextMenu? TextContextMenu
    {
        get => GetValue(TextContextMenuProperty);
        set => SetValue(TextContextMenuProperty, value);
    }

    [AssignBinding]
    public IBinding? TextBinding
    {
        get => GetValue(TextBindingProperty);
        set => SetValue(TextBindingProperty, value);
    }

    public object? IconContent
    {
        get => GetValue(IconContentProperty);
        set => SetValue(IconContentProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        var textBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");

        if (textBox is { } && TextBinding is { })
        {
            textBox.Bind(TextBox.TextProperty, TextBinding);
        }

        if (textBlock is { } && TextBinding is { })
        {
            textBlock.Bind(TextBlock.TextProperty, TextBinding);
        }
    }
}
