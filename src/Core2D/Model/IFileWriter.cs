// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.IO;

namespace Core2D.Model;

public interface IFileWriter: IPickerItem
{
    void Save(Stream stream, object item, object? options);
}
