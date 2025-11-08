// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;

namespace Core2D.ViewModels.Export;

/// <summary>
/// Serializable representation of an export options instance.
/// </summary>
public sealed record ExportOptionsDto(string TypeId, IDictionary<string, object?> Values);
