// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.Model.Renderer;

public interface ICache<in TKey, TValue> where TKey : notnull
{
    TValue? Get(TKey key);

    void Set(TKey key, TValue? value);

    void Reset();
}