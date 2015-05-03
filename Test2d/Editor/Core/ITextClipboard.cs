// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public interface ITextClipboard
    {
        bool ContainsText();
        string GetText();
        void SetText(string text);
    }
}
