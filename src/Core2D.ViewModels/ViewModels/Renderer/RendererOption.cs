// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.ViewModels.Renderer;

/// <summary>
/// Describes a selectable renderer option exposed in the UI.
/// </summary>
public sealed record RendererOption(string Id, string Title, string Description);
