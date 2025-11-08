// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Wizard.Export.Scopes;

public sealed record class ExportScopeSelection(
    ExportScopeKind Kind,
    ProjectContainerViewModel Project,
    DocumentContainerViewModel? Document,
    PageContainerViewModel? Page);
