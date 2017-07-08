// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;

namespace Core2D.Editor.Commands
{
    /// <summary>
    /// Set current tool to <see cref="ToolCubicBezier"/> or current path tool to <see cref="PathToolCubicBezier"/>.
    /// </summary>
    public interface IToolCubicBezierCommand : ICommand
    {
    }
}
