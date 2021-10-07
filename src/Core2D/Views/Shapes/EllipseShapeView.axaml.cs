﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views.Shapes;

public class EllipseShapeView : UserControl
{
    public EllipseShapeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}