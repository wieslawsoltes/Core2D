// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.IO;

namespace Core2D.Model;

public interface ITextFieldReader<out T> : IPickerItem
{
    T? Read(Stream stream);
}
