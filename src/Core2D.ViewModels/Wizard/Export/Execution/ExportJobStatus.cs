// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.ViewModels.Wizard.Export.Execution;

public enum ExportJobStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}
