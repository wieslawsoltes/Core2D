// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.ViewModels;

namespace Core2D.ViewModels.Export;

/// <summary>
/// Base class for all exporter option view models.
/// </summary>
public abstract partial class ExportOptionsBase : ViewModelBase, IExportOptionsSerializable
{
    [AutoNotify] private string _displayName = string.Empty;
    [AutoNotify] private string _description = string.Empty;
    [AutoNotify] private bool _isValid = true;
    [AutoNotify] private IReadOnlyList<string> _validationErrors = Array.Empty<string>();

    protected ExportOptionsBase(IServiceProvider? serviceProvider, string displayName, string description)
        : base(serviceProvider)
    {
        _displayName = displayName;
        _description = description;
    }

    public virtual void Validate()
    {
        SetValidationResult(true, Array.Empty<string>());
    }

    protected void SetValidationResult(bool isValid, IEnumerable<string>? errors)
    {
        IsValid = isValid;
        ValidationErrors = errors?.ToArray() ?? Array.Empty<string>();
    }

    public virtual ExportOptionsDto ToDto()
        => new(GetType().FullName ?? string.Empty, new Dictionary<string, object?>());

    public virtual void LoadFromDto(ExportOptionsDto dto)
    {
        // Derived classes populate their own properties.
    }
}
