// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.Model.Input;

[Flags]
public enum ModifierFlags
{
    None = 0,

    Alt = 1,

    Control = 2,

    Shift = 4
}
