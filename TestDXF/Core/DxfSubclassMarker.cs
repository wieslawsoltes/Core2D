// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    public static class DxfSubclassMarker
    {
        public const string Arc = "AcDbArc";
        public const string Circle = "AcDbCircle";
        public const string Ellipse = "AcDbEllipse";
        public const string Line = "AcDbLine";
        public const string Lwpolyline = "AcDbPolyline";
        public const string Text = "AcDbText";

        public const string Entity = "AcDbEntity";
        public const string Dictionary = "AcDbDictionary";

        public const string Attribute = "AcDbAttribute";
        public const string AttributeDefinition = "AcDbAttributeDefinition";

        public const string DimStyleTable = "AcDbDimStyleTable";
        public const string SymbolTable = "AcDbSymbolTable";

        public const string BlockTableRecord = "AcDbBlockTableRecord";
        public const string DimStyleTableRecord = "AcDbDimStyleTableRecord";
        public const string LayerTableRecord = "AcDbLayerTableRecord";
        public const string LinetypeTableRecord = "AcDbLinetypeTableRecord";
        public const string SymbolTableRecord = "AcDbSymbolTableRecord";
        public const string TextStyleTableRecord = "AcDbTextStyleTableRecord";
        public const string UCSTableRecord = "AcDbUCSTableRecord";
        public const string ViewportTableRecord = "AcDbViewportTableRecord";
        public const string ViewTableRecord = "AcDbViewTableRecord";

        public const string RegAppTableRecord = "AcDbRegAppTableRecord";

        public const string BlockBegin = "AcDbBlockBegin";
        public const string BlockEnd = "AcDbBlockEnd";
        public const string BlockReference = "AcDbBlockReference";
    }
}
