// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;

namespace Core2D.Editor.Commands
{
    /// <summary>
    /// Set current tool to <see cref="Tool.CubicBezier"/> or current path tool to <see cref="PathTool.CubicBezier"/>.
    /// </summary>
    public interface IToolCubicBezierCommand : ICommand
    {
    }
}
