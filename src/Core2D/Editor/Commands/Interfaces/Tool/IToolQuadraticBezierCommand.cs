// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;
using Core2D.Editor.Tools;
using Core2D.Editor.Tools.Path;

namespace Core2D.Editor.Commands
{
    /// <summary>
    /// Set current tool to <see cref="ToolQuadraticBezier"/> or current path tool to <see cref="PathToolQuadraticBezier"/>.
    /// </summary>
    public interface IToolQuadraticBezierCommand : ICommand
    {
    }
}
