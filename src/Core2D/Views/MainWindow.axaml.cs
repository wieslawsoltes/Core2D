﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Screenshot;

namespace Core2D.Views;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
        this.AttachCapture();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}