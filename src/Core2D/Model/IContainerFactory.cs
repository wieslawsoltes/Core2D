// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

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