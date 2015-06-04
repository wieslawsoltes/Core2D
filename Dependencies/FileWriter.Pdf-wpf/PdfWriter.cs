// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfWriter : IFileWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="options"></param>
        public void Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var renderer = new PdfRenderer()
            {
                DrawShapeState = ShapeState.Printable
            };

            if (item is Container)
            {
                renderer.Save(path, item as Container);
            }
            else if (item is Document)
            {
                renderer.Save(path, item as Document);
            }
            else if (item is Project)
            {
                renderer.Save(path, item as Project);
            }
        }
    }
}
