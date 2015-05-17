// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfEntities : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<DxfObject> Entities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfEntities(DxfAcadVer version, int id)
            : base(version, id)
        {
            Entities = new List<DxfObject>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Section);
            Add(2, DxfCodeName.Entities);

            foreach (var entity in Entities)
            {
                Append(entity.Create());
            }

            Add(0, DxfCodeName.EndSec);

            return Build();
        }
    }
}
