// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dxf
{
    public class DxfTables : DxfObject
    {
        public const string TableAppId = "APPID";
        public const string TableDimstyle = "DIMSTYLE";
        public const string TableBlockRecord = "BLOCK_RECORD";
        public const string TableLType = "LTYPE";
        public const string TableLayer = "LAYER";
        public const string TableStyle = "STYLE";
        public const string TableUCS = "UCS";
        public const string TableView = "VIEW";
        public const string TableVPort = "VPORT";

        public DxfTable<DxfAppid> AppidTable { get; set; }
        public DxfTable<DxfDimstyle> DimstyleTable { get; set; }
        public DxfTable<DxfBlockRecord> BlockRecordTable { get; set; }
        public DxfTable<DxfLtype> LtypeTable { get; set; }
        public DxfTable<DxfLayer> LayerTable { get; set; }
        public DxfTable<DxfStyle> StyleTable { get; set; }
        public DxfTable<DxfUcs> UcsTable { get; set; }
        public DxfTable<DxfView> ViewTable { get; set; }
        public DxfTable<DxfVport> VportTable { get; set; }

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

        private void Add<T>(DxfTable<T> table) where T: DxfObject
        {
            foreach (var item in table.Items)
            {
                Append(item.Create());
            }
        }

        private void BeginDimstyles(int count, int id)
        {
            BeginTable(TableDimstyle, count, id);

            if (Version > DxfAcadVer.AC1009)
            {
                Subclass(DxfSubclassMarker.DimStyleTable);
                Add(71, count);
            }
        }

        private void EndDimstyles()
        {
            EndTable();
        }

        private void BeginAppids(int count, int id)
        {
            BeginTable(TableAppId, count, id);
        }

        private void EndAppids()
        {
            EndTable();
        }

        private void BeginBlockRecords(int count, int id)
        {
            BeginTable(TableBlockRecord, count, id);
        }

        private void EndBlockRecords()
        {
            EndTable();
        }

        private void BeginLtypes(int count, int id)
        {
            BeginTable(TableLType, count, id);
        }

        private void EndLtypes()
        {
            EndTable();
        }

        private void BeginLayers(int count, int id)
        {
            BeginTable(TableLayer, count, id);
        }

        private void EndLayers()
        {
            EndTable();
        }

        private void BeginStyles(int count, int id)
        {
            BeginTable(TableStyle, count, id);
        }

        private void EndStyles()
        {
            EndTable();
        }

        private void BeginUcss(int count, int id)
        {
            BeginTable(TableUCS, count, id);
        }

        private void EndUcss()
        {
            EndTable();
        }

        private void BeginViews(int count, int id)
        {
            BeginTable(TableView, count, id);
        }

        private void EndViews()
        {
            EndTable();
        }

        private void BeginVports(int count, int id)
        {
            BeginTable(TableVPort, count, id);
        }

        private void EndVports()
        {
            EndTable();
        }

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

        private void EndTable()
        {
            Add(0, DxfCodeName.Endtab);
        }
    }
}
