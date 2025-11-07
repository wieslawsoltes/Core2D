// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;

namespace Core2D.Model;

public interface IContainerFactory
{
    TemplateContainerViewModel? GetTemplate(ProjectContainerViewModel project, string name);

    PageContainerViewModel? GetPage(ProjectContainerViewModel project, string name);

    DocumentContainerViewModel? GetDocument(ProjectContainerViewModel project, string name);

    ProjectContainerViewModel? GetProject();
}
