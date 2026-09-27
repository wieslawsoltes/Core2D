// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Model.Editor;

/// <summary>An immutable page-space ruler guide. Vertical guides store an X coordinate.</summary>
public sealed record CanvasGuide(Guid Id, bool IsVertical, double Position);
