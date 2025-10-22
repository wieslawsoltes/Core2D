// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model.Renderer;

public enum PathOp
{
    Difference = 0,

    Intersect = 1,

    Union = 2,

    Xor = 3,

    ReverseDifference = 4
}
