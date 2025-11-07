// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model;

public interface ISvgExporter
{
    string Create(object? item, double width, double height);
}
