// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfTables : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public const string TableAppId = "APPID";
        /// <summary>
        /// 
        /// </summary>
        public const string TableDimstyle = "DIMSTYLE";
        /// <summary>
        /// 
        /// </summary>
        public const string TableBlockRecord = "BLOCK_RECORD";
        /// <summary>
        /// 
        /// </summary>
        public const string TableLType = "LTYPE";
        /// <summary>
        /// 
        /// </summary>
        public const string TableLayer = "LAYER";
        /// <summary>
        /// 
        /// </summary>
        public const string TableStyle = "STYLE";
        /// <summary>
        /// 
        /// </summary>
        public const string TableUCS = "UCS";
        /// <summary>
        /// 
        /// </summary>
        public const string TableView = "VIEW";
        /// <summary>
        /// 
        /// </summary>
        public const string TableVPort = "VPORT";

        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfAppid> AppidTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfDimstyle> DimstyleTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfBlockRecord> BlockRecordTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfLtype> LtypeTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfLayer> LayerTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfStyle> StyleTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfUcs> UcsTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfView> ViewTable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfTable<DxfVport> VportTable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfTables(DxfAcadVer version, int id)
            : base(version, id)
        {
            AppidTable = new DxfTable<DxfAppid>();
            DimstyleTable = new DxfTable<DxfDimstyle>();
            BlockRecordTable = new DxfTable<DxfBlockRecord>();
            LtypeTable = new DxfTable<DxfLtype>();
            LayerTable = new DxfTable<DxfLayer>();
            StyleTable = new DxfTable<DxfStyle>();
            UcsTable = new DxfTable<DxfUcs>();
            ViewTable = new DxfTable<DxfView>();
            VportTable = new DxfTable<DxfVport>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Section);
            Add(2, "TABLES");

            BeginAppids(AppidTable.Items.Count(), AppidTable.Id);
            Add(AppidTable);
            EndAppids();

            BeginDimstyles(DimstyleTable.Items.Count(), DimstyleTable.Id);
            Add(DimstyleTable);
            EndDimstyles();

            if (Version > DxfAcadVer.AC1009)
            {
                BeginBlockRecords(BlockRecordTable.Items.Count(), BlockRecordTable.Id);
                Add(BlockRecordTable);
                EndBlockRecords();
            }

            BeginLtypes(LtypeTable.Items.Count(), LtypeTable.Id);
            Add(LtypeTable);
            EndLtypes();

            BeginLayers(LayerTable.Items.Count(), LayerTable.Id);
            Add(LayerTable);
            EndLayers();

            BeginStyles(StyleTable.Items.Count(), StyleTable.Id);
            Add(StyleTable);
            EndStyles();

            BeginUcss(UcsTable.Items.Count(), UcsTable.Id);
            Add(UcsTable);
            EndUcss();

            BeginViews(ViewTable.Items.Count(), ViewTable.Id);
            Add(ViewTable);
            EndViews();

            BeginVports(VportTable.Items.Count(), VportTable.Id);
            Add(VportTable);
            EndVports();

            Add(0, DxfCodeName.EndSec);

            return Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        private void Add<T>(DxfTable<T> table) where T: DxfObject
        {
            foreach (var item in table.Items)
            {
                Append(item.Create());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginDimstyles(int count, int id)
        {
            BeginTable(TableDimstyle, count, id);

            if (Version > DxfAcadVer.AC1009)
            {
                Subclass(DxfSubclassMarker.DimStyleTable);
                Add(71, count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndDimstyles()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginAppids(int count, int id)
        {
            BeginTable(TableAppId, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndAppids()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginBlockRecords(int count, int id)
        {
            BeginTable(TableBlockRecord, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndBlockRecords()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginLtypes(int count, int id)
        {
            BeginTable(TableLType, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndLtypes()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginLayers(int count, int id)
        {
            BeginTable(TableLayer, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndLayers()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginStyles(int count, int id)
        {
            BeginTable(TableStyle, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndStyles()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginUcss(int count, int id)
        {
            BeginTable(TableUCS, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndUcss()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginViews(int count, int id)
        {
            BeginTable(TableView, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndViews()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginVports(int count, int id)
        {
            BeginTable(TableVPort, count, id);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndVports()
        {
            EndTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="id"></param>
        private void BeginTable(string name, int count, int id)
        {
            Add(0, DxfCodeName.Table);
            Add(2, name);

            if (Version > DxfAcadVer.AC1009)
            {
                Handle(id);
                Subclass(DxfSubclassMarker.SymbolTable);
            }

            Add(70, count);
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndTable()
        {
            Add(0, DxfCodeName.Endtab);
        }
    }
}
