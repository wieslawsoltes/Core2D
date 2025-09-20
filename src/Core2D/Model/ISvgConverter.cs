// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.IO;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface ISvgConverter
{
    IList<BaseShapeViewModel>? Convert(Stream stream, out double width, out double height);

    IList<BaseShapeViewModel>? FromString(string text, out double width, out double height);
}
