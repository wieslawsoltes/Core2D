// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OmniXaml.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core2D.Xaml.Collections
{
    [ContentProperty("Children")]
    public class Shapes
    {
        public string Name { get; set; }
        public ICollection<BaseShape> Children { get; set; }

        public Shapes()
        {
            Children = new Collection<BaseShape>();
        }
    }
}
