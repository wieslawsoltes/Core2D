// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfObjects : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<DxfDictionary> Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfObjects(DxfAcadVer version, int id)
            : base(version, id)
        {
            Objects = new List<DxfDictionary>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Section);
            Add(2, "OBJECTS");

            foreach(var obj in Objects)
            {
                Append(obj.Create());
            }

            Add(0, DxfCodeName.EndSec);

            return Build();
        }
    }
}
