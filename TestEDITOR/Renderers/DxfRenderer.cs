// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dxf;
using Test2d;

namespace TestDXF
{
    public class DxfRenderer : ObservableObject, IRenderer
    {
        private double _zoom;
        private ShapeState _drawShapeState;
        private BaseShape _selectedShape;
        private ICollection<BaseShape> _selectedShapes;

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value != _zoom)
                {
                    _zoom = value;
                    Notify("Zoom");
                }
            }
        }

        public ShapeState DrawShapeState
        {
            get { return _drawShapeState; }
            set
            {
                if (value != _drawShapeState)
                {
                    _drawShapeState = value;
                    Notify("DrawShapeState");
                }
            }
        }

        public BaseShape SelectedShape
        {
            get { return _selectedShape; }
            set
            {
                if (value != _selectedShape)
                {
                    _selectedShape = value;
                    Notify("SelectedShape");
                }
            }
        }

        public ICollection<BaseShape> SelectedShapes
        {
            get { return _selectedShapes; }
            set
            {
                if (value != _selectedShapes)
                {
                    _selectedShapes = value;
                    Notify("SelectedShapes");
                }
            }
        }

        public DxfRenderer()
        {
            _zoom = 1.0;
            _drawShapeState = ShapeState.Visible | ShapeState.Printable;
            _selectedShape = null;
            _selectedShapes = null;

            ClearCache();
        }

        public static IRenderer Create()
        {
            return new DxfRenderer();
        }

        public void ClearCache()
        {
        }

        public void Draw(object dc, Container container)
        {
        }

        public void Draw(object dc, Layer layer)
        {
        }

        public void Draw(object dc, XLine line, double dx, double dy)
        {
        }

        public void Draw(object dc, XRectangle rectangle, double dx, double dy)
        {
        }

        public void Draw(object dc, XEllipse ellipse, double dx, double dy)
        {
        }

        public void Draw(object dc, XArc arc, double dx, double dy)
        {
        }

        public void Draw(object dc, XBezier bezier, double dx, double dy)
        {
        }

        public void Draw(object dc, XQBezier qbezier, double dx, double dy)
        {
        }

        public void Draw(object dc, XText text, double dx, double dy)
        {
        }

        private DxfAcadVer _version;
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

        private void TableAppids(DxfTable<DxfAppid> appids)
        {
            // NOTE: Appid "ACAD" - default must be present
            if (_version > DxfAcadVer.AC1009)
            {
                appids.Items.Add(new DxfAppid(_version, NextHandle())
                {
                    ApplicationName = "ACAD",
                    AppidStandardFlags = DxfAppidStandardFlags.Default
                });
            }
        }

        private void TableDimstyles(DxfTable<DxfDimstyle> dimstyles)
        {
            if (_version > DxfAcadVer.AC1009)
            {
                dimstyles.Items.Add(new DxfDimstyle(_version, NextHandle())
                {
                    Name = "Standard"
                });
            }
        }

        private void TableLayers(DxfTable<DxfLayer> layers, Container container)
        {
            // NOTE: Default layer "0" - must be present.
            if (_version > DxfAcadVer.AC1009)
            {
                layers.Items.Add(new DxfLayer(_version, NextHandle())
                {
                    Name = "0",
                    LayerStandardFlags = DxfLayerStandardFlags.Default,
                    Color = DxfDefaultColors.Default.ToDxfColor(),
                    LineType = "Continuous",
                    PlottingFlag = true,
                    LineWeight = DxfLineWeight.LnWtByLwDefault,
                    PlotStyleNameHandle = "0"
                });
            }

            if (container.TemplateLayer.IsVisible)
            {
                layers.Items.Add(new DxfLayer(_version, NextHandle())
                {
                    Name = container.TemplateLayer.Name,
                    LayerStandardFlags = DxfLayerStandardFlags.Default,
                    Color = DxfDefaultColors.Default.ToDxfColor(),
                    LineType = "Continuous",
                    PlottingFlag = true,
                    LineWeight = DxfLineWeight.LnWtByLwDefault,
                    PlotStyleNameHandle = "0"
                });
            }
            
            foreach (var layer in container.Layers) 
            {
                if (layer.IsVisible)
                {
                    layers.Items.Add(new DxfLayer(_version, NextHandle())
                    {
                        Name = layer.Name,
                        LayerStandardFlags = DxfLayerStandardFlags.Default,
                        Color = DxfDefaultColors.Default.ToDxfColor(),
                        LineType = "Continuous",
                        PlottingFlag = true,
                        LineWeight = DxfLineWeight.LnWtByLwDefault,
                        PlotStyleNameHandle = "0"
                    });
                }
            }
        }

        private void TableLtypes(DxfTable<DxfLtype> ltypes)
        {
            // NOTE: Default ltypes ByLayer, ByBlock and Continuous - must be present.

            // ByLayer
            ltypes.Items.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "ByLayer",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "ByLayer",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            });

            // ByBlock
            ltypes.Items.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "ByBlock",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "ByBlock",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            });

            // Continuous
            ltypes.Items.Add(new DxfLtype(_version, NextHandle())
            {
                Name = "Continuous",
                LtypeStandardFlags = DxfLtypeStandardFlags.Default,
                Description = "Solid line",
                DashLengthItems = 0,
                TotalPatternLength = 0,
                DashLengths = null,
            });
        }

        private void TableStyles(DxfTable<DxfStyle> styles)
        {
            // style: Standard
            styles.Items.Add(new DxfStyle(_version, NextHandle())
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
            });
        }

        private void TableUcss(DxfTable<DxfUcs> ucss)
        {
            // NOTE: Currently not used.
        }

        private void TableViews(DxfTable<DxfView> views)
        {
            views.Items.Add(new DxfView(_version, NextHandle())
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
            });
        }

        private void TableVports(DxfTable<DxfVport> vports)
        {
            if (_version > DxfAcadVer.AC1009)
            {
                vports.Items.Add(new DxfVport(_version, NextHandle())
                {
                    Name = "*Active"
                });
            }
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
                });

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
                });

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
                });

                return blocks;
            }

            return Enumerable.Empty<DxfBlock>();
        }

        private DxfBlockRecord CreateBlockRecordForBlock(string name)
        {
            return new DxfBlockRecord(_version, NextHandle())
            {
                Name = name
            };
        }

        private DxfLine CreateLine(double x1, double y1, double x2, double y2, string layer)
        {
            double _x1 = ToDxfX(x1);
            double _y1 = ToDxfY(y1);
            double _x2 = ToDxfX(x2);
            double _y2 = ToDxfY(y2);

            return new DxfLine(_version, NextHandle())
            {
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                Thickness = 0.0,
                StartPoint = new DxfVector3(_x1, _y1, 0),
                EndPoint = new DxfVector3(_x2, _y2, 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1)
            };
        }

        private DxfCircle CreateCircle(double cx, double cy, double radius, string layer)
        {
            double _cx = ToDxfX(cx);
            double _cy = ToDxfY(cy);

            return new DxfCircle(_version, NextHandle())
            {
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                Thickness = 0.0,
                CenterPoint = new DxfVector3(_cx, _cy, 0),
                Radius = radius,
                ExtrusionDirection = new DxfVector3(0, 0, 1),
            };
        }

        private DxfEllipse CreateEllipse(double x, double y, double width, double height, string layer)
        {
            double _cx = ToDxfX(x + width / 2.0);
            double _cy = ToDxfY(y + height / 2.0);
            double minor = Math.Min(height, width);
            double major = Math.Max(height, width);
            double _ex = width >= height ? major / 2.0 : 0.0; // relative to _cx
            double _ey = width < height ? major / 2.0 : 0.0; // relative to _cy

            return new DxfEllipse(_version, NextHandle())
            {
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                CenterPoint = new DxfVector3(_cx, _cy, 0),
                EndPoint = new DxfVector3(_ex, _ey, 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1),
                Ratio = minor / major,
                StartParameter = 0.0,
                EndParameter = 2.0 * Math.PI
            };
        }

        private DxfArc CreateArc(
            double x, double y,
            double radius, 
            double startAngle, double endAngle, 
            string layer)
        {
            double _cx = ToDxfX(x + radius / 2.0);
            double _cy = ToDxfY(y + radius / 2.0);

            return new DxfArc(_version, NextHandle())
            {
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                Thickness = 0.0,
                CenterPoint = new DxfVector3(_cx, _cy, 0),
                Radius = radius,
                StartAngle = startAngle,
                EndAngle = endAngle,
                ExtrusionDirection = new DxfVector3(0, 0, 1),
            };
        }
        
        private DxfSpline CreateSpline(
            double p1x, double p1y, 
            double p2x, double p2y,
            double p3x, double p3y,
            double p4x, double p4y, 
            string layer)
        {
            double _p1x  = ToDxfX(p1x);
            double _p1y  = ToDxfY(p1y);
            double _p2x  = ToDxfX(p2x);
            double _p2y  = ToDxfY(p2y);
            double _p3x  = ToDxfX(p3x);
            double _p3y  = ToDxfY(p3y);
            double _p4x  = ToDxfX(p4x);
            double _p4y  = ToDxfY(p4y);
            
            var spline = new DxfSpline(_version, NextHandle())
            {
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                NormalVector = new DxfVector3(0.0, 0.0, 1.0),
                SplineFlags = DxfSplineFlags.Planar,
                SplineCurveDegree = 3,
                KnotTolerance =  0.0000001,
                ControlPointTolerance = 0.0000001,
                FitTolerance = 0.0000000001,
                StartTangent = null,
                EndTangent = null,
                Knots = new double[8],
                Weights = null,
                ControlPoints = new DxfVector3[4],
                FitPoints = null
            };
     
            spline.Knots[0] = 0.0;
            spline.Knots[1] = 0.0;
            spline.Knots[2] = 0.0;
            spline.Knots[3] = 0.0;
            
            spline.Knots[4] = 1.0;
            spline.Knots[5] = 1.0;
            spline.Knots[6] = 1.0;
            spline.Knots[7] = 1.0;
            
            spline.ControlPoints[0] = new DxfVector3(_p1x, _p1y, 0.0);
            spline.ControlPoints[1] = new DxfVector3(_p2x, _p2y, 0.0);
            spline.ControlPoints[2] = new DxfVector3(_p3x, _p3y, 0.0);
            spline.ControlPoints[3] = new DxfVector3(_p4x, _p4y, 0.0);
            
            return spline;
        }

        private DxfText CreateText(
            string text, 
            double x, double y, 
            double height, 
            DxfHorizontalTextJustification horizontalJustification, 
            DxfVerticalTextJustification verticalJustification, 
            string style, 
            string layer)
        {
            return new DxfText(_version, NextHandle())
            {
                Thickness = 0,
                Layer = layer,
                Color = DxfDefaultColors.ByLayer.ToDxfColor(),
                FirstAlignment = new DxfVector3(ToDxfX(x), ToDxfY(y), 0),
                TextHeight = height,
                DefaultValue = text.ToDxfText(_version),
                TextRotation = 0,
                ScaleFactorX = 1,
                ObliqueAngle = 0,
                TextStyle = style,
                TextGenerationFlags = DxfTextGenerationFlags.Default,
                HorizontalTextJustification = horizontalJustification,
                SecondAlignment = new DxfVector3(ToDxfX(x), ToDxfY(y), 0),
                ExtrusionDirection = new DxfVector3(0, 0, 1),
                VerticalTextJustification = verticalJustification
            };
        }

        private void DrawLine(DxfEntities entities, XLine line, string layer)
        {
            entities.Entities.Add(CreateLine(line.Start.X, line.Start.Y, line.End.X, line.End.Y, layer));
        }

        private void DrawRectangle(DxfEntities entities, XRectangle rectangle, string layer)
        {
            var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
            entities.Entities.Add(CreateLine(rect.X, rect.Y, rect.X + rect.Width, rect.Y, layer));
            entities.Entities.Add(CreateLine(rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height, layer));
            entities.Entities.Add(CreateLine(rect.X, rect.Y, rect.X, rect.Y + rect.Height, layer));
            entities.Entities.Add(CreateLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, layer));
        }

        private void DrawEllipse(DxfEntities entities, XEllipse ellipse, string layer)
        {
            var rect = Rect2.Create(ellipse.TopLeft, ellipse.BottomRight);
            entities.Entities.Add(CreateEllipse(rect.X, rect.Y, rect.Width, rect.Height, layer));
        }

        private void DrawArc(DxfEntities entities, XArc arc, string layer)
        {
            var a = Arc.FromXArc(arc, 0.0, 0.0);
            entities.Entities.Add(CreateArc(a.X, a.Y, a.Radius, a.StartAngle, a.EndAngle, layer));
        }

        private void DrawBezier(DxfEntities entities, XBezier bezier, string layer)
        {
            entities.Entities.Add(
                CreateSpline(
                    bezier.Point1.X, bezier.Point1.Y, 
                    bezier.Point2.X, bezier.Point2.Y, 
                    bezier.Point3.X, bezier.Point3.Y, 
                    bezier.Point4.X, bezier.Point4.Y,
                    layer));
        }

        private void DrawQBezier(DxfEntities entities, XQBezier qbezier, string layer)
        {
            double x1 = qbezier.Point1.X;
            double y1 = qbezier.Point1.Y;
            double x2 = qbezier.Point1.X + (2.0 * (qbezier.Point2.X - qbezier.Point1.X)) / 3.0;
            double y2 = qbezier.Point1.Y + (2.0 * (qbezier.Point2.Y - qbezier.Point1.Y)) / 3.0;
            double x3 = x2 + (qbezier.Point3.X - qbezier.Point1.X) / 3.0;
            double y3 = y2 + (qbezier.Point3.Y - qbezier.Point1.Y) / 3.0;
            double x4 = qbezier.Point3.X;
            double y4 = qbezier.Point3.Y;

            entities.Entities.Add(
                CreateSpline(
                    x1, y1, 
                    x2, y2, 
                    x3, y3, 
                    x4, y4,
                    layer));
        }

        private void DrawText(DxfEntities entities, XText text, string layer)
        {
            DxfHorizontalTextJustification halign;
            DxfVerticalTextJustification valign;
            double x, y;

            var rect = Rect2.Create(text.TopLeft, text.BottomRight);
            
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

            entities.Entities.Add(CreateText(
                text.Text, 
                x, y, 
                text.Style.TextStyle.FontSize * (72.0 / 96.0), 
                halign, 
                valign, 
                _defaultStyle,
               layer));
        }

        private void DrawShapes(DxfEntities entities, IEnumerable<BaseShape> shapes, string layer)
        {
            foreach (var shape in shapes) 
            {
                if (shape.State.HasFlag(DrawShapeState))
                {
                    if (shape is XPoint)
                    {
                        var point = shape as XPoint;
                        // TODO: Draw point.
                    }
                    else if (shape is XLine)
                    {
                        var line = shape as XLine;
                        DrawLine(entities, line, layer);
                        // TODO: Draw start and end arrows.
                    }
                    else if (shape is XRectangle)
                    {
                        var rectangle = shape as XRectangle;
                        DrawRectangle(entities, rectangle, layer);
                    }
                    else if (shape is XEllipse)
                    {
                        var ellipse = shape as XEllipse;
                        if (_version <= DxfAcadVer.AC1009)
                        {
                            // try to use circle for ellipse
                            var rect = Rect2.Create(ellipse.TopLeft, ellipse.BottomRight);
                            if (rect.Width == rect.Height)
                            {
                                double radius = rect.Width / 2.0;
                                double cx = rect.X + radius;
                                double cy = rect.Y + radius;
                                entities.Entities.Add(CreateCircle(cx, cy, radius, layer));
                            }
                        }
                        else
                        {
                            DrawEllipse(entities, ellipse, layer);
                        }
                    }
                    else if (shape is XArc)
                    {
                        var arc = shape as XArc;
                        DrawArc(entities, arc, layer);
                    }
                    else if (shape is XBezier)
                    {
                        var bezier = shape as XBezier;
                        DrawBezier(entities, bezier, layer);
                    }
                    else if (shape is XQBezier)
                    {
                        var qbezier = shape as XQBezier;
                        DrawQBezier(entities, qbezier, layer);
                    }
                    else if (shape is XText)
                    {
                        var text = shape as XText;
                        DrawText(entities, text, layer);
                    }
                    else if (shape is XGroup)
                    {
                        var group = shape as XGroup;
                        DrawShapes(entities, group.Shapes, layer);
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

        public void Create(string path, Container container, DxfAcadVer version)
        {
            _version = version;

            _pageWidth = container.Width;
            _pageHeight = container.Height;

            //_layer = "0";
            _handle = 0;

            // create dxf file
            var file = new DxfFile(_version, NextHandle());

            // create header
            file.Header = new DxfHeader(_version, NextHandle());

            // create classes
            if (_version > DxfAcadVer.AC1009)
            {
                file.Classes = new DxfClasses(_version, NextHandle());

                // classes.Classes.Add(new DxfClass(...));

                // end of classes
            }

            // create tables
            file.Tables = new DxfTables(_version, NextHandle());

            file.Tables.AppidTable.Id = NextHandle();
            TableAppids(file.Tables.AppidTable);

            file.Tables.DimstyleTable.Id = NextHandle();
            TableDimstyles(file.Tables.DimstyleTable);

            file.Tables.BlockRecordTable.Id = NextHandle();
            if (_version > DxfAcadVer.AC1009)
            {
                // NOTE: Required block records by Dxf format.
                file.Tables.BlockRecordTable.Items.Add(
                    CreateBlockRecordForBlock("*Model_Space"));

                file.Tables.BlockRecordTable.Items.Add(
                    CreateBlockRecordForBlock("*Paper_Space"));

                file.Tables.BlockRecordTable.Items.Add(
                    CreateBlockRecordForBlock("*Paper_Space0"));

                // NOTE: Each BLOCK must have BLOCK_RECORD entry.
                
                //file.Tables.BlockRecordTable.Items.Add(
                //    CreateBlockRecordForBlock("BLOCK_NAME));
            }

            file.Tables.LtypeTable.Id = NextHandle();
            TableLtypes(file.Tables.LtypeTable);

            file.Tables.LayerTable.Id = NextHandle();
            TableLayers(file.Tables.LayerTable, container);

            file.Tables.StyleTable.Id = NextHandle();
            TableStyles(file.Tables.StyleTable);

            file.Tables.UcsTable.Id = NextHandle();
            TableUcss(file.Tables.UcsTable);

            file.Tables.ViewTable.Id = NextHandle();
            TableViews(file.Tables.ViewTable);

            file.Tables.VportTable.Id = NextHandle();
            TableVports(file.Tables.VportTable);

            // create blocks
            file.Blocks = new DxfBlocks(_version, NextHandle());
            foreach (var block in DefaultBlocks())
            {
                file.Blocks.Blocks.Add(block);
            }

            // TODO: Add user blocks.

            // create entities
            file.Entities = new DxfEntities(_version, NextHandle());

            // TODO: Add user entities.

            if (container.TemplateLayer.IsVisible)
            {
                DrawShapes(file.Entities, container.TemplateLayer.Shapes, container.TemplateLayer.Name);
            }
            
            foreach (var layer in container.Layers) 
            {
                if (layer.IsVisible)
                {
                    DrawShapes(file.Entities, layer.Shapes, layer.Name);
                }
            }

            // end of entities

            // create objects
            if (_version > DxfAcadVer.AC1009)
            {
                file.Objects = new DxfObjects(_version, NextHandle());

                // mamed dictionary
                var namedDict = new DxfDictionary(_version, NextHandle())
                {
                    OwnerDictionaryHandle = 0.ToDxfHandle(),
                    HardOwnerFlag = false,
                    DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting
                };

                // base dictionary
                var baseDict = new DxfDictionary(_version, NextHandle())
                {
                    OwnerDictionaryHandle = namedDict.Id.ToDxfHandle(),
                    HardOwnerFlag = false,
                    DuplicateRecordCloningFlags = DxfDuplicateRecordCloningFlags.KeepExisting
                };

                // add baseDict to namedDict
                namedDict.Entries.Add(baseDict.Id.ToDxfHandle(), "ACAD_GROUP");

                // finalize dictionaries
                file.Objects.Objects.Add(namedDict);
                file.Objects.Objects.Add(baseDict);

                // TODO: Add Group dictionary.
                // TODO: Add MLine style dictionary.
                // TODO: Add image dictionary dictionary.

                // end of objects
            }

            // set only after file is finilized
            file.Header.NextAvailableHandle = NextHandle();

            Save(path, file.Create());
        }
    }
}
