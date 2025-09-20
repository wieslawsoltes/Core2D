// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Immutable;
using System.Windows.Input;
using Core2D.ViewModels.Data;

namespace Core2D.Model;

public interface IDataObject
{
    ImmutableArray<PropertyViewModel> Properties { get; set; }

    RecordViewModel? Record { get; set; }

    ICommand AddProperty { get; }

    ICommand RemoveProperty { get; }

    ICommand ResetRecord { get; }
}