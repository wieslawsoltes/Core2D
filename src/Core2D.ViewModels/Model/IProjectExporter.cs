// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.IO;
using Core2D.ViewModels.Containers;

namespace Core2D.Model;

public interface IProjectExporter
{
    void Save(Stream stream, PageContainerViewModel container);

    void Save(Stream stream, DocumentContainerViewModel document);

    void Save(Stream stream, ProjectContainerViewModel project);
}
