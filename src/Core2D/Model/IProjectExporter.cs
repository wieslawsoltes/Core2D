// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

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