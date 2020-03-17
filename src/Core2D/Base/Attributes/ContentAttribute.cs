// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Attributes
{
    /// <summary>
    /// Defines the property that contains the object's content in markup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentAttribute : Attribute
    {
    }
}
