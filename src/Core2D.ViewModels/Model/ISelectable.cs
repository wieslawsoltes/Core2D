// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model;

public interface ISelectable
{
    void Move(ISelection? selection, decimal dx, decimal dy);

    void Select(ISelection? selection);

    void Deselect(ISelection? selection);
}
