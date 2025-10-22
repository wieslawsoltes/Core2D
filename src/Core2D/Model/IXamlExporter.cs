// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model;

public interface IXamlExporter
{
    string Create(object item, string? key);
}
