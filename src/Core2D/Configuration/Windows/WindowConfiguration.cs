// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;

namespace Core2D.Configuration.Windows;

public class WindowConfiguration
{
    public double X { get; set; } = double.NaN;
    public double Y { get; set; } = double.NaN;
    public double Width { get; set; } = double.NaN;
    public double Height { get; set; } = double.NaN;
    public WindowState WindowState { get; set; } = WindowState.Normal;
}