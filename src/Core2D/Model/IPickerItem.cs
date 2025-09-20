// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

namespace Core2D.Model;

public interface IPickerItem
{
    string Name { get; }

    string Extension { get; }
}
