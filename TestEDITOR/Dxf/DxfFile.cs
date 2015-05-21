// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfFile : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public DxfHeader Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfClasses Classes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTables Tables { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfBlocks Blocks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfEntities Entities { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfObjects Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfFile(DxfAcadVer version, int id)
            : base(version, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Append(Header.Create());

            if (Version > DxfAcadVer.AC1009)
            {
                Append(Classes.Create());
            }

            Append(Tables.Create());
            Append(Blocks.Create());
            Append(Entities.Create());

            if (Version > DxfAcadVer.AC1009)
            {
                Append(Objects.Create());
            }

            Add(0, DxfCodeName.Eof);

            return Build();
        }
    }
}
