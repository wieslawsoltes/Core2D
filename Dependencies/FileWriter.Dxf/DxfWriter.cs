// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfWriter : IFileWriter
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

            var renderer = new DxfRenderer();
            renderer.State.DrawShapeState = ShapeState.Printable;

            if (item is Container)
            {
                renderer.Save(
                    path, 
                    item as Container, 
                    options == null ? Dxf.DxfAcadVer.AC1015 : (Dxf.DxfAcadVer)options);
            }
        }
    }
}
