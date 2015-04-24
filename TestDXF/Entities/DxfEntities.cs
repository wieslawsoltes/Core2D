// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Dxf
{
    public class DxfEntities : DxfObject<DxfEntities>
    {
        public DxfEntities(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        public DxfEntities Begin()
        {
            Add(0, DxfCodeName.Section);
            Add(2, DxfCodeName.Entities);
            return this;
        }

        public DxfEntities Add<T>(T entity)
        {
            Append(entity.ToString());
            return this;
        }

        public DxfEntities Add<T>(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }

            return this;
        }

        public DxfEntities End()
        {
            Add(0, DxfCodeName.EndSec);
            return this;
        }
    }
}
