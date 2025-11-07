// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Threading.Tasks;

namespace Core2D.Model;

public interface IScriptRunner
{
    Task<object?> Execute(string code, object? state);
}
