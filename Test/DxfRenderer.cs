// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace Dxf
{
    public class DxfRenderer
    {
        private DxfAcadVer _version = DxfAcadVer.AC1015;
        private string _layer = "0";
        private int _handle = 0;
        private string _defaultStyle = "Standard";
        private double _pageWidth;
        private double _pageHeight;
        private string _stylePrimatyFont = "calibri.ttf"; // "arial.ttf"; "arialuni.ttf";
        private string _stylePrimatyFontDescription = "Calibri"; // "Arial"; "Arial Unicode MS"
        private string _styleBigFont = "";

        private int NextHandle() { return _handle += 1; }
        private double ToDxfX(double x) { return x; }
        private double ToDxfY(double y) { return _pageHeight - y; }

        private string EncodeText(string text)
        {
            if (_version >= DxfAcadVer.AC1021)
                return text;
            if (string.IsNullOrEmpty(text))
                return text;
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c > 255)
                    sb.Append(string.Concat("\\U+", Convert.ToInt32(c).ToString("X4")));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private IEnumerable<DxfAppid> TableAppids()
        {
            var appids = new List<DxfAppid>();

            // ACAD - default must be present
            if (_version > DxfAcadVer.AC1009)
            {
                var acad = new DxfAppid(_version, NextHandle())
                {
                    ApplicationName = "ACAD",
                    AppidStandardFlags = DxfAppidStandardFlags.Default
                }.Create();

                appids.Add(acad);
            }

            return appids;
        }

        private IEnumerable<DxfDimstyle> TableDimstyles()
        {
            var dimstyles = new List<DxfDimstyle>();

            if (_version > DxfAcadVer.AC1009)
            {
                dimstyles.Add(new DxfDimstyle(_version, NextHandle())
                {
                    Name = "Standard"
                }.Create());
            }

            return dimstyles;
        }

        private IEnumerable<DxfLayer> TableLayers()
        {
            var layers = new List<DxfLayer>();

            // default layer 0 - must be present
            if (_version > DxfAcadVer.AC1009)
            {
                layers.Add(new DxfLayer(_version, NextHandle())
                {
                    Name = "0",
                    LayerStandardFlags = DxfLayerStandardFlags.Default,
                    Color = DxfDefaultColors.Default.ColorToString(),
                    LineType = "Continuous",
                    PlottingFlag = true,
                    LineWeight = DxfLineWeight.LnWtByLwDefault,
                    PlotStyleNameHandle = "0"
                }.Create());
            }

            return layers;
        }

        private IEnumerable<DxfLtype> TableLtypes()
        {
            var ltypes = new List<DxfLtype>();

            // default ltypes ByLayer, ByBlock and Continuous - must be present

            // ByLayer
            ltypes.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "ByLayer",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "ByLayer",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            }.Create());

            // ByBlock
            ltypes.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "ByBlock",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "ByBlock",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            }.Create());

            // Continuous
            ltypes.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "Continuous",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "Solid line",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            }.Create());

            return ltypes;
        }

        private IEnumerable<DxfStyle> TableStyles()
        {
            var styles = new List<DxfStyle>();

            // style: Standard
            var standard = new DxfStyle(_version, NextHandle())
            {
                Name = "Standard",
                StyleStandardFlags = DxfStyleFlags.Default,
                FixedTextHeight = 0,
                WidthFactor = 1,
                ObliqueAngle = 0,
                TextGenerationFlags = DxfTextGenerationFlags.Default,
                LastHeightUsed = 1,
                PrimaryFontFile = _stylePrimatyFont,
                BifFontFile = _styleBigFont,
                PrimatyFontDescription = _stylePrimatyFontDescription
            }.Create();

            styles.Add(standard);

            return styles;
        }

        private IEnumerable<DxfUcs> TableUcss()
        {
            return Enumerable.Empty<DxfUcs>();
        }

        private IEnumerable<DxfView> TableViews()
        {
            var view = new DxfView(_version, NextHandle())
            {
                Name = "View",
                ViewStandardFlags = DxfViewStandardFlags.Default,
                Height = _pageHeight,
                Width = _pageWidth,
                Center = new DxfVector2(_pageWidth / 2, _pageHeight / 2),
                ViewDirection = new DxfVector3(0, 0, 1),
                TargetPoint = new DxfVector3(0, 0, 0),
                FrontClippingPlane = 0,
                BackClippingPlane = 0,
                TwistAngle = 0
            }.Create();

            yield return view;
        }

        private IEnumerable<DxfVport> TableVports()
        {
            var vports = new List<DxfVport>();

            if (_version > DxfAcadVer.AC1009)
            {
                vports.Add(new DxfVport(_version, NextHandle())
                {
                    Name = "*Active"
                }.Create());
            }

            return vports;
        }

        private IEnumerable<DxfBlock> DefaultBlocks()
        {
            if (_version > DxfAcadVer.AC1009)
            {
                var blocks = new List<DxfBlock>();
                string layer = "0";

                blocks.Add(new DxfBlock(_version, NextHandle())
                {
                    Name = "*Model_Space",
                    Layer = layer,
                    BlockTypeFlags = DxfBlockTypeFlags.Default,
                    BasePoint = new DxfVector3(0, 0, 0),
                    XrefPathName = null,
                    Description = null,
                    EndId = NextHandle(),
                    EndLayer = layer,
                    Entities = null
                }.Create());

                blocks.Add(new DxfBlock(_version, NextHandle())
                {
                    Name = "*Paper_Space",
                    Layer = layer,
                    BlockTypeFlags = DxfBlockTypeFlags.Default,
                    BasePoint = new DxfVector3(0, 0, 0),
                    XrefPathName = null,
                    Description = null,
                    EndId = NextHandle(),
                    EndLayer = layer,
                    Entities = null
                }.Create());

                blocks.Add(new DxfBlock(_version, NextHandle())
                {
                    Name = "*Paper_Space0",
                    Layer = layer,
                    BlockTypeFlags = DxfBlockTypeFlags.Default,
                    BasePoint = new DxfVector3(0, 0, 0),
                    XrefPathName = null,
                    Description = null,
                    EndId = NextHandle(),
                    EndLayer = layer,
                    Entities = null
                }.Create());

                return blocks;
            }

            return Enumerable.Empty<DxfBlock>();
        }

        private DxfBlockRecord CreateBlockRecordForBlock(string name)
        {
            return new DxfBlockRecord(_version, NextHandle())
            {
                Name = name
            }.Create();
        }

        private DxfLine CreateLine(double x1, double y1, double x2, double y2)
        {
            double _x1 = ToDxfX(x1);
            double _y1 = ToDxfY(y1);
            double _x2 = ToDxfX(x2);
            double _y2 = ToDxfY(y2);

            return new DxfLine(_version, NextHandle())
            {
                Layer = _layer,
                Color = DxfDefaultColors.ByLayer.ColorToString(),
                Thickness = 0.0,
                StartPoint = new DxfVector3(_x1, _y1, 0),
                EndPoint = new DxfVector3(_x2, _y2, 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1)
            }.Create();
        }

        private DxfCircle CreateCircle(double x, double y, double radius)
        {
            double _x = ToDxfX(x);
            double _y = ToDxfY(y);

            return new DxfCircle(_version, NextHandle())
            {
                Layer = _layer,
                Color = DxfDefaultColors.ByLayer.ColorToString(),
                Thickness = 0.0,
                CenterPoint = new DxfVector3(_x, _y, 0),
                Radius = radius,
                ExtrusionDirection = new DxfVector3(0, 0, 1),
            }.Create();
        }

        private DxfEllipse CreateEllipse(double x, double y, double width, double height)
        {
            double _cx = ToDxfX(x + width / 2.0);
            double _cy = ToDxfY(y + height / 2.0);
            double _ex = width / 2.0; // relative to _cx
            double _ey = 0.0; // relative to _cy

            return new DxfEllipse(_version, NextHandle())
            {
                Layer = _layer,
                Color = DxfDefaultColors.ByLayer.ColorToString(),
                CenterPoint = new DxfVector3(_cx, _cy, 0),
                EndPoint = new DxfVector3(_ex, _ey, 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1),
                Ratio = height / width,
                StartParameter = 0.0,
                EndParameter = 2.0 * Math.PI
            }.Create();
        }

        private DxfText CreateText(
            string text, 
            double x, double y, 
            double height, 
            DxfHorizontalTextJustification horizontalJustification, 
            DxfVerticalTextJustification verticalJustification, 
            string style)
        {
            return new DxfText(_version, NextHandle())
            {
                Thickness = 0,
                Layer = _layer,
                Color = DxfDefaultColors.ByLayer.ColorToString(),
                FirstAlignment = new DxfVector3(ToDxfX(x), ToDxfY(y), 0),
                TextHeight = height,
                DefaultValue = EncodeText(text),
                TextRotation = 0,
                ScaleFactorX = 1,
                ObliqueAngle = 0,
                TextStyle = style,
                TextGenerationFlags = DxfTextGenerationFlags.Default,
                HorizontalTextJustification = horizontalJustification,
                SecondAlignment = new DxfVector3(ToDxfX(x), ToDxfY(y), 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1),
                VerticalTextJustification = verticalJustification
            }.Create();
        }

        private static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            double width = (brx - tlx);
            double height = (bry - tly);
            return new Rect2(tlx + dx, tly + dy, width, height);
        }

        private void DrawLine(DxfEntities entities, XLine line)
        {
            entities.Add(CreateLine(line.Start.X, line.Start.Y, line.End.X, line.End.Y));
        }

        private void DrawRectangle(DxfEntities entities, XRectangle rectangle)
        {
            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, 0.0, 0.0);
            entities.Add(CreateLine(rect.X, rect.Y, rect.X + rect.Width, rect.Y));
            entities.Add(CreateLine(rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height));
            entities.Add(CreateLine(rect.X, rect.Y, rect.X, rect.Y + rect.Height));
            entities.Add(CreateLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height));
        }

        private void DrawEllipse(DxfEntities entities, XEllipse ellipse)
        {
            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, 0.0, 0.0);
            entities.Add(CreateEllipse(rect.X, rect.Y, rect.Width, rect.Height));
        }

        private void DrawText(DxfEntities entities, XText text)
        {
            DxfHorizontalTextJustification halign;
            DxfVerticalTextJustification valign;
            double x, y;

            var rect = CreateRect(text.TopLeft, text.BottomRight, 0.0, 0.0);
            
            switch (text.Style.TextStyle.TextHAlignment)
            {
                default:
                case TextHAlignment.Left:
                    halign = DxfHorizontalTextJustification.Left;
                    x = rect.X;
                    break;
                case TextHAlignment.Center:
                    halign = DxfHorizontalTextJustification.Center;
                    x = rect.X + rect.Width / 2.0;
                    break;
                case TextHAlignment.Right:
                    halign = DxfHorizontalTextJustification.Right;
                    x = rect.X + rect.Width;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case TextVAlignment.Top:
                    valign = DxfVerticalTextJustification.Top;
                    y = rect.Y;
                    break;
                case TextVAlignment.Center:
                    valign = DxfVerticalTextJustification.Middle;
                    y = rect.Y + rect.Height / 2.0;
                    break;
                case TextVAlignment.Bottom:
                    valign = DxfVerticalTextJustification.Bottom;
                    y = rect.Y + rect.Height;
                    break;
            }

            entities.Add(CreateText(
                text.Text, 
                x, y, 
                text.Style.TextStyle.FontSize * (72.0 / 96.0), 
                halign, 
                valign, 
                _defaultStyle));
        }

        private void DrawShapes(DxfEntities entities, IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes) 
            {
                if (shape.State.HasFlag(ShapeState.Printable))
                {
                    if (shape is XPoint)
                    {
                        var point = shape as XPoint;
                        // TODO:
                    }
                    else if (shape is XLine)
                    {
                        var line = shape as XLine;
                        DrawLine(entities, line);
                        // TODO: Draw start and end arrows.
                    }
                    else if (shape is XRectangle)
                    {
                        var rectangle = shape as XRectangle;
                        DrawRectangle(entities, rectangle);
                    }
                    else if (shape is XEllipse)
                    {
                        var ellipse = shape as XEllipse;
                        DrawEllipse(entities, ellipse);
                    }
                    else if (shape is XArc)
                    {
                        var arc = shape as XArc;
                        // TODO:
                    }
                    else if (shape is XBezier)
                    {
                        var bezier = shape as XBezier;
                        // TODO:
                    }
                    else if (shape is XQBezier)
                    {
                        var qbezier = shape as XQBezier;
                        // TODO:
                    }
                    else if (shape is XText)
                    {
                        var text = shape as XText;
                        DrawText(entities, text);
                    }
                    else if (shape is XGroup)
                    {
                        var group = shape as XGroup;
                        DrawShapes(entities, group.Shapes);
                    }
                }
            }
        }
 
        private void Save(string path, string text)
        {
            try
            {
                if (text != null)
                {
                    using (var stream = System.IO.File.CreateText(path))
                    {
                        stream.Write(text);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        public void Create(string path, Container container)
        {
            _pageWidth = container.Width;
            _pageHeight = container.Height;

            _version = DxfAcadVer.AC1015;

            _layer = "0";
            _handle = 0;

            // dxf file sections
            DxfHeader header = null;
            DxfClasses classes = null;
            DxfTables tables = null;
            DxfBlocks blocks = null;
            DxfObjects objects = null;

            // create dxf file
            var file = new DxfFile(_version, NextHandle());

            // create header
            header = new DxfHeader(_version, NextHandle()).Begin().Default();

            // create classes
            if (_version > DxfAcadVer.AC1009)
            {
                classes = new DxfClasses(_version, NextHandle()).Begin();

                // classes.Add(new DxfClass(...));

                classes.End();
            }

            // create tables
            tables = new DxfTables(_version, NextHandle());
            tables.Begin();
            tables.AddAppidTable(TableAppids(), NextHandle());
            tables.AddDimstyleTable(TableDimstyles(), NextHandle());

            if (_version > DxfAcadVer.AC1009)
            {
                var records = new List<DxfBlockRecord>();

                // TODO: each BLOCK must have BLOCK_RECORD entry

                // required block records by dxf format
                records.Add(CreateBlockRecordForBlock("*Model_Space"));
                records.Add(CreateBlockRecordForBlock("*Paper_Space"));
                records.Add(CreateBlockRecordForBlock("*Paper_Space0"));

                // TODO: add user layers

                //records.Add(CreateBlockRecordForBlock("NEW_LAYER_NAME"));

                tables.AddBlockRecordTable(records, NextHandle());
            }

            tables.AddLtypeTable(TableLtypes(), NextHandle());
            tables.AddLayerTable(TableLayers(), NextHandle());
            tables.AddStyleTable(TableStyles(), NextHandle());
            tables.AddUcsTable(TableUcss(), NextHandle());
            tables.AddViewTable(TableViews(), NextHandle());
            tables.AddVportTable(TableVports(), NextHandle());

            tables.End();

            // create blocks
            blocks = new DxfBlocks(_version, NextHandle()).Begin();
            blocks.Add(DefaultBlocks());

            // TODO: add user blocks

            blocks.End();

            // create entities
            var Entities = new DxfEntities(_version, NextHandle()).Begin();

            // TODO: add user entities

            if (container.TemplateLayer.IsVisible)
            {
                DrawShapes(Entities, container.TemplateLayer.Shapes);
            }
            
            foreach (var layer in container.Layers) 
            {
                if (layer.IsVisible)
                {
                    DrawShapes(Entities, layer.Shapes);
                }
            }

            Entities.End();

            // create objects
            if (_version > DxfAcadVer.AC1009)
            {
                objects = new DxfObjects(_version, NextHandle()).Begin();

                // mamed dictionary
                var namedDict = new DxfDictionary(_version, NextHandle())
                {
                    OwnerDictionaryHandle = 0.ToDxfHandle(),
                    HardOwnerFlag = false,
                    DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting,
                    Entries = new Dictionary<string, string>()
                };

                // base dictionary
                var baseDict = new DxfDictionary(_version, NextHandle())
                {
                    OwnerDictionaryHandle = namedDict.Id.ToDxfHandle(),
                    HardOwnerFlag = false,
                    DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting,
                    Entries = new Dictionary<string, string>()
                };

                // add baseDict to namedDict
                namedDict.Entries.Add(baseDict.Id.ToDxfHandle(), "ACAD_GROUP");

                // finalize dictionaries
                objects.Add(namedDict.Create());
                objects.Add(baseDict.Create());

                // TODO: add Group dictionary
                // TODO: add MLine style dictionary
                // TODO: add image dictionary dictionary

                // finalize objects
                objects.End();
            }

            // finalize dxf file
            file.Header(header.End(NextHandle()));

            if (_version > DxfAcadVer.AC1009)
            {
                file.Classes(classes);
            }

            file.Tables(tables);
            file.Blocks(blocks);
            file.Entities(Entities);

            if (_version > DxfAcadVer.AC1009)
            {
                file.Objects(objects);
            }

            file.Eof();

            Save(path, file.ToString());
        }
    }

    /*
    private DxfEntities Entities = null;
    private const double PageWidth = 1260;
    private const double PageHeight = 891;
    private const string LayerFrame = "FRAME";
    private const string LayerGrid = "GRID";
    private const string LayerTable = "TABLE";
    private const string LayerIO = "IO";
    private const string LayerWires = "WIRES";
    private const string LayerElements = "ELEMENTS";
    private string StylePrimatyFont = "arial.ttf"; // arialuni.ttf
    private string StylePrimatyFontDescription = "Arial"; // Arial Unicode MS
    private string StyleBigFont = "";

    private DxfLine Line(double x1, double y1,
        double x2, double y2,
        double offsetX, double offsetY,
        string layer)
    {
        double _x1 = X(x1 + offsetX);
        double _y1 = Y(y1 + offsetY);
        double _x2 = X(x2 + offsetX);
        double _y2 = Y(y2 + offsetY);

        double thickness = 0;

        var line = new DxfLine(Version, GetNextHandle())
        {
            Layer = layer,
            Color = DxfDefaultColors.ByLayer.ColorToString(),
            Thickness = thickness,
            StartPoint = new Vector3(_x1, _y1, 0),
            EndPoint = new Vector3(_x2, _y2, 0),
            ExtrusionDirection = new Vector3(0, 0, 1)
        };

        return line.Create();
    }

    private DxfCircle Circle(double x, double y,
        double radius,
        double offsetX, double offsetY,
        string layer)
    {
        double _x = X(x + offsetX);
        double _y = Y(y + offsetY);

        double thickness = 0;

        var circle = new DxfCircle(Version, GetNextHandle())
            .Layer(layer)
            .Color(DxfDefaultColors.ByLayer.ColorToString())
            .Thickness(thickness)
            .Radius(radius)
            .Center(new Vector3(_x, _y, 0));

        return circle;
    }

    private DxfAttdef AttdefTable(string tag, double x, double y, 
        string defaultValue,
        bool isVisible,
        DxfHorizontalTextJustification horizontalTextJustification,
        DxfVerticalTextJustification verticalTextJustification)
    {
        var attdef = new DxfAttdef(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerTable,
            Color = DxfDefaultColors.ByLayer.ColorToString(),
            FirstAlignment = new Vector3(X(x), Y(y), 0),
            TextHeight = 6,
            DefaultValue = defaultValue,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextTableTag",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = horizontalTextJustification,
            SecondAlignment = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Prompt = tag,
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = verticalTextJustification
        };

        return attdef.Create();
    }

    private DxfAttrib AttribTable(string tag, string text,
        double x, double y,
        bool isVisible,
        DxfHorizontalTextJustification horizontalTextJustification,
        DxfVerticalTextJustification verticalTextJustification)
    {
        var attrib = new DxfAttrib(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerTable,
            StartPoint = new Vector3(X(x), Y(y), 0),
            TextHeight = 6,
            DefaultValue = text,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextTableTag",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = horizontalTextJustification,
            AlignmentPoint = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = DxfVerticalTextJustification.Middle
        };

        return attrib.Create();
    }

    private DxfAttdef AttdefIO(string tag, double x, double y, 
        string defaultValue, bool isVisible)
    {
        var attdef = new DxfAttdef(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerIO,
            Color = DxfDefaultColors.ByLayer.ColorToString(),
            FirstAlignment = new Vector3(X(x), Y(y), 0),
            TextHeight = 6,
            DefaultValue = defaultValue,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextElementIO",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = DxfHorizontalTextJustification.Left,
            SecondAlignment = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Prompt = tag,
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = DxfVerticalTextJustification.Middle
        };

        return attdef.Create();
    }

    private DxfAttrib AttribIO(string tag, string text,
        double x, double y,
        bool isVisible)
    {
        var attrib = new DxfAttrib(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerIO,
            StartPoint = new Vector3(X(x), Y(y), 0),
            TextHeight = 6,
            DefaultValue = text,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextElementIO",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = DxfHorizontalTextJustification.Left,
            AlignmentPoint = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = DxfVerticalTextJustification.Middle
        };

        return attrib.Create();
    }

    private DxfAttdef AttdefGate(string tag, double x, double y, 
        string defaultValue, bool isVisible)
    {
        var attdef = new DxfAttdef(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerElements,
            Color = DxfDefaultColors.ByLayer.ColorToString(),
            FirstAlignment = new Vector3(X(x), Y(y), 0),
            TextHeight = 10,
            DefaultValue = defaultValue,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextElementGate",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = DxfHorizontalTextJustification.Center,
            SecondAlignment = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Prompt = tag,
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = DxfVerticalTextJustification.Middle
        };

        return attdef.Create();
    }

    private DxfAttrib AttribGate(string tag, string text,
        double x, double y,
        bool isVisible)
    {
        var attrib = new DxfAttrib(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = LayerElements,
            StartPoint = new Vector3(X(x), Y(y), 0),
            TextHeight = 10,
            DefaultValue = text,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = "TextElementGate",
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = DxfHorizontalTextJustification.Center,
            AlignmentPoint = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            Tag = tag,
            AttributeFlags = isVisible ? DxfAttributeFlags.Default : DxfAttributeFlags.Invisible,
            FieldLength = 0,
            VerticalTextJustification = DxfVerticalTextJustification.Middle
        };

        return attrib.Create();
    }

    private DxfText Text(string text, 
        string style,
        string layer,
        double height,
        double x, double y,
        DxfHorizontalTextJustification horizontalJustification,
        DxfVerticalTextJustification verticalJustification)
    {
        var txt = new DxfText(Version, GetNextHandle())
        {
            Thickness = 0,
            Layer = layer,
            Color = DxfDefaultColors.ByLayer.ColorToString(),
            FirstAlignment = new Vector3(X(x), Y(y), 0),
            TextHeight = height,
            DefaultValue = text,
            TextRotation = 0,
            ScaleFactorX = 1,
            ObliqueAngle = 0,
            TextStyle = style,
            TextGenerationFlags = DxfTextGenerationFlags.Default,
            HorizontalTextJustification = horizontalJustification,
            SecondAlignment = new Vector3(X(x), Y(y), 0),
            ExtrusionDirection = new Vector3(0, 0, 1),
            VerticalTextJustification = verticalJustification
        };

        return txt.Create();
    }

    private DxfBlockRecord CreateBlockRecordForBlock(string name)
    {
        var blockRecord = new DxfBlockRecord(Version, GetNextHandle())
        {
            Name = name
        };

        return blockRecord.Create();
    }

    private IEnumerable<DxfAppid> TableAppids()
    {
        var appids = new List<DxfAppid>();

        // ACAD - default must be present
        if (Version > DxfAcadVer.AC1009)
        {
            var acad = new DxfAppid(Version, GetNextHandle())
                .Application("ACAD")
                .StandardFlags(DxfAppidStandardFlags.Default);

            appids.Add(acad);
        }

        // TestDXF
        var cade = new DxfAppid(Version, GetNextHandle())
            .Application("TestDXF")
            .StandardFlags(DxfAppidStandardFlags.Default);

        appids.Add(cade);

        return appids;
    }

    private IEnumerable<DxfDimstyle> TableDimstyles()
    {
        var dimstyles = new List<DxfDimstyle>();

        if (Version > DxfAcadVer.AC1009)
        {
            dimstyles.Add(new DxfDimstyle(Version, GetNextHandle())
            {
                Name = "Standard"
            }.Create()); 
        }

        return dimstyles;
    }
        
    private IEnumerable<DxfLayer> TableLayers()
    {
        var layers = new List<DxfLayer>();

        // default layer 0 - must be present
        if (Version > DxfAcadVer.AC1009)
        {
            layers.Add(new DxfLayer(Version, GetNextHandle())
            {
                Name = "0",
                LayerStandardFlags = DxfLayerStandardFlags.Default,
                Color = DxfDefaultColors.Default.ColorToString(),
                LineType = "Continuous",
                PlottingFlag = true,
                LineWeight = DxfLineWeight.LnWtByLwDefault,
                PlotStyleNameHandle = "0"
            }.Create());
        }

        // layer: FRAME
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerFrame,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = 250.ToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt013,
            PlotStyleNameHandle = "0"
        }.Create());

        // layer: GRID
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerGrid,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = 251.ToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt013,
            PlotStyleNameHandle = "0"
        }.Create());

        // layer: TABLE
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerTable,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = 250.ToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt013,
            PlotStyleNameHandle = "0"
        }.Create());

        // layer: IO
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerIO,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = DxfDefaultColors.Default.ColorToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt025,
            PlotStyleNameHandle = "0"
        }.Create());

        // layer: WIRES
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerWires,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = DxfDefaultColors.Default.ColorToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt018,
            PlotStyleNameHandle = "0"
        }.Create());

        // layer: ELEMENTS
        layers.Add(new DxfLayer(Version, GetNextHandle())
        {
            Name = LayerElements,
            LayerStandardFlags = DxfLayerStandardFlags.Default,
            Color = DxfDefaultColors.Default.ColorToString(),
            LineType = "Continuous",
            PlottingFlag = true,
            LineWeight = DxfLineWeight.LnWt035,
            PlotStyleNameHandle = "0"
        }.Create());

        return layers;
    }

    private IEnumerable<DxfLtype> TableLtypes()
    {
        var ltypes = new List<DxfLtype>();

        // default ltypes ByLayer, ByBlock and Continuous - must be present

        // ByLayer
        ltypes.Add(new DxfLtype(Version, GetNextHandle())
        {
            Name = "ByLayer",
            LtypeStandardFlags = DxfLtypeStandardFlags.Default,
            Description = "ByLayer",
            DashLengthItems = 0,
            TotalPatternLenght = 0,
            DashLenghts = null,
        }.Create());

        // ByBlock
        ltypes.Add(new DxfLtype(Version, GetNextHandle())
        {
            Name = "ByBlock",
            LtypeStandardFlags = DxfLtypeStandardFlags.Default,
            Description = "ByBlock",
            DashLengthItems = 0,
            TotalPatternLenght = 0,
            DashLenghts = null,
        }.Create());

        // Continuous
        ltypes.Add(new DxfLtype(Version, GetNextHandle())
        {
            Name = "Continuous",
            LtypeStandardFlags = DxfLtypeStandardFlags.Default,
            Description = "Solid line",
            DashLengthItems = 0,
            TotalPatternLenght = 0,
            DashLenghts = null,
        }.Create());

        return ltypes;
    }

    private IEnumerable<DxfStyle> TableStyles()
    {
        var styles = new List<DxfStyle>();

        // style: Standard
        var standard = new DxfStyle(Version, GetNextHandle())
            .Name("Standard")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            standard.Add(1001, "ACAD");
            standard.Add(1000, StylePrimatyFontDescription);
            standard.Add(1071, 0);
        }

        styles.Add(standard);

        // style: TextFrameHeaderSmall
        var textFrameHeaderSmall = new DxfStyle(Version, GetNextHandle())
            .Name("TextFrameHeaderSmall")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textFrameHeaderSmall.Add(1001, "ACAD");
            textFrameHeaderSmall.Add(1000, StylePrimatyFontDescription);
            textFrameHeaderSmall.Add(1071, 0);
        }

        styles.Add(textFrameHeaderSmall);

        // style: TextFrameHeaderLarge
        var textFrameHeaderLarge = new DxfStyle(Version, GetNextHandle())
            .Name("TextFrameHeaderLarge")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textFrameHeaderLarge.Add(1001, "ACAD");
            textFrameHeaderLarge.Add(1000, StylePrimatyFontDescription);
            textFrameHeaderLarge.Add(1071, 0);
        }

        styles.Add(textFrameHeaderLarge);

        // style: TextFrameNumber
        var textFrameNumber = new DxfStyle(Version, GetNextHandle())
            .Name("TextFrameNumber")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textFrameNumber.Add(1001, "ACAD");
            textFrameNumber.Add(1000, StylePrimatyFontDescription);
            textFrameNumber.Add(1071, 0);
        }

        styles.Add(textFrameNumber);

        // style: TextTableHeader
        var textTableHeader = new DxfStyle(Version, GetNextHandle())
            .Name("TextTableHeader")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textTableHeader.Add(1001, "ACAD");
            textTableHeader.Add(1000, StylePrimatyFontDescription);
            textTableHeader.Add(1071, 0);
        }

        styles.Add(textTableHeader);

        // style: TextTableTag
        var textTableTag = new DxfStyle(Version, GetNextHandle())
            .Name("TextTableTag")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textTableTag.Add(1001, "ACAD");
            textTableTag.Add(1000, StylePrimatyFontDescription);
            textTableTag.Add(1071, 0);
        }

        styles.Add(textTableTag);

        // style: TextElementGate
        var textElementGate = new DxfStyle(Version, GetNextHandle())
            .Name("TextElementGate")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textElementGate.Add(1001, "ACAD");
            textElementGate.Add(1000, StylePrimatyFontDescription);
            textElementGate.Add(1071, 0);
        }

        styles.Add(textElementGate);

        // style: TextElementIO
        var textElementIO = new DxfStyle(Version, GetNextHandle())
            .Name("TextElementIO")
            .StandardFlags(DxfStyleFlags.Default)
            .FixedTextHeight(0)
            .WidthFactor(1)
            .ObliqueAngle(0)
            .TextGenerationFlags(DxfTextGenerationFlags.Default)
            .LastHeightUsed(1)
            .PrimaryFontFile(StylePrimatyFont)
            .BifFontFile(StyleBigFont);

        if (Version > DxfAcadVer.AC1009)
        {
            // extended STYLE data
            textElementIO.Add(1001, "ACAD");
            textElementIO.Add(1000, StylePrimatyFontDescription);
            textElementIO.Add(1071, 0);
        }

        styles.Add(textElementIO);

        return styles;
    }

    private IEnumerable<DxfUcs> TableUcss()
    {
        return Enumerable.Empty<DxfUcs>();
    }

    private IEnumerable<DxfView> TableViews()
    {
        var view = new DxfView(Version, GetNextHandle())
            .Name("DIAGRAM")
            .StandardFlags(DxfViewStandardFlags.Default)
            .Height(PageHeight)
            .Width(PageWidth)
            .Center(new Vector2(PageWidth / 2, PageHeight / 2))
            .ViewDirection(new Vector3(0, 0, 1))
            .TargetPoint(new Vector3(0, 0, 0))
            .FrontClippingPlane(0)
            .BackClippingPlane(0)
            .Twist(0);

        yield return view;
    }

    private IEnumerable<DxfVport> TableVports()
    {
        var vports = new List<DxfVport>();

        if (Version > DxfAcadVer.AC1009)
        {
            vports.Add(new DxfVport(Version, GetNextHandle())
            {
                Name = "*Active"
            }.Create()); 
        }

        return vports;
    }

    public IEnumerable<DxfBlock> DefaultBlocks()
    {
        if (Version > DxfAcadVer.AC1009)
        {
            var blocks = new List<DxfBlock>();
            string layer = "0";

            blocks.Add(new DxfBlock(Version, GetNextHandle())
            {
                Name = "*Model_Space",
                Layer = layer,
                BlockTypeFlags = DxfBlockTypeFlags.Default,
                BasePoint = new Vector3(0, 0, 0),
                XrefPathName = null,
                Description = null,
                EndId = GetNextHandle(),
                EndLayer = layer,
                Entities = null
            }.Create());

            blocks.Add(new DxfBlock(Version, GetNextHandle())
            {
                Name = "*Paper_Space",
                Layer = layer,
                BlockTypeFlags = DxfBlockTypeFlags.Default,
                BasePoint = new Vector3(0, 0, 0),
                XrefPathName = null,
                Description = null,
                EndId = GetNextHandle(),
                EndLayer = layer,
                Entities = null
            }.Create());

            blocks.Add(new DxfBlock(Version, GetNextHandle())
            {
                Name = "*Paper_Space0",
                Layer = layer,
                BlockTypeFlags = DxfBlockTypeFlags.Default,
                BasePoint = new Vector3(0, 0, 0),
                XrefPathName = null,
                Description = null,
                EndId = GetNextHandle(),
                EndLayer = layer,
                Entities = null
            }.Create());

            return blocks;
        }

        return Enumerable.Empty<DxfBlock>();
    }

    public DxfBlock BlockOutput()
    {
        var block = new DxfBlock(Version, GetNextHandle())
        {
            Name = "OUTPUT",
            Layer = LayerIO,
            BlockTypeFlags = DxfBlockTypeFlags.NonConstantAttributes,
            BasePoint = new Vector3(0, 0, 0),
            XrefPathName = null,
            Description = "Output Signal",
            EndId = GetNextHandle(),
            EndLayer = LayerIO,
            Entities = new List<object>()
        };

        var entities = block.Entities;

        entities.Add(Line(0, 0, 300, 0, 0, 0, LayerIO));
        entities.Add(Line(300, 30, 0, 30, 0, 0, LayerIO));
        entities.Add(Line(0, 30, 0, 0, 0, 0, LayerIO));
        entities.Add(Line(210, 0, 210, 30, 0, 0, LayerIO));
        entities.Add(Line(300, 30, 300, 0, 0, 0, LayerIO));

        double offsetX = 3;

        entities.Add(AttdefIO("ID", 300 + offsetX, 30, "ID", false));
        entities.Add(AttdefIO("TAGID", 300 + offsetX, 0, "TAGID", false));
        entities.Add(AttdefIO("DESIGNATION", offsetX, 7.5, "DESIGNATION", true));
        entities.Add(AttdefIO("DESCRIPTION", offsetX, 22.5, "DESCRIPTION", true));
        entities.Add(AttdefIO("SIGNAL", 210 + offsetX, 7.5, "SIGNAL", true));
        entities.Add(AttdefIO("CONDITION", 210 + offsetX, 22.5, "CONDITION", true));

        return block.Create();
    }

    public DxfBlock BlockOrGate()
    {
        var block = new DxfBlock(Version, GetNextHandle())
        {
            Name = "ORGATE",
            Layer = LayerElements,
            BlockTypeFlags = DxfBlockTypeFlags.NonConstantAttributes,
            BasePoint = new Vector3(0, 0, 0),
            XrefPathName = null,
            Description = "OR Gate",
            EndId = GetNextHandle(),
            EndLayer = LayerElements,
            Entities = new List<object>()
        };

        var entities = block.Entities;

        entities.Add(Line(0, 0, 30, 0, 0, 0, LayerElements));
        entities.Add(Line(0, 30, 30, 30, 0, 0, LayerElements));
        entities.Add(Line(0, 0, 0, 30, 0, 0, LayerElements));
        entities.Add(Line(30, 0, 30, 30, 0, 0, LayerElements));

        entities.Add(AttdefGate("ID", 30, 30, "ID", false));
        entities.Add(AttdefGate("TEXT", 15, 15, "\\U+22651", true));

        return block.Create();
    }

    public DxfInsert CreateFrame(double x, double y)
    {
        var frame = new DxfInsert(Version, GetNextHandle())
            .Block("FRAME")
            .Layer(LayerFrame)
            .Insertion(new Vector3(X(x), Y(y), 0));

        return frame;
    }

    public DxfInsert CreateGrid(double x, double y)
    {
        var frame = new DxfInsert(Version, GetNextHandle())
            .Block("GRID")
            .Layer(LayerGrid)
            .Insertion(new Vector3(X(x), Y(y), 0));

        return frame;
    }

    public string GenerateDxf(string model, DxfAcadVer version)
    {
        this.Version = version;

        ResetHandleCounter();

        // initialize parser
        var parser = new Parser();
        var parseOptions = DefaultParseOptions();

        // dxf file sections
        DxfHeader header = null;
        DxfClasses classes = null;
        DxfTables tables = null;
        DxfBlocks blocks = null;
        DxfObjects objects = null;

        // create dxf file
        var dxf = new DxfFile(Version, GetNextHandle());
            
        // create header
        header = new DxfHeader(Version, GetNextHandle()).Begin().Default();

        // create classes
        if (Version > DxfAcadVer.AC1009)
        {
            classes = new DxfClasses(Version, GetNextHandle())
                .Begin();

            // classes.Add(new DxfClass(...));

            classes.End();
        }

        // create tables
        tables = new DxfTables(Version, GetNextHandle());

        tables.Begin();
        tables.AddAppidTable(TableAppids(), GetNextHandle());
        tables.AddDimstyleTable(TableDimstyles(), GetNextHandle());

        if (Version > DxfAcadVer.AC1009)
        {
            var records = new List<DxfBlockRecord>();

            // TODO: each BLOCK must have BLOCK_RECORD entry

            // required block records by dxf format
            records.Add(CreateBlockRecordForBlock("*Model_Space"));
            records.Add(CreateBlockRecordForBlock("*Paper_Space"));
            records.Add(CreateBlockRecordForBlock("*Paper_Space0"));

            // canvas Diagram block records
            records.Add(CreateBlockRecordForBlock("FRAME"));
            records.Add(CreateBlockRecordForBlock("TABLE"));
            records.Add(CreateBlockRecordForBlock("GRID"));
            records.Add(CreateBlockRecordForBlock("INPUT"));
            records.Add(CreateBlockRecordForBlock("OUTPUT"));
            records.Add(CreateBlockRecordForBlock("ANDGATE"));
            records.Add(CreateBlockRecordForBlock("ORGATE"));

            tables.AddBlockRecordTable(records, GetNextHandle());
        }

        tables.AddLtypeTable(TableLtypes(), GetNextHandle());
        tables.AddLayerTable(TableLayers(), GetNextHandle());
        tables.AddStyleTable(TableStyles(), GetNextHandle());
        tables.AddUcsTable(TableUcss(), GetNextHandle());
        tables.AddViewTable(TableViews(), GetNextHandle());
        tables.AddVportTable(TableVports(), GetNextHandle());

        tables.End();

        // create blocks
        blocks = new DxfBlocks(Version, GetNextHandle())
            .Begin()
            .Add(DefaultBlocks())
            .Add(BlockFrame())
            .Add(BlockTable())
            .Add(BlockGrid())
            .Add(BlockInput())
            .Add(BlockOutput())
            .Add(BlockAndGate())
            .Add(BlockOrGate())
            .End();

        // create entities
        Entities = new DxfEntities(Version, GetNextHandle())
            .Begin()
            .Add(CreateFrame(0, 0))
            .Add(CreateGrid(330, 31))
            .Add(CreateTable(1, 810, table));

        parser.Parse(model, this, parseOptions);

        Entities.End();

        // create objects
        if (Version > DxfAcadVer.AC1009)
        {
            objects = new DxfObjects(Version, GetNextHandle()).Begin();

            // mamed dictionary
            var namedDict = new DxfDictionary(Version, GetNextHandle())
            {
                OwnerDictionaryHandle = 0.ToDxfHandle(),
                HardOwnerFlag = false,
                DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting,
                Entries = new Dictionary<string, string>()
            };

            // base dictionary
            var baseDict = new DxfDictionary(Version, GetNextHandle())
            {
                OwnerDictionaryHandle = namedDict.Id.ToDxfHandle(),
                HardOwnerFlag = false,
                DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting,
                Entries = new Dictionary<string, string>()
            };

            // add baseDict to namedDict
            namedDict.Entries.Add(baseDict.Id.ToDxfHandle(), "ACAD_GROUP");

            // TODO: add more named object dictionaries

            // finalize dictionaries
            objects.Add(namedDict.Create());
            objects.Add(baseDict.Create());

            // finalize objects
            objects.End();
        }

        // finalize dxf file

        dxf.Header(header.End(GetNextHandle()));

        if (Version > DxfAcadVer.AC1009)
            dxf.Classes(classes);

        dxf.Tables(tables);
        dxf.Blocks(blocks);
        dxf.Entities(Entities);

        if (Version > DxfAcadVer.AC1009)
            dxf.Objects(objects);

        dxf.Eof();

        // return dxf file contents
        return dxf.ToString();
    }
    */
}
