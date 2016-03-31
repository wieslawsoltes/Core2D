#region netDxf, Copyright(C) 2015 Daniel Carvajal, Licensed under LGPL.
// 
//                         netDxf library
//  Copyright (C) 2009-2015 Daniel Carvajal (haplokuon@gmail.com)
//  
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using Attribute = netDxf.Entities.Attribute;
using Image = netDxf.Entities.Image;
using Point = netDxf.Entities.Point;
using Trace = netDxf.Entities.Trace;

namespace TestDxfDocument
{
    /// <summary>
    /// This is just a simple test of work in progress for the netDxf Library.
    /// </summary>
    public class Program
    {
        private static void Main()
        {
            DxfDocument doc = Test(@"sample.dxf");

            #region Samples for new and modified features 1.1.0

            //WipeoutEntity();
            //ToleranceEntity();
            //LeaderEntity();
            //UnderlayEntity();
            //SplineFitPoints();
            //ImageClippingBoundary();

            #endregion

            #region Samples for new and modified features 1.0.2

            //AssociativeHatches();
            //TraceEntity();
            //SolidEntity();

            #endregion

            #region Samples for new and modified features 1.0.0

            //ModifyingDocumentEntities();
            //ModifyingBlockProperties();
            //ModifyingMLineStyles();
            //DimensionsLinearAndAngularUnits();
            //ModifyingDimensionGeometryAndStyle();
            //ModifyingGroups();
            //ModifyingXData();
            //DimensionUserText();

            #endregion

            #region Samples for fixes, new and modified features 0.9.3

            //RemoveBlock();
            //LinearDimension();
            //AlignedDimension();
            //Angular2LineDimension();
            //Angular3PointDimension();
            //DiametricDimension();
            //RadialDimension();
            //OrdinateDimension();

            #endregion

            #region Samples for fixes, new and modified features 0.9.2

            //NurbsEvaluator();
            //XDataInformation();
            //DynamicBlocks();

            #endregion

            #region Samples for fixes, new and modified features 0.9.1

            //LoadAndSaveBlocks();

            #endregion

            #region Samples for fixes, new and modified features 0.9.0

            //MakingGroups();
            //CombiningTwoDrawings();
            //BinaryChunkXData();
            //BinaryDxfFiles();
            //MeshEntity();

            #endregion

            #region Samples for new and modified features 0.8.0

            //MTextEntity();
            //TransparencySample();
            //DocumentUnits();
            //PaperSpace();
            //BlockWithAttributes();

            #endregion

            #region other

            //NestedBlock();
            //DimensionNestedBlock();
            //EncodingTest();
            //CheckReferences();
            //ComplexHatch();
            //RayAndXLine();
            //UserCoordinateSystems();
            //ExplodeInsert();
            //ImageUsesAndRemove();
            //LayerAndLineTypesUsesAndRemove();
            //TextAndDimensionStyleUsesAndRemove();
            //MLineStyleUsesAndRemove();
            //AppRegUsesAndRemove();
            //ExplodePolyfaceMesh(); 
            //ApplicationRegistries();
            //TestOCStoWCS();
            //WriteGradientPattern();
            //WriteGroup();
            //WriteMLine();
            //ObjectVisibility();
            //EntityTrueColor();
            //EntityLineWeight();
            //ReadWriteFromStream();
            //Text();
            //WriteNoAsciiText();
            //WriteSplineBoundaryHatch();
            //WriteNoInsertBlock();
            //WriteImage();
            //AddAndRemove();
            //LoadAndSave();
            //CleanDrawing();
            //OrdinateDimensionDrawing();
            //Angular2LineDimensionDrawing();
            //Angular3PointDimensionDrawing();
            //DiametricDimensionDrawing();
            //RadialDimensionDrawing();
            //LinearDimensionDrawing();
            //AlignedDimensionDrawing();
            //WriteMText();
            //LineWidth();
            //HatchCircleBoundary();
            //ToPolyline();
            //FilesTest();
            //CustomHatchPattern();
            //LoadSaveHatchTest();
            //WriteDxfFile();
            //ReadDxfFile();
            //ExplodeTest();
            //HatchTestLinesBoundary();
            //HatchTest1();
            //HatchTest2();
            //HatchTest3();
            //HatchTest4();
            //WriteNestedInsert();
            //WritePolyfaceMesh();
            //Ellipse();
            //Solid();
            //Face3d();
            //LwPolyline();
            //Polyline();
            //Dxf2000();
            //SpeedTest();
            //WritePolyline3d();
            //WriteInsert();

            #endregion
        }

        #region Samples for new and modified features 1.1.0

        private static void WipeoutEntity()
        {
            Line line1 = new Line(new Vector2(-1, -1), new Vector2(1, 1));
            Line line2 = new Line(new Vector2(-1, 1), new Vector2(1, -1));
            Circle circle = new Circle(Vector2.Zero, 0.5);

            Wipeout wipeout1 = new Wipeout(new Vector2(-1.5, -0.25), new Vector2(1.5, 0.25)); // a rectangular wipeout defined from two opposite corners
            //Wipeout wipeout1 = new Wipeout(-1.5, -0.25, 3.0, 0.5); // a rectangular wipeout defined by its bottom-left corner and its width and height
            //Wipeout wipeout1 = new Wipeout(new List<Vector2>{new Vector2(-1.5, 0.25), new Vector2(1.5, 0.25), new Vector2(1.5, -0.25), new Vector2(-1.5, -0.25)}); // a polygonal wipeout

            List<Vector2> vertexes = new List<Vector2>
            {
                new Vector2(-30, 30),
                new Vector2(-20, 60),
                new Vector2(-10, 40),
                new Vector2(10, 70),
                new Vector2(30, 20)
            };

            Wipeout wipeout2 = new Wipeout(vertexes);
            // optionally you can set the normal and elevation
            //wipeout1.Normal = new Vector3(1.0);
            //wipeout1.Elevation = 10;
            DxfDocument doc = new DxfDocument();
            doc.AddEntity(line1);
            doc.AddEntity(line2);
            doc.AddEntity(circle);
            doc.AddEntity(wipeout1);
            doc.AddEntity(wipeout2);

            doc.Save("wipeout.dxf");

            DxfDocument test = DxfDocument.Load("wipeout.dxf");
            test.Save("test.dxf");

        }

        private static void ToleranceEntity()
        {
            ToleranceEntry entry = new ToleranceEntry
            {
                GeometricSymbol = ToleranceGeometricSymbol.Symmetry,
                Tolerance1 = new ToleranceValue(true, "12.5", ToleranceMaterialCondition.Maximum)
            };
            Tolerance tolerance = new Tolerance();
            tolerance.Entry1 = entry;
            tolerance.Rotation = 30;

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(tolerance);
            doc.Save("Tolerance.dxf");

            DxfDocument dxf = DxfDocument.Load("Tolerance.dxf");
            dxf.Save("Test.dxf");
        }

        private static void LeaderEntity()
        {
            // a basic text annotation
            List<Vector2> vertexes1 = new List<Vector2>();
            vertexes1.Add(new Vector2(0, 0));
            vertexes1.Add(new Vector2(-5, 5));
            Leader leader1 = new Leader("Sample annotation", vertexes1);
            leader1.Offset = new Vector2(0, -0.5);
            // We need to call manually the method Update if the annotation position is modified,
            // or the Leader properties like Style, Normal, Elevation, Annotation, TextVerticalPosition, and/or Offset.
            leader1.Update();

            // leader not in the XY plane
            Leader cloned = (Leader) leader1.Clone();
            cloned.Normal = new Vector3(1);
            cloned.Elevation = 5;

            // a text annotation with style
            DimensionStyle style = new DimensionStyle("MyStyle");
            style.DIMCLRD = AciColor.Green;
            style.DIMCLRT = AciColor.Blue;
            style.DIMLDRBLK = DimensionArrowhead.DotBlank;
            style.DIMSCALE = 2.0;

            List<Vector2> vertexes2 = new List<Vector2>();
            vertexes2.Add(new Vector2(0, 0));
            vertexes2.Add(new Vector2(5, 5));
            Leader leader2 = new Leader("Sample annotation", vertexes2, style);
            ((MText) leader2.Annotation).AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
            leader2.TextVerticalPosition = LeaderTextVerticalPosition.Centered;
            leader2.Update();

            // a tolerance annotation,
            // its hook point will always appear at the left since I have no way of measuring the real length of a tolerance,
            // and it has no alignment options as the MText does.
            List<Vector2> vertexes3 = new List<Vector2>();
            vertexes3.Add(new Vector2(0));
            vertexes3.Add(new Vector2(5, -5));
            vertexes3.Add(new Vector2(7.5, -5));
            ToleranceEntry entry = new ToleranceEntry
            {
                GeometricSymbol = ToleranceGeometricSymbol.Symmetry,
                Tolerance1 = new ToleranceValue(true, "12.5", ToleranceMaterialCondition.Maximum)
            };
            Leader leader3 = new Leader(entry, vertexes3);

            // a block annotation
            Block block = new Block("BlockAnnotation");
            block.Entities.Add(new Line(new Vector2(-1, -1), new Vector2(1, 1)));
            block.Entities.Add(new Line(new Vector2(-1, 1), new Vector2(1, -1)));
            block.Entities.Add(new Circle(Vector2.Zero, 0.5));

            List<Vector2> vertexes4 = new List<Vector2>();
            vertexes4.Add(new Vector2(0));
            vertexes4.Add(new Vector2(-5, -5));
            vertexes4.Add(new Vector2(-7.5, -5));
            Leader leader4 = new Leader(block, vertexes4);
            // change the leader offset to move  the leader hook (the last vertex of the leader vertexes list) in relation to the annotation position.
            leader4.Offset = new Vector2(1, 1);
            leader4.Update();

            // add entities to the document
            DxfDocument doc = new DxfDocument();
            doc.AddEntity(cloned);
            doc.AddEntity(leader1);
            doc.AddEntity(leader2);
            doc.AddEntity(leader3);
            doc.AddEntity(leader4);
            doc.Save("Leader.dxf");

            DxfDocument test = DxfDocument.Load("Leader.dxf");
            test.Save("test.dxf");
        }

        private static void UnderlayEntity()
        {
            UnderlayDgnDefinition underlayDef1 = new UnderlayDgnDefinition("DgnUnderlay.dgn");
            Underlay underlay1 = new Underlay(underlayDef1);
            underlay1.Normal = new Vector3(1, 0, 0);
            underlay1.Position = new Vector3(0, 1, 0);
            underlay1.Scale = new Vector3(0.001);

            UnderlayDwfDefinition underlayDef2 = new UnderlayDwfDefinition("DwfUnderlay.dwf");
            Underlay underlay2 = new Underlay(underlayDef2);
            underlay2.Rotation = 45;
            underlay2.Scale = new Vector3(0.01);

            UnderlayPdfDefinition underlayDef3 = new UnderlayPdfDefinition("PdfUnderlay.pdf");
            underlayDef3.Page = "3";
            Underlay underlay3 = new Underlay(underlayDef3);

            DxfDocument doc = new DxfDocument(DxfVersion.AutoCad2013);
            doc.AddEntity(underlay1);
            doc.AddEntity(underlay2);
            doc.AddEntity(underlay3);
            doc.Save("UnderlayEntity.dxf");

            DxfDocument load = DxfDocument.Load("UnderlayEntity.dxf");
            load.Save("UnderlayEntity2.dxf");
        }

        private static void SplineFitPoints()
        {
            // this will be the list of fit points, the resulting spline will pass through them.
            List<Vector3> points = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(5, 5, 0),
                new Vector3(10, 0, 0),
                new Vector3(15, 5, 0),
                new Vector3(20, 0, 0),
                new Vector3(0, 0, 0)
            };

            // splines crated from a set of fit points are cubic curves (degree 3) and non periodic.
            // to close the spline just repeat the last fit point.
            // note: at the moment splines created this way cannot be used as a boundary path in a hatch, or converted to a polyline or to a list of vertexes,
            // the control point information is not calculated when an spline is created using a list of fit points.
            Spline spline = new Spline(points);

            // to make the spline continuous at the end points we can adjust the start and end tangents.
            spline.StartTangent = new Vector3(0, 1, 0);
            spline.EndTangent = new Vector3(0, 1, 0);

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(spline);

            Spline cloned = (Spline) spline.Clone();
            cloned.Reverse();
            doc.AddEntity(cloned);


            // and this is a spline created with control points
            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1.0),
                                                    new SplineVertex(new Vector3(25, 50, 50), 2.0),
                                                    new SplineVertex(new Vector3(50, 0, 100), 3.0),
                                                    new SplineVertex(new Vector3(75, 50, 50), 4.0),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5.0)
                                                };

            // the constructor will generate a uniform knot vector 
            Spline openSpline = new Spline(ctrlPoints, 3);
            Spline cloned2 = (Spline) openSpline.Clone();
            
            cloned2.Reverse();
            doc.AddEntity(openSpline);
            doc.AddEntity(cloned2);

            doc.Save("SplineFitPoints.dxf");
        }

        private static void ImageClippingBoundary()
        {
            ImageDefinition imageDef = new ImageDefinition(@".\img\image02.jpg", "MyImage");
            imageDef.ResolutionUnits = ImageResolutionUnits.Centimeters;
            double width = imageDef.Width / imageDef.HorizontalResolution;
            double height = imageDef.Height / imageDef.VerticalResolution;
            Image image = new Image(imageDef, new Vector2(0, 0), width, height);
            image.Rotation = 30;


            // the coordinates of the clipping boundary are relative to the image with its actual dimensions and not to the width and height of its definition.
            // this clipping boundary will only show the middle center of the image.
            double x = width / 4;
            double y = height / 4;
            ClippingBoundary clip = new ClippingBoundary(x, y, 2 * x, 2 * y);
            image.ClippingBoundary = clip;

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(image);
            doc.Save("image.dxf");

            DxfDocument test = DxfDocument.Load("image.dxf");
            test.Save("test.dxf");
        }

        #endregion

        #region Samples for new and modified features 1.0.2

        private static void AssociativeHatches()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            // optionally you can the normal of the polyline, by default it is the UnitZ vector
            //poly.Normal = new Vector3(1.0);
            poly.IsClosed = true;


            HatchBoundaryPath boundary = new HatchBoundaryPath(new List<EntityObject> { poly });
            HatchPattern pattern = HatchPattern.Line;
            pattern.Scale = 10;
            pattern.Angle = 45;

            // the hatch boundary can be set in the hatch constructor or it can be added later
            //Hatch hatch = new Hatch(pattern, new[]{boundary}, true);

            Hatch hatch = new Hatch(pattern, true);
            // you will need to manually set the hatch normal to the boundary normal if it is not the UnitZ,
            // to work properly all boundary entities must belong to the same plane
            //hatch.Normal = poly.Normal;

            // the hatch boundary can be set in the hatch constructor or it can be added later, remember hatches with no boundaries will not be saved
            hatch.BoundaryPaths.Add(boundary);
            Circle circle = new Circle(Vector2.Zero, 5);
            // all boundary entities should have the same normal, by default it is the UnitZ
            // the hatch will not handle the normals of the different boundary path, you will have to make sure they all lay on the same plane
            //circle.Normal = poly.Normal;

            hatch.BoundaryPaths.Add(new HatchBoundaryPath(new List<EntityObject> { circle }));
            // when an associative hatch is added to a document the referenced boundary entities will be added too
            dxf.AddEntity(hatch);
            dxf.Save("Hatch.dxf");


            DxfDocument dxf2 = DxfDocument.Load("Hatch.dxf");
            // you can remove boundaries from a hatch
            dxf2.Hatches[0].BoundaryPaths.Remove(dxf2.Hatches[0].BoundaryPaths[1]);
            // and add new ones
            LwPolyline p = new LwPolyline();
            p.Vertexes.Add(new LwPolylineVertex(-20, -20));
            p.Vertexes.Add(new LwPolylineVertex(20, -20));
            p.Vertexes.Add(new LwPolylineVertex(20, 20));
            p.Vertexes.Add(new LwPolylineVertex(-20, 20));
            p.IsClosed = true;
            dxf2.Hatches[0].BoundaryPaths.Add(new HatchBoundaryPath(new List<EntityObject> { p }));
            dxf2.Save("Hatch add and remove boundaries.dxf");


            DxfDocument dxf3 = DxfDocument.Load("Hatch.dxf");
            // unlinking the boundary entities from a hatch will not automatically remove them from the document, you can use the returned list to delete them
            // unlinking the boundary will make the hatch non-associative 
            List<EntityObject> oldBoundary = dxf3.Hatches[0].UnLinkBoundary();
            dxf3.RemoveEntity(oldBoundary);

            // we can recreate the hatch boundary and optionally linking it, thus making it associative,
            // if the hatch is associative and belongs to a document the new entities will also be automatically added to the same document
            List<EntityObject> newBoundary = dxf3.Hatches[0].CreateBoundary(true);

            dxf3.Save("Hatch new contour.dxf");

            DxfDocument dxf4 = DxfDocument.Load("Hatch.dxf");
            // if the hatch is associative, it is possible to modify the entities that make the boundary
            // for non-associative the list of entities will contain zero items
            if (dxf4.Hatches[0].Associative)
            {
                // this will only work for associative hatches
                HatchBoundaryPath path = dxf4.Hatches[0].BoundaryPaths[0];
                LwPolyline entity = (LwPolyline) path.Entities[0];
                entity.Vertexes[2].Position = new Vector2(15, 15);
                // after modifying the boundary entities, it is necessary to rebuild the edges
                path.Update();
                dxf4.Save("Hatch change boundary.dxf");
            }
        }

        private static void TraceEntity()
        {
            // The Trace entity behaves exactly as the Solid, they have the same properties and have the same graphical representation.
            // They are a different entity since in AutoCad they show as two distinct types of objects.
            // In any case, it is recommended to always use the Solid in its place.
            Vector2 a = new Vector2(-1, -1);
            Vector2 b = new Vector2(1, -1);
            Vector2 c = new Vector2(-1, 1);
            Vector2 d = new Vector2(1, 1);

            Trace trace = new Trace(a, b, c, d);
            trace.Normal = new Vector3(1, 1, 0);
            trace.Elevation = 2;

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(trace);
            doc.Save("TraceEntity.dxf");
        }

        private static void SolidEntity()
        {
            // The solid vertexes are expressed in OCS (object coordinate system)
            // Now they are stored as Vector2 to force all vertexes to lay on a plane, this is similar as how the LwPolyline works.
            // The Z coordinate is controlled by the elevation property of the Solid.
            Vector2 a = new Vector2(-1, -1);
            Vector2 b = new Vector2(1, -1);
            Vector2 c = new Vector2(-1, 1);
            Vector2 d = new Vector2(1, 1);

            Solid solid = new Solid(a, b, c, d);
            solid.Normal = new Vector3(1, 1, 0);

            solid.Elevation = 2;

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(solid);
            doc.Save("SolidEntity.dxf");
        }

        #endregion

        #region Samples for new and modified features 1.0.0

        public static void ModifyingDocumentEntities()
        {
            Layer layer1 = new Layer("layer1");
            Layer layer2 = new Layer("layer2");
            Layer layer3 = new Layer("layer3");

            LineType lineType1 = LineType.Dot;
            LineType lineType2 = LineType.Dashed;

            Line line = new Line(Vector2.Zero, Vector2.UnitX);
            line.Layer = layer1;
            line.LineType = lineType1;

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(line);

            // if the layer does not exist in the document it will be added automatically
            line.Layer = layer2;
            Debug.Assert(ReferenceEquals(line.Layer, doc.Layers[line.Layer.Name]), "References are not equal.");

            // you can always add it first
            doc.Layers.Add(layer3);
            // layer3 is defined in the document
            line.Layer = layer3;
            Debug.Assert(ReferenceEquals(line.Layer, doc.Layers[line.Layer.Name]), "References are not equal.");

            // same thing is applicable to line types
            line.LineType = lineType2;
            Debug.Assert(ReferenceEquals(line.LineType, doc.LineTypes[line.LineType.Name]), "References are not equal.");

            doc.Save("entity.dxf");

            // it is also possible to rename table objects
            layer1.Name = "New layer1 name";
            lineType1.Name = "DotDot";

            // this operation is illegal, you cannot rename reserved table objects.
            //doc.Layers[Layer.DefaultName].Name = "NewName";

            doc.Save("test.dxf");
        }

        public static void ModifyingBlockProperties()
        {
            DxfDocument doc = new DxfDocument();
            doc.DrawingVariables.InsUnits = DrawingUnits.Centimeters;
            Line existingLine = new Line(new Vector2(-10, 10), new Vector2(10, -10));
            doc.AddEntity(existingLine);

            AttributeDefinition attDef4 = new AttributeDefinition("MyAttribute4");
            attDef4.Value = "MyValue4";
            attDef4.Alignment = TextAlignment.TopCenter;
            Block block = new Block("MyBlock", null, new List<AttributeDefinition>{attDef4});
            block.Record.Units = DrawingUnits.Millimeters;

            // this is incorrect we cannot add an entity that belongs to a document when the block does not belong to anyone.
            //block.Entities.Add(existingLine);
            doc.Blocks.Add(block);
            // when the block and the entity that is being added belong to the same document, the entity will be removed from its current layout and added to the block
            // you cannot add an entity that belongs to a different document or block. Clone it instead.
            block.Entities.Add(existingLine);

            // now we can modify the block properties even if it has been already added to the document
            Line line = new Line(new Vector2(-10, -10), new Vector2(10, 10));

            // when new entities that do not belong to anyone are added to an existing block, they will also be added to the document
            block.Entities.Add(line);

            DxfDocument doc2 = new DxfDocument();
            Circle circle = new Circle(Vector2.Zero, 5);
            doc2.AddEntity(circle);

            // this is incorrect the circle already belongs to another document
            //block.Entities.Add(circle);
            // we need to clone it first
            Circle circle2 = (Circle) circle.Clone();
            circle2.Radius = 2.5;
            block.Entities.Add(circle2);

            //you could also remove circle2 from doc2 and add it to the block
            doc2.RemoveEntity(circle);
            block.Entities.Add(circle);

            AttributeDefinition attDef = new AttributeDefinition("MyAttribute1");
            attDef.Value = "MyValue1";
            block.AttributeDefinitions.Add(attDef);

            // the same that is applicable to entities is also true to attribute definitions
            AttributeDefinition attDef2 = new AttributeDefinition("MyAttribute2");
            attDef2.Value = "MyValue2";
            attDef2.Alignment = TextAlignment.BaselineRight;
            block.AttributeDefinitions.Add(attDef2);

            Insert ins = new Insert(block);
            doc.AddEntity(ins);

            // if the insert has been added to a document, any new attribute definitions added to the block will not be reflected in the insert
            // this mimics the behavior in AutoCad
            AttributeDefinition attDef3 = new AttributeDefinition("MyAttribute3");
            attDef3.Value = "MyValue3";
            attDef3.Alignment = TextAlignment.TopCenter;
            block.AttributeDefinitions.Add(attDef3);
            ins.Rotation = 30;

            // to update the insert attributes call the method Sync, this method will also call the method TransformAttributes
            ins.Sync();

            // the ins2 will have all three attributes
            Insert ins2 = new Insert(block, new Vector2(20,0));
            doc.AddEntity(ins2);

            doc.Save("Test.dxf");

            block.Name = "MyBlockRenamed";

            doc.Save("BlockRename.dxf");

            doc = Test("BlockRename.dxf");
        }

        public static void ModifyingMLineStyles()
        {
            DxfDocument doc = new DxfDocument(DxfVersion.AutoCad2010);
            doc.DrawingVariables.LtScale = 10;

            List<Vector2> vertexes = new List<Vector2>
                                            {
                                                new Vector2(0, 0),
                                                new Vector2(0, 150),
                                                new Vector2(150, 150),
                                                new Vector2(150, 0)
                                            };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
         
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();           
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;

            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
           
            doc.AddEntity(mline);

            // change the multi line style after it has been added to the document
            mline.Style = style;
            Debug.Assert(ReferenceEquals(mline.Style, doc.MlineStyles[mline.Style.Name]), "Reference not equals.");

            // VERY IMPORTANT: We have modified the MLine after setting its vertexes so we need to manually call this method.
            // It is also necessary when manually editing the vertex distances.
            mline.Update();
            
            // the line type will be automatically added to the document
            foreach (MLineStyleElement e in style.Elements)
            {
                // making changes after the MLineStyle has been added to the document
                e.LineType = LineType.Dashed;
                Debug.Assert(ReferenceEquals(e.LineType, doc.LineTypes[e.LineType.Name]), "Reference not equals.");
            }

            MLine copy = (MLine) mline.Clone();
            copy.Scale = 100;
            doc.AddEntity(copy);
            // once the entity has been added to the document, changing its style requires that the new style is also present in the document.
            copy.Style = doc.MlineStyles["standard"];
            // VERY IMPORTANT: We have modified the MLine after setting its vertexes so we need to manually call this method.
            // It is also necessary when manually editing the vertex distances.
            copy.Update();

            doc.Save("ModifyingMLineStyle.dxf");
            Test("ModifyingMLineStyle.dxf");
        }

        public static void ModifyingDimensionGeometryAndStyle()
        {
            DimensionStyle style = new DimensionStyle("MyStyle");
            Vector3 p1 = new Vector3(-2.5, 0, 0);
            Vector3 p2 = new Vector3(2.5, 0, 0);

            LinearDimension dim = new LinearDimension(p1, p2, 4, 0, style);

            // This is illegal. Trying to rebuild the dimension block before it has been added to a document will throw an exception
            //dim.RebuildBlock();

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(dim);

            // modifying the dimension style
            dim.Style.DIMBLK = DimensionArrowhead.ArchitecturalTick;
            // if we make any change to the dimension style, we need to manually call the RebuildBlock method to reflect the new changes
            // since we will also modify the geometry of the dimension we will rebuild the block latter
            //dim.RebuildBlock();

            // the same kind of procedure needs to be done when modifying the geometry of a dimension
            dim.FirstReferencePoint = new Vector3(-5.0, 0, 0);
            dim.SecondReferencePoint = new Vector3(5.0, 0, 0);
            // now that all necessary changes has been made, we will rebuild the block.
            // this is an expensive operation, use it only when need it.

            dim.Style.DIMBLK = DimensionArrowhead.Box;
            dim.Style.DIMBLK = DimensionArrowhead.ArchitecturalTick;
            Debug.Assert(ReferenceEquals(dim.Style.DIMBLK, doc.Blocks[dim.Style.DIMBLK.Name]), "References are not equal.");
            Debug.Assert(ReferenceEquals(style.DIMBLK, doc.Blocks[style.DIMBLK.Name]), "References are not equal.");
            //dim.Style.DIMBLK = null;

            // VERY IMPORTANT: If any change is made to the dimension geometry and/or its style, we need to rebuild the drawing representation
            // so the dimension block will reflect the new changes. This is only necessary for dimension that already belongs to a document.
            // This process is automatically called when a new dimension is added to a document.
            dim.Update();
            Debug.Assert(ReferenceEquals(dim.Block, doc.Blocks[dim.Block.Name]));

            doc.Save("dimension.dxf");
            Test("dimension.dxf");

        }

        public static void ModifyingGroups()
        {
            Line line1 = new Line(new Vector2(0, 0), new Vector2(100, 100));
            line1.Color = AciColor.Red;
            Line line2 = new Line(new Vector2(100, 0), new Vector2(200, 100));
            line2.Color = AciColor.Yellow;
            Line line3 = new Line(new Vector2(200, 0), new Vector2(300, 100));
            line3.Color = AciColor.Magenta;

            DxfDocument doc = new DxfDocument();

            Block blk = new Block("MyBlock");
            blk.Entities.Add(line1);
            Insert ins = new Insert(blk);
            doc.AddEntity(ins);

            doc.AddEntity(line2);

            Layout layout = new Layout("Layout1");
            doc.Layouts.Add(layout);
            doc.ActiveLayout = layout.Name;
            doc.AddEntity(line3);
            
            // group
            Group group = new Group("MyGroup");
            doc.Groups.Add(group);

            // the Add method will also add the entities contained in a group to the document (in the active layout).
            doc.Groups.Add(group);

            // when the group belongs to a document, all entities must belong to the same document.
            // even if it does not sound very useful, a group can contain entities that belongs to different layouts and even blocks.
            group.Entities.Add(line1);
            group.Entities.Add(line2);
            group.Entities.Add(line3);

            Line line4 = new Line(new Vector2(300, 0), new Vector2(400, 100));
            line4.Color = AciColor.Blue;
            // if a new entity, that does not belong to any document, is added to the group, it will be added to the group document active layout.
            doc.ActiveLayout = Layout.ModelSpaceName;
            group.Entities.Add(line4);

            Line line5 = new Line(new Vector2(400, 0), new Vector2(500, 100));
            line5.Color = AciColor.Green;
            DxfDocument doc2 = new DxfDocument();
            doc2.AddEntity(line5);

            // this is illegal, line5 belongs to another document.
            //group.Entities.Add(line5);
            // you need to clone the entity before adding it to the group. This is also the common practice to copy entities between documents.
            group.Entities.Add((EntityObject) line5.Clone());

            // remember removing a group only deletes it from the collection not the entities
            //doc.Groups.Remove(group);
            doc.Save("group.dxf");

            doc = DxfDocument.Load("group.dxf");
        }

        public static void ModifyingXData()
        {
            Line line = new Line(Vector2.Zero, Vector2.UnitX);

            ApplicationRegistry appReg = new ApplicationRegistry("netDxf");
            XData xdata = new XData(appReg);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Length"));
            line.XData.Add(xdata);

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(line);

            // modifying existing extended data
            line.XData[appReg.Name].XDataRecord.Add(new XDataRecord(XDataCode.Real, Vector3.Distance(line.StartPoint, line.EndPoint)));

            // adding new extended data entry to an existing entity
            ApplicationRegistry appReg2 = new ApplicationRegistry("newXData");
            XData xdata2 = new XData(appReg2);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "XData entries"));
            line.XData.Add(xdata2);

            Debug.Assert(ReferenceEquals(line.XData[appReg2.Name].ApplicationRegistry, doc.ApplicationRegistries[appReg2.Name]));

            // deleting existing extended data
            line.XData.Remove(appReg.Name);

            // we can also change the name of the application registry name
            doc.ApplicationRegistries["newXData"].Name = "netDxfRenamed";

            doc.Save("xData.dxf");

            doc = DxfDocument.Load("xData.dxf");
        }

        public static void DimensionsLinearAndAngularUnits()
        {
            DimensionStyle style = new DimensionStyle("MyStyle")
            {
                // DIMDEC defines the number of decimal places.
                // For Architectural and Fractional units the minimum fraction is defined by 1/2^DIMDEC.
                DIMDEC = 4,
                DIMFRAC = FractionFormatType.Horizontal,
                DIMLUNIT = LinearUnitType.Engineering,
                SuppressLinearTrailingZeros = true,
                SuppressZeroFeet = false,
                SuppressZeroInches = false,
                DIMLFAC = 10.0,
                // the round off to nearest DIMRND is applied to the linear dimension measurement after applying the scale DIMLFAC
                DIMRND = 0.025,
                DIMADEC = 2,
                DIMAUNIT = AngleUnitType.DegreesMinutesSeconds
            };

            Layer layer = new Layer("Layer1") { Color = AciColor.Blue };

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(21.2548, 0, 0);

            LinearDimension dim = new LinearDimension(p1, p2, 4, 0, style);

            Vector2 s1 = new Vector2(-2, 2);
            Vector2 s2 = new Vector2(2, -2);

            Vector2 e1 = new Vector2(-1, -3);
            Vector2 e2 = new Vector2(1, 3);

            Line line1 = new Line(s1, s2) { Layer = layer };
            Line line2 = new Line(e1, e2) { Layer = layer };
            Angular2LineDimension dim1 = new Angular2LineDimension(line2, line1, 4, style);

            DxfDocument doc = new DxfDocument();
            doc.AddEntity(dim);
            doc.AddEntity(dim1);
            doc.Save("DimensionsLinearAndAngularUnits.dxf");

            DxfDocument dxf = DxfDocument.Load("DimensionsLinearAndAngularUnits.dxf");
        }

        public static void DimensionUserText()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);

            Vector3 p2 = new Vector3(0, 0, 0);
            Vector3 p1 = new Vector3(5, 0, 0);

            Line line1 = new Line(p1, p2)
            {
                Layer = new Layer("Reference line")
                {
                    Color = AciColor.Green
                }
            };
            dxf.AddEntity(line1);

            DimensionStyle style = new DimensionStyle("MyStyle");

            double offset = 0.75;
            LinearDimension dim = new LinearDimension(line1, offset, 0, style);
            dim.UserText = null;    // 5.00 (this is the default behavior)
            dxf.AddEntity(dim);

            dim = new LinearDimension(line1, 2 * offset, 0, style);
            dim.UserText = string.Empty;    // 5.00 (same behavior as null)
            dxf.AddEntity(dim);

            dim = new LinearDimension(line1, 3 * offset, 0, style);
            dim.UserText = " ";    // No dimension text will be drawn (one blank space)
            dxf.AddEntity(dim);

            dim = new LinearDimension(line1, 4 * offset, 0, style);
            dim.UserText = "<>";    // 5.00 (the characters <> will be substituted with the style.DIMPOST property)
            dxf.AddEntity(dim);

            dim = new LinearDimension(line1, 5 * offset, 0, style);
            dim.UserText = "Length: <> mm"; // Length: 5.00 mm (the characters <> will be substituted with the style.DIMPOST property)
            dxf.AddEntity(dim);

            dim = new LinearDimension(line1, 6 * offset, 0, style);
            dim.UserText = "User text"; // User text
            dxf.AddEntity(dim);

            dxf.Save("DimensionUserText.dxf");

        }

        #endregion

        #region Samples for new and modified features 0.9.3

        private static void RemoveBlock()
        {
            DxfDocument dxf = new DxfDocument();
            Block block = new Block("MyBlock");

            Line line1 = new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0));
            Line line2 = new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0));
            block.Entities.Add(line1);
            block.Entities.Add(line2);

            Insert insert = new Insert(block);

            dxf.AddEntity(insert);

            bool ok;
            // line1 is used by block and cannot be removed (ok = false)
            ok = dxf.RemoveEntity(line1);
            // block is used by insert and cannot be removed (ok = false)
            ok = dxf.Blocks.Remove(block);
            // it is safe to remove insert, it doesn't belong to anybody (ok = true)
            ok = dxf.RemoveEntity(insert);
            // it is safe to remove block, it doesn't belong to anybody (ok = true)
            // at the same time, all entities that were part of the block have been also removed
            ok = dxf.Blocks.Remove(block);
            // obj is null the line1 does not exist in the document, the block was removed
            DxfObject obj = dxf.GetObjectByHandle(line1.Handle);

            Console.WriteLine("Press a key...");
            Console.ReadKey();

        }

        private static DimensionStyle CreateDimStyle()
        {
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMDLE = 0.18;
            myStyle.DIMDEC = 2;
            myStyle.DIMSCALE = 2.0;
            myStyle.DIMSAH = true;
            //myStyle.DIMLTEX1 = LineType.Dot;
            myStyle.DIMCLRD = AciColor.Yellow;
            //myStyle.DIMBLK = DimensionArrowhead.ArchitecturalTick;
            myStyle.DIMBLK1 = DimensionArrowhead.Box;
            myStyle.DIMBLK2 = DimensionArrowhead.DotBlank;
            //myStyle.DIMSE1 = true;
            //myStyle.DIMSE2 = true;

            myStyle.DIMCLRT = AciColor.Red;

            return myStyle;
        }

        private static void LinearDimension()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);
            DimensionStyle myStyle = CreateDimStyle();

            Vector3 p1 = new Vector3(0, -5, 0);
            Vector3 p2 = new Vector3(-5, 0, 0);

            Line line1 = new Line(p1, p2)
            {
                Layer = new Layer("Reference line")
                {
                    Color = AciColor.Green
                }
            };
            dxf.AddEntity(line1);

            double offset = 4;
            LinearDimension dimX1 = new LinearDimension(line1, offset, 0, myStyle);
            LinearDimension dimY1 = new LinearDimension(line1, offset, 90, myStyle);
            LinearDimension dim5 = new LinearDimension(line1, offset, -30, myStyle);
            LinearDimension dim6 = new LinearDimension(line1, offset, -60, myStyle);

            Vector3 p3 = new Vector3(6, -5, 0);
            Vector3 p4 = new Vector3(11, 0, 0);
            Line line2 = new Line(p3, p4)
            {
                Layer = new Layer("Reference line")
                {
                    Color = AciColor.Green
                }
            };
            dxf.AddEntity(line2);
            LinearDimension dimX2 = new LinearDimension(line2, offset, -30.0, myStyle);
            LinearDimension dimY2 = new LinearDimension(line2, offset, -60.0, myStyle);
            LinearDimension dim3 = new LinearDimension(line2, offset, 30.0, myStyle);
            LinearDimension dim4 = new LinearDimension(line2, offset, 60.0, myStyle);

            dxf.AddEntity(dimX1);
            dxf.AddEntity(dimY1);
            dxf.AddEntity(dimX2);
            dxf.AddEntity(dimY2);
            dxf.AddEntity(dim3);
            dxf.AddEntity(dim4);
            dxf.AddEntity(dim5);
            dxf.AddEntity(dim6);
            dxf.Save("dimension drawing.dxf");

            //DxfDocument dwg = DxfDocument.Load("dimension drawing.dxf");
            //dwg.Save("dimension drawing saved.dxf");

            //Console.WriteLine("Press a key...");
            //Console.ReadKey();
        }
        
        private static void AlignedDimension()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);
            DimensionStyle myStyle = CreateDimStyle();

            Vector3 p1 = new Vector3(0, -5, 0);
            Vector3 p2 = new Vector3(-5, 0, 0);

            Line line1 = new Line(p1, p2)
            {
                Layer = new Layer("Reference line")
                {
                    Color = AciColor.Green
                }
            };
            dxf.AddEntity(line1);

            double offset = 4;
            AlignedDimension dim1 = new AlignedDimension(line1, offset, myStyle);
            AlignedDimension dim11 = new AlignedDimension(line1, -offset, myStyle);

            Vector3 p3 = new Vector3(6, -5, 0);
            Vector3 p4 = new Vector3(11, 0, 0);
            Line line2 = new Line(p3, p4)
            {
                Layer = new Layer("Reference line")
                {
                    Color = AciColor.Green
                }
            };
            dxf.AddEntity(line2);
            AlignedDimension dim2 = new AlignedDimension(line2, offset, myStyle);
            AlignedDimension dim21 = new AlignedDimension(line2, -offset, myStyle);

            dxf.AddEntity(dim1);
            dxf.AddEntity(dim11);
            dxf.AddEntity(dim2);
            dxf.AddEntity(dim21);

            dxf.Save("dimension drawing.dxf");

            //Console.WriteLine("Press a key...");
            //Console.ReadKey();

            //DxfDocument dwg = DxfDocument.Load("dimension drawing.dxf");
            //dwg.Save("dimension drawing saved.dxf");

            
        }

        private static void Angular2LineDimension()
        {
            DimensionStyle myStyle = CreateDimStyle();

            double offset = 4;

            Layer layer = new Layer("Layer1") {Color = AciColor.Blue};
            Vector2 s1 = new Vector2(-2, 2);
            Vector2 s2 = new Vector2(2, -2);

            Vector2 e1 = new Vector2(-1,-3);
            Vector2 e2 = new Vector2(1,3);

            Line line1 = new Line(s1, s2){Layer = layer};
            Line line2 = new Line(e1, e2){Layer = layer};
            Angular2LineDimension dim1 = new Angular2LineDimension(line1, line2, offset, myStyle);
            Angular2LineDimension dim2 = new Angular2LineDimension(line1, line2, -offset, myStyle);
            line1.Reverse();
            Angular2LineDimension dim3 = new Angular2LineDimension(line2, line1, offset, myStyle);
            Angular2LineDimension dim4 = new Angular2LineDimension(line2, line1, -offset, myStyle);

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line1);
            dxf.AddEntity(line2);
            dxf.AddEntity(dim1);
            dxf.AddEntity(dim2);
            dxf.AddEntity(dim3);
            dxf.AddEntity(dim4);
            dxf.Save("dimension drawing.dxf");
            dxf = DxfDocument.Load("dimension drawing.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("dimension drawing saved.dxf");
        }

        private static void Angular3PointDimension()
        {
            DxfDocument dxf = new DxfDocument();
            DimensionStyle myStyle = CreateDimStyle();
            myStyle.DIMADEC = 4;
            myStyle.DIMAUNIT = AngleUnitType.DegreesMinutesSeconds;
            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.5;
            Arc arc = new Arc(center, radius, -32.8, 160.5);
            Angular3PointDimension dim1 = new Angular3PointDimension(arc, 5, myStyle);
            Angular3PointDimension dim2 = new Angular3PointDimension(arc, -5, myStyle);
            dxf.AddEntity(arc);
            dxf.AddEntity(dim1);
            dxf.AddEntity(dim2);
            dxf.Save("dimension drawing.dxf");

            dxf = DxfDocument.Load("dimension drawing.dxf");

            DxfDocument doc = new DxfDocument();
            foreach (var c in dxf.Circles)
            {
                doc.AddEntity((EntityObject)c.Clone());
            }
            foreach (var d in dxf.Dimensions)
            {
                doc.AddEntity((EntityObject)d.Clone());
            }
            doc.Save("dimension drawing saved.dxf");
        }

        private static void DiametricDimension()
        {          
            DxfDocument dxf = new DxfDocument();
            DimensionStyle myStyle = CreateDimStyle();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 3;
            Circle circle = new Circle(center, radius);
            //circle.Normal = new Vector3(1, 1, 1);
            double angle = MathHelper.HalfPI*0.5;
            Vector3 refPoint = center + new Vector3(radius * Math.Cos(angle), radius * Math.Cos(angle), 0);

            //DiametricDimension dim = new DiametricDimension(center, refPoint, -1.0, myStyle);
            double offset = 0;
            DiametricDimension dim1 = new DiametricDimension(circle, 0, offset, myStyle);
            DiametricDimension dim2 = new DiametricDimension(circle, 45, offset, myStyle);
            DiametricDimension dim3 = new DiametricDimension(circle, 90, offset, myStyle);
            DiametricDimension dim4 = new DiametricDimension(circle, 120, offset, myStyle);
            DiametricDimension dim5 = new DiametricDimension(circle, 180, offset, myStyle);
            DiametricDimension dim6 = new DiametricDimension(circle, 220, offset, myStyle);
            DiametricDimension dim7 = new DiametricDimension(circle, 270, offset, myStyle);
            DiametricDimension dim8 = new DiametricDimension(circle, 330, offset, myStyle);

            // if the dimension normal is not equal to the circle normal strange things might happen at the moment
            //dim1.Normal = circle.Normal;
            dxf.AddEntity(circle);
            dxf.AddEntity(dim1);
            dxf.AddEntity(dim2);
            dxf.AddEntity(dim3);
            dxf.AddEntity(dim4);
            dxf.AddEntity(dim5);
            dxf.AddEntity(dim6);
            dxf.AddEntity(dim7);
            dxf.AddEntity(dim8);
            dxf.Save("dimension drawing.dxf");

            dxf = DxfDocument.Load("dimension drawing.dxf");

            DxfDocument doc = new DxfDocument();
            foreach (var c in dxf.Circles)
            {
                doc.AddEntity((EntityObject) c.Clone());
            }
            foreach (var d in dxf.Dimensions)
            {
                doc.AddEntity((EntityObject) d.Clone());
            }
            doc.Save("dimension drawing saved.dxf");
        }

        private static void RadialDimension()
        {
            DxfDocument dxf = new DxfDocument();
            DimensionStyle myStyle = CreateDimStyle();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 3;
            Circle circle = new Circle(center, radius);
            //circle.Normal = new Vector3(1, 1, 1);
            double angle = MathHelper.HalfPI * 0.5;
            Vector3 refPoint = center + new Vector3(radius * Math.Cos(angle), radius * Math.Cos(angle), 0);

            //DiametricDimension dim = new DiametricDimension(center, refPoint, -1.0, myStyle);
            double offset = 3;
            RadialDimension dim1 = new RadialDimension(circle, 0, offset, myStyle);
            RadialDimension dim2 = new RadialDimension(circle, 45, offset, myStyle);
            RadialDimension dim3 = new RadialDimension(circle, 90, offset, myStyle);
            RadialDimension dim4 = new RadialDimension(circle, 120, offset, myStyle);
            RadialDimension dim5 = new RadialDimension(circle, 180, offset, myStyle);
            RadialDimension dim6 = new RadialDimension(circle, 220, offset, myStyle);
            RadialDimension dim7 = new RadialDimension(circle, 270, offset, myStyle);
            RadialDimension dim8 = new RadialDimension(circle, 330, offset, myStyle);
            // if the dimension normal is not equal to the circle normal strange things might happen at the moment
            //dim1.Normal = circle.Normal;
            dxf.AddEntity(circle);
            dxf.AddEntity(dim1);
            dxf.AddEntity(dim2);
            dxf.AddEntity(dim3);
            dxf.AddEntity(dim4);
            dxf.AddEntity(dim5);
            dxf.AddEntity(dim6);
            dxf.AddEntity(dim7);
            dxf.AddEntity(dim8);
            dxf.Save("dimension drawing.dxf");

            dxf = DxfDocument.Load("dimension drawing.dxf");

            DxfDocument doc = new DxfDocument();
            foreach (var c in dxf.Circles)
            {
                doc.AddEntity((EntityObject)c.Clone());
            }
            foreach (var d in dxf.Dimensions)
            {
                doc.AddEntity((EntityObject)d.Clone());
            }
            doc.Save("dimension drawing saved.dxf");
        }

        private static void OrdinateDimension()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 origin = new Vector3(2, 1, 0);
            Vector2 refX = new Vector2(1, 0);
            Vector2 refY = new Vector2(0, 2);
            double length = 3;
            double angle = 30;
            DimensionStyle myStyle = CreateDimStyle();

            OrdinateDimension dimX1 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, 0, myStyle);
            OrdinateDimension dimX2 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, angle, myStyle);
            OrdinateDimension dimY1 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, 0, myStyle);
            OrdinateDimension dimY2 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, angle, myStyle);

            dxf.AddEntity(dimX1);
            dxf.AddEntity(dimY1);
            dxf.AddEntity(dimX2);
            dxf.AddEntity(dimY2);

            Line lineX = new Line(origin, origin + 5 * Vector3.UnitX);
            Line lineY = new Line(origin, origin + 5 * Vector3.UnitY);

            Vector2 point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad);
            Line lineXRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad + MathHelper.HalfPI);
            Line lineYRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            dxf.AddEntity(lineX);
            dxf.AddEntity(lineY);
            dxf.AddEntity(lineXRotate);
            dxf.AddEntity(lineYRotate);

            dxf.Save("dimension drawing.dxf");

            dxf = DxfDocument.Load("dimension drawing.dxf");

            DxfDocument doc = new DxfDocument();
            foreach (var c in dxf.Circles)
            {
                doc.AddEntity((EntityObject)c.Clone());
            }
            foreach (var d in dxf.Dimensions)
            {
                doc.AddEntity((EntityObject)d.Clone());
            }
            doc.Save("dimension drawing saved.dxf");
        }

        #endregion

        #region Samples for new and modified features 0.9.2

        public static void NurbsEvaluator()
        {
            Layer result = new Layer("Nurbs evaluator");
            result.Color = AciColor.Red;

            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1.0),
                                                    new SplineVertex(new Vector3(25, 50, 50), 2.0),
                                                    new SplineVertex(new Vector3(50, 0, 100), 3.0),
                                                    new SplineVertex(new Vector3(75, 50, 50), 4.0),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5.0)
                                                };

            // the constructor will generate a uniform knot vector 
            Spline openSpline = new Spline(ctrlPoints, 3);

            List<SplineVertex> ctrlPointsClosed = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0)),
                                                    new SplineVertex(new Vector3(25, 50, 0)),
                                                    new SplineVertex(new Vector3(50, 0, 0)),
                                                    new SplineVertex(new Vector3(75, 50, 0)),
                                                    new SplineVertex(new Vector3(100, 0, 0)),
                                                    new SplineVertex(new Vector3(0, 0, 0)) // closed spline non periodic we repeat the last control point
                                                };
            Spline closedNonPeriodicSpline = new Spline(ctrlPointsClosed, 3);

            // the periodic spline will generate a periodic (unclamped) closed curve,
            // as far as my tests have gone not all programs handle them correctly, most of them only handle clamped splines
            Spline closedPeriodicSpline = new Spline(ctrlPoints, 4, true);
            // always use spline vertex weights of 1.0 (default value) looks like that AutoCAD does not handle them correctly for periodic splines,
            // but they work fine for non periodic splines
            closedPeriodicSpline.SetUniformWeights(1.0);

            // manually defining the control points and the knot vector (example a circle created with nurbs)
            List<SplineVertex> circle = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0),
                                                    new SplineVertex(new Vector3(100, 0, 0), 0.5),
                                                    new SplineVertex(new Vector3(100, 100, 0), 0.5),
                                                    new SplineVertex(new Vector3(50, 100, 0), 1.0),
                                                    new SplineVertex(new Vector3(0, 100, 0), 0.5),
                                                    new SplineVertex(new Vector3(0, 0, 0), 0.5),
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0) // repeat the first point to close the circle
                                                };

            // the number of knots must be control points number + degree + 1
            // Conics are 2nd degree curves
            List<double> knots = new List<double>{ 0.0, 0.0, 0.0, 1.0 / 4.0, 1.0 / 2.0, 1.0 / 2.0, 3.0 / 4.0, 1.0, 1.0, 1.0 };
            Spline splineCircle = new Spline(circle, knots, 2);

            DxfDocument dxf = new DxfDocument();

            Polyline pol;

            dxf.AddEntity(openSpline);
            // we will convert the Spline to a Polyline
            pol = openSpline.ToPolyline(100);
            pol.Layer = result;
            dxf.AddEntity(pol);

            dxf.AddEntity(closedNonPeriodicSpline);
            pol = closedNonPeriodicSpline.ToPolyline(100);
            pol.Layer = result;
            dxf.AddEntity(pol);

            dxf.AddEntity(closedPeriodicSpline);
            pol = closedPeriodicSpline.ToPolyline(100);
            pol.Layer = result;
            dxf.AddEntity(pol);
      
            dxf.AddEntity(splineCircle);
            pol = splineCircle.ToPolyline(100);
            pol.Layer = result;
            dxf.AddEntity(pol);

            dxf.Save("spline.dxf");

        }

        public static void XDataInformation()
        {
            Line line = new Line(Vector2.Zero, Vector2.UnitY);
            XData xdata = new XData(new ApplicationRegistry("a"));
            //xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            //xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "a"));
            //xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 1));
            //xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 1.1));
            //xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldDirectionX, 1.2));
            line.XData.Add(xdata);
            DxfDocument dwg = new DxfDocument();
            dwg.AddEntity(line);
            dwg.Save("Xdata.dxf");
        }

        public static void DynamicBlocks()
        {
            // netDxf can read dynamic blocks but you will loose the information related with the parameters, actions and constrains
            // for every insert AutoCad creates a new block definition with the name *U#, where # is a positive integer
            // except in the case where the dynamic block parameters are not modified, in this case the original block will be used instead

            DxfDocument drawing;
            drawing = DxfDocument.Load("..//samples//DynamicBlock.dxf");

            Insert insert;
            Block block;
            string name;
            EntityCollection entities;

            // let's take a look at the block associated with the inserts
            // the dynamic parameter of this insert was modified so the block name will be called *U#
            insert = drawing.Inserts[0];
            block = insert.Block;
            // this is the block name
            name = block.Name;
            // the list of entities contained in the block are the ones defined in the original block definition modified by the dynamic parameter
            entities = block.Entities;

            // to access the original dynamic block we need to get first the extended data associated with the BlockRecord,
            // the application registry for this extended data always has the name "AcDbBlockRepBTag"
            XData xdata = block.Record.XData["AcDbBlockRepBTag"];
            string handle = null;
            // the original dynamic block handle is stored in the extended data
            foreach (XDataRecord data in xdata.XDataRecord)
            {
                if (data.Code == XDataCode.DatabaseHandle)
                    handle = (string) data.Value;
            }

            // now we can the original dynamic block record
            BlockRecord originalDynamicBlockRecord = (BlockRecord)drawing.GetObjectByHandle(handle);
            string dynamicBlockName = originalDynamicBlockRecord.Name;
            // if we need the original block instead of just the record, we can get it from the list of block since we know now its name
            Block originalBlockRecord = drawing.Blocks[dynamicBlockName];

            // the dynamic parameter of this insert was NOT modified so the block will be the original
            insert = drawing.Inserts[1];
            block = insert.Block;
            // this is the block name
            name = block.Name;
            // the list of entities contained in the block are the ones defined in the original
            entities = block.Entities;

            // remember all dynamic parameters information will be lost
            drawing.Save("Saved sample.dxf");
        }

        #endregion

        #region Samples for new and modified features 0.9.1

        public static void LoadAndSaveBlocks()
        {
            // Create a block to be used as sample
            Block baseBlk = new Block("BaseBlock");
            baseBlk.Record.Units = DrawingUnits.Millimeters;
            baseBlk.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            baseBlk.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));
            AttributeDefinition attdef = new AttributeDefinition("MyAttribute")
            {
                Prompt = "Enter a value:",
                Value = 0,
                Position = Vector3.Zero,
                Layer = new Layer("MyLayer")
                {
                    Color = AciColor.Red
                }
            };
            baseBlk.AttributeDefinitions.Add(attdef);

            // Blocks are saved in a similar way as other dxf, just pass the dxf file name, the DxfVersion, and optionally if the dxf needs to be saved in binary format
            // Only AutoCad2000 and newer versions are supported.
            // The block entities and attribute definitions will be added to the Model layout.
            // The drawing header units will be the ones defined in the block record.
            baseBlk.Save(baseBlk.Name + ".dxf", DxfVersion.AutoCad2000);

            DxfDocument dxf = new DxfDocument();

            // Blocks are loaded as any other dxf, just pass the dxf file name,
            // optionally you can also give it a name, by default the file name without extension will be used.
            // Only AutoCad2000 and newer versions are supported,
            // Only the entities contained in the Model layout will be used.
            // The block units will be the ones defined in the dxf header.
            Block block = Block.Load(baseBlk.Name + ".dxf", "MyBlock");

            // in case the loading process has failed check for null
            // In DEBUG mode the loading process will raise exceptions while in RELEASE it will just return null, the same as loading a DxfDocument
            if (block == null)
            {
                Console.WriteLine("Error loading the block dxf file.");
                Console.WriteLine("Press a key to continue...");
                Console.ReadKey();
                return;
            }

            // once the block is loaded we can use it in insert entities
            Insert insert = new Insert(block, new Vector2(10));

            // the block might also contain attribute definitions
            int attdefCount = block.AttributeDefinitions.Count;

            // this is the list of attribute definition tags
            // remember netDxf does not allow the use of duplicate tag names, although AutoCad allows it, it is not recommended
            ICollection<string> tags = block.AttributeDefinitions.Tags;

            // we can assign values to the insert attributes
            foreach (Attribute att in insert.Attributes)
            {
                att.Value = string.Format("{0} value", att.Tag);
            }

            // optionally we can manually add the block definition to the document
            dxf.Blocks.Add(block);

            // we add the insert entity to the document, if the block associated with the block has not been added this method will do it automatically
            dxf.AddEntity(insert);

            // also it is possible to manually add attribute definitions to a document
            AttributeDefinition def = new AttributeDefinition("AttDefOutsideBlock")
            {
                Prompt = "Enter value:",
                Value = 0,
                Color = AciColor.Blue,
                Position = new Vector3(0, 30, 0)
            };

            // we will add the attribute definition to the document just like anyother entity
            dxf.AddEntity(def);
            
            // now we can save our new document
            dxf.Save("CreateBlockFromDxf.dxf");

            DxfDocument load = DxfDocument.Load("CreateBlockFromDxf.dxf");

        }

        #endregion

        #region Samples for new and modified features 0.9.0

        private static void MakingGroups()
        {
            Line line1 = new Line(Vector2.Zero, Vector2.UnitX);
            Line line2 = new Line(Vector2.Zero, Vector2.UnitY);
            Group group = new Group();
            group.Entities.Add(line1);
            group.Entities.Add(line2);

            DxfDocument dxf = new DxfDocument();
            // when we add a group to the document all the entities contained in the group will be automatically added to the document
            dxf.Groups.Add(group);

            // adding the group entities to the document is not necessary, but doing so should not cause any harm
            // the AddEntity method will return false in those cases, since those entities are already in the document
            dxf.AddEntity(line1);
            dxf.AddEntity(line2);

            dxf.Save("group.dxf");

            DxfDocument load = DxfDocument.Load("group.dxf");

            Console.WriteLine("Press a key to finish...");
            Console.ReadKey();
        }
        private static void CombiningTwoDrawings()
        {
            // create first drawing
            Line line1 = new Line(Vector2.Zero, Vector2.UnitX);
            line1.Layer = new Layer("Layer01");
            line1.Layer.Color = AciColor.Blue;
            DxfDocument dxf1 = new DxfDocument();
            dxf1.AddEntity(line1);
            dxf1.Save("drawing01.dxf");

            // create second drawing
            Line line2 = new Line(Vector2.Zero, Vector2.UnitY);
            line2.Layer = new Layer("Layer02");
            line2.Layer.Color = AciColor.Red;
            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(line2);
            dxf2.Save("drawing02.dxf");

            // load the drawings that will be combined
            DxfDocument source01 = DxfDocument.Load("drawing01.dxf");
            DxfDocument source02 = DxfDocument.Load("drawing02.dxf");

            // our destination drawing
            DxfDocument combined = new DxfDocument();
            foreach (Line l in source01.Lines)
            {
                // It is recommended to make a copy of the source line before we can added to the destination drawing
                // if we do not make a copy weird things might happen if we save the original drawing again
                Line copy = (Line)l.Clone();
                combined.AddEntity(copy);
            }

            // Another safe way is removing the entity from the original drawing before adding it to the destination drawing
            Line line = source02.Lines[0];
            source02.RemoveEntity(line);
            combined.AddEntity(line);

            combined.Save("CombinedDrawing.dxf");
        }
        private static void BinaryChunkXData()
        {
            Line line = new Line(Vector2.Zero, Vector2.UnitX);

            ApplicationRegistry appId = new ApplicationRegistry("TestBinaryChunk");

            // the extended data binary data (code 1004) is stored in a different way depending if the dxf file is text or binary.
            // in text based files as a string of hexadecimal digits, two per binary byte,
            // while in binary files the data is stored in chunks of 127 bytes, preceding a byte that defines the number of bytes in the chunk
            byte[] data = new byte[325];

            // fill up the array with some random data
            Random rnd = new Random();
            rnd.NextBytes(data);

            XData xdata = new XData(appId);

            // the XDataRecord will store the binary data as a byte array and not as a string as it use to be
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.BinaryData, data));
            line.XData.Add(xdata);

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);

            dxf.Save("BinaryChunkXData.dxf");
            dxf.Save("BinaryChunkXData binary.dxf", true);

            // some testing
            DxfDocument test = DxfDocument.Load("BinaryChunkXData binary.dxf");
            Line lineTest = test.Lines[0];
            XDataRecord recordTest = lineTest.XData[appId.Name].XDataRecord[0];
            Debug.Assert(recordTest.Code == XDataCode.BinaryData);
            byte[] dataText = (byte[]) recordTest.Value;

            byte[] compare = new byte[127];
            Array.Copy(data, compare, 127);

            for (int i = 0; i < 127; i++)
            {
                Console.WriteLine(dataText[i] == compare[i]);
            }

            // this is the string as it is saved in text based dxf files
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataText.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", data[i]));
            }
            Console.WriteLine(sb.ToString());

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
        private static void BinaryDxfFiles()
        {
            // Binary dxf files preserve the accuracy of the drawing while text dxf files are saved with 17 decimals.
            // Binary dxf files are 4-5 times faster to write, while reading is just a little faster.
            // Binary dxf files take about 20% less file space.
            DxfDocument dxf = new DxfDocument();
            // optionally you can give a name to de document
            dxf.Name = "Binary dxf";
            Line line = new Line(Vector3.Zero, new Vector3(10));
            dxf.AddEntity(line);


            // To save a document as a binary dxf just set the isBinary parameter to true, by default it will always be saved as a text based dxf 
            // you can use the document name as tha file name, or just give another one.
            string file = dxf.Name + ".dxf";

            // Handling with error checking of the saving process
            bool ok = dxf.Save(file, true);
            if(ok)
                Console.WriteLine("The file \"{0}\" has been correctly saved.", file);
            else
                Console.WriteLine("Fatal error while saving \"{0}\".", file);

            Console.WriteLine();

            // Handling with error checking of the loading process

            // check if the file exists
            if (!File.Exists(file))
            {
                Console.WriteLine("The file \"{0}\" does not exists.", file);
            }
            else
            {
                bool isBinary;
                DxfVersion version = DxfDocument.CheckDxfFileVersion(file, out isBinary);

                // netDxf only supports AutoCad2000 and above.
                if (version >= DxfVersion.AutoCad2000)
                {
                    // To load a binary dxf nothing needs to be done, the reader will detect the correct type.
                    DxfDocument load = DxfDocument.Load(file);
                    if (load == null)
                        Console.WriteLine("Fatal error while loading \"{0}\".", file);
                    else
                        //when a document is loaded the file name without extension is used as the document name
                        Console.WriteLine("The file \"{0}\" version {1} has been correctly loaded.\n\tBinary? {2}", file, version, isBinary);
                }
                else
                {
                    if(version == DxfVersion.Unknown)
                        Console.WriteLine("The file \"{0}\" is not a dxf.", file);
                    else
                        Console.WriteLine("Dxf file \"{0}\" version {1} is not supported, only AutoCad2000 and above.", file, version);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();

        }
        private static void MeshEntity()
        {
            DxfDocument dxf = new DxfDocument();

            // construct a simple cube (see the AutoCad documentation for more information about creating meshes)

            // the mesh data is always defined at level 0 (no subdivision)
            // 8 vertices
            List<Vector3> vertexes = new List<Vector3>
                                            {
                                                new Vector3(-5, -5, -5),
                                                new Vector3(5, -5, -5),
                                                new Vector3(5, 5, -5),
                                                new Vector3(-5, 5, -5),
                                                new Vector3(-5, -5, 5),
                                                new Vector3(5, -5, 5),
                                                new Vector3(5, 5, 5),
                                                new Vector3(-5, 5, 5)
                                            };

            //6 faces
            List<int[]> faces = new List<int[]>
                                                {
                                                    new[] {0, 3, 2, 1},
                                                    new[] {0, 1, 5, 4},
                                                    new[] {1, 2, 6, 5},
                                                    new[] {2, 3, 7, 6},
                                                    new[] {0, 4, 7, 3},
                                                    new[] {4, 5, 6, 7}
                                                };

            

            // the list of edges is optional and only really needed when applying creases values to them
            // crease negative values will sharpen the edge independently of the subdivision level. Any negative crease value will be reseted as -1.
            // by default edge creases are set to 0.0 (no edge sharpening)
            List<MeshEdge> edges = new List<MeshEdge>
            {
                new MeshEdge(0, 1),
                new MeshEdge(1, 2),
                new MeshEdge(2, 3),
                new MeshEdge(3, 0),
                new MeshEdge(4, 5, -1.0),
                new MeshEdge(5, 6, -1.0),
                new MeshEdge(6, 7, -1.0),
                new MeshEdge(7, 4, -1.0),
                new MeshEdge(0, 4),
                new MeshEdge(1, 5),
                new MeshEdge(2, 6),
                new MeshEdge(3, 7)

            };
            Mesh mesh = new Mesh(vertexes, faces, edges);
            mesh.SubdivisionLevel = 3;

            ApplicationRegistry newAppReg = dxf.ApplicationRegistries.Add(new ApplicationRegistry("netDxf"));

            XData xdata = new XData(newAppReg);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "xdata string sample"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 50000));
            mesh.XData.Add(xdata);

            dxf.AddEntity(mesh);

            dxf.Save("mesh.dxf");

            dxf = DxfDocument.Load("mesh.dxf");

        }

        #endregion

        #region Samples for new and modified features 0.8.0

        private static void MTextEntity()
        {
            TextStyle style = new TextStyle("Arial");

            MText text1 = new MText(Vector2.Zero, 10, 0, style);
            // you can set manually the text value with all available formatting commands
            text1.Value = "{\\C71;\\c10938556;Text} with true color\\P{\\C140;Text} with indexed color";

            MText text2 = new MText(new Vector2(0, 30), 10, 0, style);
            // or use the Write() method
            MTextFormattingOptions op = new MTextFormattingOptions(text2.Style);
            op.Color = new AciColor(188, 232, 166); // using true color
            text2.Write("Text", op);
            op.Color = null; // set color to the default defined in text2.Style
            text2.Write(" with true color");
            text2.EndParagraph();
            op.Color = new AciColor(140); // using index color
            text2.Write("Text", op); // set color to the default defined in text2.Style
            op.Color = null;
            text2.Write(" with indexed color");

            // both text1 and text2 should yield to the same result
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);
            dxf.AddEntity(text1);
            dxf.AddEntity(text2);

            dxf.Save("MText format.dxf");

            // now you can retrieve the MText text value without any formatting codes, control characters like tab '\t' will be preserved in the result,
            // the new paragraph command "\P" will be converted to new line feed '\r\n'.
            Console.WriteLine(text1.PlainText());
            Console.WriteLine();
            Console.WriteLine(text2.PlainText());
            Console.WriteLine();
            Console.WriteLine("Press a key to finish...");
            Console.ReadKey();

        }
        private static void TransparencySample()
        {
            // transparencies can only be applied to entities and layer
            Layer layer = new Layer("Layer with transparency");
            layer.Color = new AciColor(Color.MediumVioletRed);
            // the transparency is expressed in percentage. Initially all Transparency values are initialized as ByLayer.
            layer.Transparency.Value = 50;
            // You cannot use the reserved values 0 and 100 that represents ByLayer and ByBlock. Use Transparency.ByLayer and Transparency.ByBlock
            // this behavior is similar to the index in AciColor or the weight in Lineweight
            // this is wrong and will rise and exception
            //layer.Transparency.Value = 0;
            // this is ok
            //layer.Transparency = Transparency.ByLayer;

            // this line will use the transparency defined in the layer to which it belongs
            Line line1 = new Line(new Vector2(-5, -5), new Vector2(5, 5));
            line1.Layer = layer;

            // this line will use its own transparency
            Line line2 = new Line(new Vector2(-5, 5), new Vector2(5, -5));
            line2.Transparency.Value = 80;

            // transparency as the true color is not supported by AutoCad2000 database version
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2004);
            dxf.AddEntity(line1);
            dxf.AddEntity(line2);

            dxf.Save("TransparencySample.dxf");

            dxf = DxfDocument.Load("TransparencySample.dxf");

        }
        private static void DocumentUnits()
        {
            DxfDocument dxf = new DxfDocument();

            // setting the LUnit variable to engineering or architectural will also set the InsUnits variable to Inches,
            // this need to be this way since AutoCad will show those units in feet and inches and will always consider the drawing base units as inches.
            // You can change again the InsUnits it at your own risk.
            // its main purpose is at the user interface level
            //dxf.DrawingVariables.LUnits = LinearUnitType.Engineering;

            // this is the recommended document unit type
            dxf.DrawingVariables.LUnits = LinearUnitType.Decimal;

            // this is the real important unit,
            // it is used when inserting blocks or images into the drawing as this and the block units will give the scale of the resulting Insert
            dxf.DrawingVariables.InsUnits = DrawingUnits.Millimeters;

            // the angle unit type is purely cosmetic as it has no influence on how the angles are stored in the dxf 
            // its purpose is only at the user interface level
            dxf.DrawingVariables.AUnits = AngleUnitType.Radians;

            // even though we have set the drawing angles in radians the dxf always stores angle data in degrees,
            // this arc goes from 45 to 270 degrees and not radians or whatever the AUnits header variable says.
            Arc arc = new Arc(Vector2.Zero, 5, 45, 270);
            // Remember, at the moment, once the entity has been added to the document is not safe to modify it, changes in some of their properties might generate problems
            dxf.AddEntity(arc);

            // the units of this line will correspond to the ones set in InsUnits
            Line lineM = new Line(new Vector2(-5, -5), new Vector2(5, 5));
            dxf.AddEntity(lineM);

            // All entities added to a block are expressed in the coordinates defined by the block
            // You can set a default unit so all new blocks will use them, the default value is Unitless
            // You might want to use the same units as the drawing, this is just a convenient way to make sure all blocks share the same units 
            BlockRecord.DefaultUnits = dxf.DrawingVariables.InsUnits;

            // In this case the line will be 10 cm long
            Line lineCm = new Line(new Vector2(-5, 0), new Vector2(5, 0));
            Block blockCm = new Block("CmBlock");
            // You can override the default units changing the block.Record.Units value
            blockCm.Record.Units = DrawingUnits.Centimeters;
            blockCm.Entities.Add(lineCm);
            Insert insCm = new Insert(blockCm);

            // In this case the line will be 10 dm long
            Line lineDm = new Line(new Vector2(0, 5), new Vector2(0, -5));
            Block blockDm = new Block("DmBlock");
            blockDm.Record.Units = DrawingUnits.Decimeters;
            // AllowExploding and ScaleUniformy properties will only be recognized by dxf version AutoCad2007 and upwards
            blockDm.Record.AllowExploding = false;
            blockDm.Record.ScaleUniformly = true;
            blockDm.Entities.Add(lineDm);
            blockDm.Entities.Add(insCm);
            Insert insDm = new Insert(blockDm);

            dxf.AddEntity(insDm);

            // the image units are stored in the raster variables units, it is recommended to use the same units as the document to avoid confusions
            dxf.RasterVariables.Units = ImageUnits.Millimeters;
            // Sometimes AutoCad does not like image file relative paths, in any case reloading the references will fix the problem
            ImageDefinition imgDefinition = new ImageDefinition("image.jpg");
            // the resolution units is only used to calculate the image resolution that will return pixels per inch or per centimeter (the use of NoUnits is not recommended).
            imgDefinition.ResolutionUnits = ImageResolutionUnits.Inches;
            // this image will be 10x10 mm in size
            Image img = new Image(imgDefinition, Vector3.Zero, 10, 10);
            dxf.AddEntity(img);

            dxf.Save("Document Units.dxf");

            DxfDocument dxfLoad = DxfDocument.Load("Document Units.dxf");

        }
        private static void PaperSpace()
        {
            // Sample on how to work with Layouts
            DxfDocument dxf = new DxfDocument();
            // A new DxfDocument will create the default "Model" layout that is associated with the ModelSpace block. This layout cannot be erased or renamed.
            Line line = new Line(new Vector2(0), new Vector2(100));
            // The line will be added to the "Model" layout since this is the active one by default.
            dxf.AddEntity(line);

            // Create a new Layout, all new layouts will be associated with different PaperSpace blocks,
            // while there can be only one ModelSpace multiple PaperSpace blocks might exist in the document
            Layout layout1 = new Layout("Layout1");

            // When the layout is added to the list, a new PaperSpace block will be created automatically
            dxf.Layouts.Add(layout1);
            // Set this new Layout as the active one. All entities will now be added to this layout.
            dxf.ActiveLayout = layout1.Name;

            // Create a viewport, this is the window to the ModelSpace
            Viewport viewport1 = new Viewport
                {
                    Width = 100,
                    Height = 100,
                    Center = new Vector3(50, 50, 0),
                };

            // Add it to the "Layout1" since this is the active one
            dxf.AddEntity(viewport1);
            // Also add a circle
            Circle circle = new Circle(new Vector2(150), 25);
            dxf.AddEntity(circle);

            // Create a second Layout, add it to the list, and set it as the active one.
            Layout layout2 = new Layout("Layout2");
            dxf.Layouts.Add(layout2);
            dxf.ActiveLayout = layout2.Name;

            // viewports might have a non rectangular boundary, in this case we will use an ellipse.
            Ellipse ellipse = new Ellipse(new Vector2(100), 200, 150);
            Viewport viewport2 = new Viewport
            {
                ClippingBoundary = ellipse,
            };

            // Add the viewport to the document. This will also add the ellipse to the document.
            dxf.AddEntity(viewport2);

            Layout layout3 = new Layout("AnyName");
            dxf.Layouts.Add(layout3);
            //layout can also be renamed
            layout3.Name = "Layout3";
            
            //dxf.Layouts.Remove(layout2.Name);

            ShowDxfDocumentInformation(dxf);

            // Save the document as always.
            dxf.Save("PaperSpace.dxf");

#region CAUTION - This is subject to change in the future, use it with care

            // You cannot directly remove the ellipse from the document since it has been attached to a viewport
            bool ok = dxf.RemoveEntity(ellipse); // OK = false

            // If an entity has been attached to another, its reactor will point to its owner
            // This information is subject to change in the future to become a list, an entity can be attached to multiple objects;
            // but at the moment only the viewport clipping boundary make use of this.
            // This is the way AutoCad also handles hatch and dimension associativity, that I might implement in the future
            DxfObject reactor = ellipse.Reactors[0]; // in this case reactor points to viewport2

            // You need to delete the viewport instead. This deletes the viewport and the ellipse
            //dxf.RemoveEntity(viewport2);

            // another way of deleting the ellipse, is first to assign another clipping boundary to the viewport or just set it to null
            viewport2.ClippingBoundary = null;
            // now it will be possible to delete the ellipse. This will not delete the viewport.
            ok = dxf.RemoveEntity(ellipse); // OK = true

            // Save the document if you want to test the changes
            dxf.Save("PaperSpace.dxf");

#endregion

            DxfDocument dxfLoad = DxfDocument.Load("PaperSpace.dxf");

            // For every entity you can check its layout
            // The entity Owner will return the block to which it belongs, it can be a *Model_Space, *Paper_Space, ... or a common block if the entity is part of its geometry.
            // The block record stores information about the block and one of them is the layout, this mimics the way the dxf stores this information.
            // Remember only the internal blocks *Model_Space, *Paper_Space, *Paper_Space0, *Paper_Space1, ... have an associated layout,
            // all other blocks will return null is asked for block.Record.Layout
            Layout associatedLayout = dxfLoad.Lines[0].Owner.Record.Layout;

            // or you can get the complete list of entities of a layout
            foreach (Layout layout in dxfLoad.Layouts)
            {
                List<DxfObject> entities = dxfLoad.Layouts.GetReferences(layout.Name); 
            }

            // You can also remove any layout from the list, except the "Model".
            // Remember all entities that has been added to this layout will also be removed.
            // This mimics the behavior in AutoCad, when a layout is deleted all entities in it will also be deleted.
            dxfLoad.Layouts.Remove(layout1.Name);

            Layout layout4 = (Layout) layout2.Clone("Layout4");
            dxfLoad.Layouts.Add(layout4);

            ShowDxfDocumentInformation(dxfLoad);

            dxfLoad.Save("PaperSpace removed.dxf");

        }
        private static void BlockWithAttributes()
        {
            DxfDocument dxf = new DxfDocument();
            Block block = new Block("BlockWithAttributes");
            block.Layer = new Layer("BlockSample");
            // It is possible to change the block position, even though it is recommended to keep it at Vector3.Zero,
            // since the block geometry is expressed in local coordinates of the block.
            // The block position defines the base point when inserting an Insert entity.
            block.Origin = new Vector3(10, 5, 0);

            // create an attribute definition, the attDef tag must be unique as it is the way to identify the attribute.
            // even thought AutoCad allows multiple attribute definition in block definitions, it is not recommended
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            // this is the text prompt shown to introduce the attribute value when a new Insert entity is inserted into the drawing
            attdef.Prompt = "InfoText";
            // optionally we can set a default value for new Insert entities
            attdef.Value = 0;
            // the attribute definition position is in local coordinates to the Insert entity to which it belongs
            attdef.Position = new Vector3(1, 1, 0);

            // modifying directly the text style might not get the desired results. Create one or get one from the text style table, modify it and assign it to the attribute text style.
            // one thing to note, if there is already a text style with the assigned name, the existing one in the text style table will override the new one.
            //attdef.Style.IsVertical = true;

            TextStyle txt = new TextStyle("MyStyle", "Arial.ttf");
            txt.IsVertical = true;
            attdef.Style = txt;
            attdef.WidthFactor = 2;
            // not all alignment options are available for TTF fonts 
            attdef.Alignment = TextAlignment.MiddleCenter;
            attdef.Rotation = 90;

            // remember, netDxf does not allow adding attribute definitions with the same tag, even thought AutoCad allows this behavior, it is not recommended in anyway.
            // internally attributes and their associated attribute definitions are handled through dictionaries,
            // and the tags work as ids to easily identify the information stored in the attribute value.
            // When reading a file the attributes or attribute definitions with duplicate tags will be automatically removed.
            // This is subject to change on public demand, it is possible to reimplement this behavior with simple collections to allow for duplicate tags.
            block.AttributeDefinitions.Add(attdef);

            // The entities list defines the actual geometry of the block, they are expressed in th block local coordinates
            Line line1 = new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0));
            Line line2 = new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0));
            block.Entities.Add(line1);
            block.Entities.Add(line2);

            // You can check the entity ownership with:
            Block line1Owner = line1.Owner;
            Block line2Owner = line2.Owner;
            // in this example line1Oner = line2Owner = block
            // As explained in the PaperSpace() sample, the layout associated with a common block will always be null
            Layout associatedLayout = line1.Owner.Record.Layout;
            // associatedLayout = null

            // create an Insert entity with the block definition, during the initialization the Insert attributes list will be created with the default attdef properties
            Insert insert1 = new Insert(block)
            {
                Position = new Vector3(5, 5, 5),
                Normal = new Vector3(1, 1, 1),
                Rotation = 45
            };

            // When the insert position, rotation, normal and/or scale are modified we need to transform the attributes.
            // It is not recommended to manually change the attribute position and orientation and let the Insert entity handle the transformations to maintain them in the same local position.
            // In this particular case we have changed the position, normal and rotation.
            insert1.TransformAttributes();
            
            // Once the insert has been created we can modify the attributes properties, the list cannot be modified only the items stored in it
            insert1.Attributes[0].Value = 24;

            // Modifying directly the layer might not get the desired results. Create one or get one from the layers table, modify it and assign it to the insert
            // One thing to note, if there is already a layer with the same name, the existing one in the layers table will override the new one, when the entity is added to the document.
            Layer layer = new Layer("MyInsertLayer");
            layer.Color.Index = 4;

            // optionally we can add the new layer to the document, if not the new layer will be added to the Layers collection when the insert entity is added to the document
            // in case a new layer is found in the list the add method will return the layer already stored in the list
            // this behavior is similar for all TableObject elements, all table object names must be unique (case insensitive)
            layer = dxf.Layers.Add(layer);

            // assign the new layer to the insert
            insert1.Layer = layer;

            // add the entity to the document
            dxf.AddEntity(insert1);

            // create a second insert entity
            // the constructor will automatically reposition the insert2 attributes to the insert local position
            Insert insert2 = new Insert(block, new Vector3(10, 5, 0));

            // as before now we can change the insert2 attribute value
            insert2.Attributes[0].Value = 34;

            // additionally we can insert extended data information
            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0.0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0.0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0.0));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);

            insert2.XData.Add(xdata1);
            dxf.AddEntity(insert2);

            // all entities support this feature
            XData xdata2 = new XData(new ApplicationRegistry("MyApplication1"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            // multiple extended data entries might be added
            XData xdata3 = new XData(new ApplicationRegistry("MyApplication2"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata3.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata3.XDataRecord.Add(XDataRecord.CloseControlString);

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("MyCircleLayer");
            // AutoCad 2000 does not support true colors, in that case an approximated color index will be used instead
            circle.Layer.Color = new AciColor(Color.MediumSlateBlue);
            circle.XData.Add(xdata2);
            circle.XData.Add(xdata3);

            dxf.AddEntity(circle);

            dxf.Save("BlockWithAttributes.dxf");
            DxfDocument dxfLoad = DxfDocument.Load("BlockWithAttributes.dxf");
        }

        #endregion

        private static void ShowDxfDocumentInformation(DxfDocument dxf)
        {
            Console.WriteLine("FILE VERSION: {0}", dxf.DrawingVariables.AcadVer);
            Console.WriteLine();
            Console.WriteLine("FILE COMMENTS: {0}", dxf.Comments.Count);
            foreach (var o in dxf.Comments)
            {
                Console.WriteLine("\t{0}", o);
            }
            Console.WriteLine();
            Console.WriteLine("FILE TIME:");
            Console.WriteLine("\tdrawing created (UTC): {0}.{1}", dxf.DrawingVariables.TduCreate, dxf.DrawingVariables.TduCreate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing last update (UTC): {0}.{1}", dxf.DrawingVariables.TduUpdate, dxf.DrawingVariables.TduUpdate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing edition time: {0}", dxf.DrawingVariables.TdinDwg);
            Console.WriteLine();    
            Console.WriteLine("APPLICATION REGISTRIES: {0}", dxf.ApplicationRegistries.Count);
            foreach (var o in dxf.ApplicationRegistries)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.ApplicationRegistries.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LAYERS: {0}", dxf.Layers.Count);
            foreach (var o in dxf.Layers)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layers.GetReferences(o).Count);
                Debug.Assert(ReferenceEquals(o.LineType, dxf.LineTypes[o.LineType.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("LINE TYPES: {0}", dxf.LineTypes.Count);
            foreach (var o in dxf.LineTypes)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.LineTypes.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("TEXT STYLES: {0}", dxf.TextStyles.Count);
            foreach (var o in dxf.TextStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.TextStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DIMENSION STYLES: {0}", dxf.DimensionStyles.Count);
            foreach (var o in dxf.DimensionStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.DimensionStyles.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.DIMTXSTY, dxf.TextStyles[o.DIMTXSTY.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTYPE, dxf.LineTypes[o.DIMLTYPE.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTEX1, dxf.LineTypes[o.DIMLTEX1.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTEX2, dxf.LineTypes[o.DIMLTEX2.Name]), "Object reference not equal.");
                if (o.DIMBLK != null) Debug.Assert(ReferenceEquals(o.DIMBLK, dxf.Blocks[o.DIMBLK.Name]), "Object reference not equal.");
                if (o.DIMBLK1 != null) Debug.Assert(ReferenceEquals(o.DIMBLK1, dxf.Blocks[o.DIMBLK1.Name]), "Object reference not equal.");
                if (o.DIMBLK2 != null) Debug.Assert(ReferenceEquals(o.DIMBLK2, dxf.Blocks[o.DIMBLK2.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("MLINE STYLES: {0}", dxf.MlineStyles.Count);
            foreach (var o in dxf.MlineStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.MlineStyles.GetReferences(o.Name).Count);
                foreach (var e in o.Elements)
                {
                    Debug.Assert(ReferenceEquals(e.LineType, dxf.LineTypes[e.LineType.Name]), "Object reference not equal.");
                }
            }
            Console.WriteLine();

            Console.WriteLine("UCSs: {0}", dxf.UCSs.Count);
            foreach (var o in dxf.UCSs)
            {
                Console.WriteLine("\t{0}", o.Name);
            }
            Console.WriteLine();

            Console.WriteLine("BLOCKS: {0}", dxf.Blocks.Count);
            foreach (var o in dxf.Blocks)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Blocks.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.Layer, dxf.Layers[o.Layer.Name]), "Object reference not equal.");

                foreach (var e in o.Entities)
                {
                    Debug.Assert(ReferenceEquals(e.Layer, dxf.Layers[e.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.LineType, dxf.LineTypes[e.LineType.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.Owner, dxf.Blocks[o.Name]), "Object reference not equal.");
                    foreach (var x in e.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }

                    Text txt = e as Text;
                    if (txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }

                foreach (var a in o.AttributeDefinitions.Values)
                {
                    Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                    foreach (var x in a.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("LAYOUTS: {0}", dxf.Layouts.Count);
            foreach (var o in dxf.Layouts)
            {
                Debug.Assert(ReferenceEquals(o.AssociatedBlock, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");

                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layouts.GetReferences(o.Name).Count);
                List<DxfObject> entities = dxf.Layouts.GetReferences(o.Name);
                foreach (var e in entities)
                {
                    EntityObject entity = e as EntityObject;
                    if (entity != null)
                    {
                        Debug.Assert(ReferenceEquals(entity.Layer, dxf.Layers[entity.Layer.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.LineType, dxf.LineTypes[entity.LineType.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.Owner, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");
                        foreach (var x in entity.XData.Values)
                        {
                            Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                        }
                    }

                    Text txt = e as Text;
                    if(txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("IMAGE DEFINITIONS: {0}", dxf.ImageDefinitions.Count);
            foreach (var o in dxf.ImageDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.ImageDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DGN UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDgnDefinitions.Count);
            foreach (var o in dxf.UnderlayDgnDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDgnDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DWF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDwfDefinitions.Count);
            foreach (var o in dxf.UnderlayDwfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDwfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("PDF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayPdfDefinitions.Count);
            foreach (var o in dxf.UnderlayPdfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayPdfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("GROUPS: {0}", dxf.Groups.Count);
            foreach (var o in dxf.Groups)
            {
                Console.WriteLine("\t{0}; Entities count: {1}", o.Name, o.Entities.Count);
            }
            Console.WriteLine();

            // the entities lists contain the geometry that has a graphical representation in the drawing across all layouts,
            // to get the entities that belongs to a specific layout you can get the references through the Layouts.GetReferences(name)
            // or check the EntityObject.Owner.Record.Layout property
            Console.WriteLine("ENTITIES:");
            Console.WriteLine("\t{0}; count: {1}", EntityType.Arc, dxf.Arcs.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.AttributeDefinition, dxf.AttributeDefinitions.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Circle, dxf.Circles.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Dimension, dxf.Dimensions.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ellipse, dxf.Ellipses.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Face3D, dxf.Faces3d.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Hatch, dxf.Hatches.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Image, dxf.Images.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Insert, dxf.Inserts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Leader, dxf.Leaders.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.LightWeightPolyline, dxf.LwPolylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Line, dxf.Lines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Mesh, dxf.Meshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MLine, dxf.MLines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MText, dxf.MTexts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Point, dxf.Points.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.PolyfaceMesh, dxf.PolyfaceMeshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Polyline, dxf.Polylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Solid, dxf.Solids.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Spline, dxf.Splines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Text, dxf.Texts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ray, dxf.Rays.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Underlay, dxf.Underlays.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Viewport, dxf.Viewports.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Wipeout, dxf.Wipeouts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.XLine, dxf.XLines.Count);
            Console.WriteLine();

            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
        }

        private static DxfDocument Test(string file, string output = null)
        {    
            // optionally you can save the information to a text file
            bool outputLog = !string.IsNullOrEmpty(output);
            TextWriter writer = null;
            if (outputLog)
            {
                writer = new StreamWriter(File.Create(output));
                Console.SetOut(writer);
            }

            // check if the dxf actually exists
            FileInfo fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                Console.WriteLine("THE FILE {0} DOES NOT EXIST", file);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);

            // check if the file is a dxf
            if (dxfVersion == DxfVersion.Unknown)
            {
                Console.WriteLine("THE FILE {0} IS NOT A VALID DXF OR THE DXF DOES NOT INCLUDE VERSION INFORMATION IN THE HEADER SECTION", file);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            // check if the dxf file version is supported
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                Console.WriteLine("THE FILE {0} IS NOT A SUPPORTED DXF", file);
                Console.WriteLine();

                Console.WriteLine("FILE VERSION: {0}", dxfVersion);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            DxfDocument dxf = DxfDocument.Load(file);

            // check if there has been any problems loading the file,
            // this might be the case of a corrupt file or a problem in the library
            if (dxf == null)
            {
                Console.WriteLine("ERROR LOADING {0}", file);
                Console.WriteLine();

                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            // the dxf has been properly loaded, let's show some information about it
            Console.WriteLine("FILE NAME: {0}", file);
            Console.WriteLine("\tbinary dxf: {0}", isBinary);
            Console.WriteLine();            
            Console.WriteLine("FILE VERSION: {0}", dxf.DrawingVariables.AcadVer);
            Console.WriteLine();
            Console.WriteLine("FILE COMMENTS: {0}", dxf.Comments.Count);
            foreach (var o in dxf.Comments)
            {
                Console.WriteLine("\t{0}", o);
            }
            Console.WriteLine();
            Console.WriteLine("FILE TIME:");
            Console.WriteLine("\tdrawing created (UTC): {0}.{1}", dxf.DrawingVariables.TduCreate, dxf.DrawingVariables.TduCreate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing last update (UTC): {0}.{1}", dxf.DrawingVariables.TduUpdate, dxf.DrawingVariables.TduUpdate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing edition time: {0}", dxf.DrawingVariables.TdinDwg);
            Console.WriteLine();    
            Console.WriteLine("APPLICATION REGISTRIES: {0}", dxf.ApplicationRegistries.Count);
            foreach (var o in dxf.ApplicationRegistries)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.ApplicationRegistries.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LAYERS: {0}", dxf.Layers.Count);
            foreach (var o in dxf.Layers)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layers.GetReferences(o).Count);
                Debug.Assert(ReferenceEquals(o.LineType, dxf.LineTypes[o.LineType.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("LINE TYPES: {0}", dxf.LineTypes.Count);
            foreach (var o in dxf.LineTypes)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.LineTypes.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("TEXT STYLES: {0}", dxf.TextStyles.Count);
            foreach (var o in dxf.TextStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.TextStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DIMENSION STYLES: {0}", dxf.DimensionStyles.Count);
            foreach (var o in dxf.DimensionStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.DimensionStyles.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.DIMTXSTY, dxf.TextStyles[o.DIMTXSTY.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTYPE, dxf.LineTypes[o.DIMLTYPE.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTEX1, dxf.LineTypes[o.DIMLTEX1.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DIMLTEX2, dxf.LineTypes[o.DIMLTEX2.Name]), "Object reference not equal.");
                if (o.DIMBLK != null) Debug.Assert(ReferenceEquals(o.DIMBLK, dxf.Blocks[o.DIMBLK.Name]), "Object reference not equal.");
                if (o.DIMBLK1 != null) Debug.Assert(ReferenceEquals(o.DIMBLK1, dxf.Blocks[o.DIMBLK1.Name]), "Object reference not equal.");
                if (o.DIMBLK2 != null) Debug.Assert(ReferenceEquals(o.DIMBLK2, dxf.Blocks[o.DIMBLK2.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("MLINE STYLES: {0}", dxf.MlineStyles.Count);
            foreach (var o in dxf.MlineStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.MlineStyles.GetReferences(o.Name).Count);
                foreach (var e in o.Elements)
                {
                    Debug.Assert(ReferenceEquals(e.LineType, dxf.LineTypes[e.LineType.Name]), "Object reference not equal.");
                }
            }
            Console.WriteLine();

            Console.WriteLine("UCSs: {0}", dxf.UCSs.Count);
            foreach (var o in dxf.UCSs)
            {
                Console.WriteLine("\t{0}", o.Name);
            }
            Console.WriteLine();

            Console.WriteLine("BLOCKS: {0}", dxf.Blocks.Count);
            foreach (var o in dxf.Blocks)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Blocks.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.Layer, dxf.Layers[o.Layer.Name]), "Object reference not equal.");

                foreach (var e in o.Entities)
                {
                    Debug.Assert(ReferenceEquals(e.Layer, dxf.Layers[e.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.LineType, dxf.LineTypes[e.LineType.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.Owner, dxf.Blocks[o.Name]), "Object reference not equal.");
                    foreach (var x in e.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }

                    Text txt = e as Text;
                    if (txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }

                foreach (var a in o.AttributeDefinitions.Values)
                {
                    Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                    foreach (var x in a.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("LAYOUTS: {0}", dxf.Layouts.Count);
            foreach (var o in dxf.Layouts)
            {
                Debug.Assert(ReferenceEquals(o.AssociatedBlock, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");

                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layouts.GetReferences(o.Name).Count);
                List<DxfObject> entities = dxf.Layouts.GetReferences(o.Name);
                foreach (var e in entities)
                {
                    EntityObject entity = e as EntityObject;
                    if (entity != null)
                    {
                        Debug.Assert(ReferenceEquals(entity.Layer, dxf.Layers[entity.Layer.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.LineType, dxf.LineTypes[entity.LineType.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.Owner, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");
                        foreach (var x in entity.XData.Values)
                        {
                            Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                        }
                    }

                    Text txt = e as Text;
                    if(txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.LineType, dxf.LineTypes[a.LineType.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("IMAGE DEFINITIONS: {0}", dxf.ImageDefinitions.Count);
            foreach (var o in dxf.ImageDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.ImageDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DGN UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDgnDefinitions.Count);
            foreach (var o in dxf.UnderlayDgnDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDgnDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DWF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDwfDefinitions.Count);
            foreach (var o in dxf.UnderlayDwfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDwfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("PDF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayPdfDefinitions.Count);
            foreach (var o in dxf.UnderlayPdfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayPdfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("GROUPS: {0}", dxf.Groups.Count);
            foreach (var o in dxf.Groups)
            {
                Console.WriteLine("\t{0}; Entities count: {1}", o.Name, o.Entities.Count);
            }
            Console.WriteLine();

            // the entities lists contain the geometry that has a graphical representation in the drawing across all layouts,
            // to get the entities that belongs to a specific layout you can get the references through the Layouts.GetReferences(name)
            // or check the EntityObject.Owner.Record.Layout property
            Console.WriteLine("ENTITIES:");
            Console.WriteLine("\t{0}; count: {1}", EntityType.Arc, dxf.Arcs.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.AttributeDefinition, dxf.AttributeDefinitions.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Circle, dxf.Circles.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Dimension, dxf.Dimensions.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ellipse, dxf.Ellipses.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Face3D, dxf.Faces3d.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Hatch, dxf.Hatches.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Image, dxf.Images.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Insert, dxf.Inserts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Leader, dxf.Leaders.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.LightWeightPolyline, dxf.LwPolylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Line, dxf.Lines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Mesh, dxf.Meshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MLine, dxf.MLines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MText, dxf.MTexts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Point, dxf.Points.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.PolyfaceMesh, dxf.PolyfaceMeshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Polyline, dxf.Polylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Solid, dxf.Solids.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Spline, dxf.Splines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Text, dxf.Texts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ray, dxf.Rays.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Underlay, dxf.Underlays.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Viewport, dxf.Viewports.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Wipeout, dxf.Wipeouts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.XLine, dxf.XLines.Count);
            Console.WriteLine();

            // the dxf version is controlled by the DrawingVariables property of the dxf document,
            // also a HeaderVariables instance or a DxfVersion can be passed to the constructor to initialize a new DxfDocument.
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            dxf.Save("sample 2013.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("sample 2010.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("sample 2007.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("sample 2004.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("sample 2000.dxf");

            // saving to binary dxf
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("binary test.dxf", true);
            DxfDocument test = DxfDocument.Load("binary test.dxf");

            if (outputLog)
            {
                writer.Flush();
                writer.Close();
            }
            else
            {
                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();
            }
            return dxf;
        }

        //private static void ExplodeInsert()
        //{
        //    DxfDocument dxf = DxfDocument.Load("explode\\ExplodeInsertUniformScale.dxf");

        //    List<DxfObject> refs = dxf.Blocks.References["ExplodeBlock"];
        //    Insert insert = (Insert)refs[0];
        //    dxf.RemoveEntity(insert);
        //    insert.Layer = new Layer("Original block");
        //    insert.Layer.Color = AciColor.DarkGrey;
        //    dxf.AddEntity(insert);
        //    List<Entity> explodedEntities = insert.Explode();
        //    dxf.AddEntity(explodedEntities);

        //    dxf.Save("ExplodeInsert.dxf");
        //}

        private static void LinearDimensionTests()
        {
            DxfDocument dxf1 = new DxfDocument();
            Vector2 pt1 = new Vector2(15, -5);
            Vector2 pt2 = new Vector2(5, 5);
            double offset = 10;

            LinearDimension ld1z = new LinearDimension(pt1, pt2, offset, 30);
            LinearDimension ld2z = new LinearDimension(pt1, pt2, offset, 45);
            LinearDimension ld3z = new LinearDimension(pt1, pt2, offset, 90);
            LinearDimension ld4z = new LinearDimension(pt1, pt2, offset, 135);
            LinearDimension ld5z = new LinearDimension(pt1, pt2, offset, 180);
            LinearDimension ld6z = new LinearDimension(pt1, pt2, offset, 220);
            LinearDimension ld7z = new LinearDimension(pt2, pt1, offset, 270);

            dxf1.AddEntity(ld1z);
            dxf1.AddEntity(ld2z);
            dxf1.AddEntity(ld3z);
            dxf1.AddEntity(ld4z);
            dxf1.AddEntity(ld5z);
            dxf1.AddEntity(ld6z);
            dxf1.AddEntity(ld7z);

            Line line = new Line(pt1, pt2);
            line.Color = AciColor.Yellow;
            dxf1.AddEntity(line);

            dxf1.Save("test2.dxf");

            DxfDocument dxf2 = new DxfDocument();

            LinearDimension ld1 = new LinearDimension(new Vector2(0, 0), new Vector2(0, 15), 1, 90);
            LinearDimension ld1b = new LinearDimension(new Vector2(0, 0), new Vector2(0, 15), 1, 100);
            LinearDimension ld1c = new LinearDimension(new Vector2(0, 0), new Vector2(0, 15), 1, 80);

            LinearDimension ld2 = new LinearDimension(new Vector2(5, 15), new Vector2(5, 0), 1, 90);
            LinearDimension ld3 = new LinearDimension(new Vector2(10, 0), new Vector2(10, 15), -1, 270);

            LinearDimension ld4 = new LinearDimension(new Vector2(15, 0), new Vector2(15, 15), 1, 270);
            LinearDimension ld4b = new LinearDimension(new Vector2(15, 0), new Vector2(15, 15), 1, 300);
            LinearDimension ld4c = new LinearDimension(new Vector2(15, 0), new Vector2(15, 15), 1, 240);

            LinearDimension ld5 = new LinearDimension(new Vector2(15, 0), new Vector2(0, 0), 1, 0);
            LinearDimension ld6 = new LinearDimension(new Vector2(0, 0), new Vector2(15, 0), -1, 0);

            AlignedDimension ld1a = new AlignedDimension(new Vector2(0, 0), new Vector2(0, 15), 1);
            ld1a.Color = AciColor.Yellow;
            AlignedDimension ld2a = new AlignedDimension(new Vector2(5, 15), new Vector2(5, 0), 1);
            ld2a.Color = AciColor.Yellow;
            AlignedDimension ld3a = new AlignedDimension(new Vector2(10, 0), new Vector2(10, 15), -1);
            ld3a.Color = AciColor.Yellow;
            AlignedDimension ld4a = new AlignedDimension(new Vector2(15, 0), new Vector2(15, 15), 1);
            ld4a.Color = AciColor.Yellow;
            AlignedDimension ld5a = new AlignedDimension(new Vector2(15, 0), new Vector2(0, 0), 1);
            ld5a.Color = AciColor.Yellow;
            AlignedDimension ld6a = new AlignedDimension(new Vector2(0, 0), new Vector2(15, 0), -1);
            ld6a.Color = AciColor.Yellow;

            dxf2.AddEntity(ld1);
            dxf2.AddEntity(ld1b);
            dxf2.AddEntity(ld1c);

            dxf2.AddEntity(ld2);
            dxf2.AddEntity(ld3);

            dxf2.AddEntity(ld4);
            dxf2.AddEntity(ld4b);
            dxf2.AddEntity(ld4c);

            dxf2.AddEntity(ld5);
            dxf2.AddEntity(ld6);

            dxf2.AddEntity(ld1a);
            dxf2.AddEntity(ld2a);
            dxf2.AddEntity(ld3a);
            dxf2.AddEntity(ld4a);
            dxf2.AddEntity(ld5a);
            dxf2.AddEntity(ld6a);

            dxf2.Save("test1.dxf");
        }
        private static void TestingTrueTypeFonts()
        {
            DxfDocument dxfText = new DxfDocument();
            TextStyle textStyle1 = new TextStyle("arial.ttf");
            TextStyle textStyle2 = new TextStyle("arialbi.ttf");
            TextStyle textStyle3 = new TextStyle("92642.ttf");
            Text text1 = new Text("testing", Vector2.Zero, 6, textStyle1);
            Text text2 = new Text("testing", Vector2.Zero, 6, textStyle2);
            Text text3 = new Text("testing", Vector2.Zero, 6, textStyle3);
            dxfText.AddEntity(text1);
            dxfText.AddEntity(text2);
            dxfText.AddEntity(text3);
            dxfText.Save("text3.dxf");

            DxfDocument load = DxfDocument.Load("text3.dxf");
            load.Save("test.dxf");

        }
        private static void EncodingTest()
        {
            DxfDocument dxf;
            dxf = DxfDocument.Load("tests//EncodeDecodeProcess (cad 2010).dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("EncodeDecodeProcess (netDxf 2000).dxf");

            dxf = DxfDocument.Load("tests//EncodeDecodeProcess (cad 2000).dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("EncodeDecodeProcess (netDxf 2010).dxf");
        }
        private static void CheckReferences()
        {
            DxfDocument dxf = new DxfDocument();
            
            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.Layer = layer1;
            dxf.AddEntity(poly);

            Ellipse ellipse = new Ellipse(new Vector3(2, 2, 0), 5, 3);
            ellipse.Rotation = 30;
            ellipse.Layer = layer1;
            dxf.AddEntity(ellipse);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));
            line.Layer = layer2;
            line.LineType = LineType.DashDot;
            dxf.AddEntity(line);

            dxf.Save("test.dxf");

            dxf = DxfDocument.Load("sample.dxf");

            foreach (ApplicationRegistry registry in dxf.ApplicationRegistries)
            {
                foreach (DxfObject o in dxf.ApplicationRegistries.GetReferences(registry))
                {
                    if (o is EntityObject)
                    {
                        foreach (KeyValuePair<string, XData> data in ((EntityObject)o).XData)
                        {
                            if (data.Key == registry.Name)
                                if (!ReferenceEquals(registry, data.Value.ApplicationRegistry))
                                    Console.WriteLine("Application registry {0} not equal entity to {1}", registry.Name, o.CodeName);
                        }
                    }
                }
            }

            foreach (Block block in dxf.Blocks)
            {
                foreach (DxfObject o in dxf.Blocks.GetReferences(block))
                {
                    if (o is Insert)
                        if (!ReferenceEquals(block, ((Insert)o).Block))
                            Console.WriteLine("Block {0} not equal entity to {1}", block.Name, o.CodeName);
                }
            }

            foreach (ImageDefinition def in dxf.ImageDefinitions)
            {
                foreach (DxfObject o in dxf.ImageDefinitions.GetReferences(def))
                {
                    if (o is Image)
                        if (!ReferenceEquals(def, ((Image)o).Definition))
                            Console.WriteLine("Image definition {0} not equal entity to {1}", def.Name, o.CodeName);
                }
            }

            foreach (DimensionStyle dimStyle in dxf.DimensionStyles)
            {
                foreach (DxfObject o in dxf.DimensionStyles.GetReferences(dimStyle))
                {
                    if (o is Dimension)
                        if (!ReferenceEquals(dimStyle, ((Dimension)o).Style))
                            Console.WriteLine("Dimension style {0} not equal entity to {1}", dimStyle.Name, o.CodeName);
                }

            }

            foreach (Group g in dxf.Groups)
            {
                foreach (DxfObject o in dxf.Groups.GetReferences(g))
                {
                    // no references
                }
            }

            foreach (UCS u in dxf.UCSs)
            {
                foreach (DxfObject o in dxf.UCSs.GetReferences(u))
                {
                    
                }
            }

            foreach (TextStyle style in dxf.TextStyles)
            {
                foreach (DxfObject o in dxf.TextStyles.GetReferences(style))
                {
                    if (o is Text)
                        if (!ReferenceEquals(style, ((Text)o).Style))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);

                    if (o is MText)
                        if (!ReferenceEquals(style, ((MText)o).Style))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);

                    if (o is DimensionStyle)
                        if (!ReferenceEquals(style, ((DimensionStyle)o).DIMTXSTY))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);
                }
            }

            foreach (Layer layer in dxf.Layers)
            {
                foreach (DxfObject o in dxf.Layers.GetReferences(layer))
                {
                    if (o is Block)
                        if (!ReferenceEquals(layer, ((Block)o).Layer))
                            Console.WriteLine("Layer {0} not equal entity to {1}", layer.Name, o.CodeName);
                    if (o is EntityObject)
                        if (!ReferenceEquals(layer, ((EntityObject)o).Layer))
                            Console.WriteLine("Layer {0} not equal entity to {1}", layer.Name, o.CodeName);
                }
            }

            foreach (LineType lType in dxf.LineTypes)
            {
                foreach (DxfObject o in dxf.LineTypes.GetReferences(lType))
                {
                    if (o is Layer)
                        if (!ReferenceEquals(lType, ((Layer)o).LineType))
                            Console.WriteLine("Line type {0} not equal to {1}", lType.Name, o.CodeName);
                    if (o is MLineStyle)
                    {
                        foreach (MLineStyleElement e in ((MLineStyle)o).Elements)
                        {
                            if (!ReferenceEquals(lType, e.LineType))
                                Console.WriteLine("Line type {0} not equal to {1}", lType.Name, o.CodeName);
                        }
                    }
                    if (o is EntityObject)
                        if (!ReferenceEquals(lType, ((EntityObject)o).LineType))
                            Console.WriteLine("Line type {0} not equal entity to {1}", lType.Name, o.CodeName);

                }
            }

            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
        private static void DimensionNestedBlock()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            LinearDimension dim = new LinearDimension(line, 7, 0.0, myStyle);

            Block nestedBlock = new Block("NestedBlock");
            nestedBlock.Entities.Add(line);
            Insert nestedIns = new Insert(nestedBlock);

            Block block = new Block("MyBlock");
            block.Entities.Add(dim);
            block.Entities.Add(nestedIns);

            Insert ins = new Insert(block);
            ins.Position = new Vector3(10, 10, 0);
            dxf.AddEntity(ins);

            Circle circle = new Circle(p2, 5);
            Block block2 = new Block("MyBlock2");
            block2.Entities.Add(circle);

            Insert ins2 = new Insert(block2);
            ins2.Position = new Vector3(-10, -10, 0);
            dxf.AddEntity(ins2);

            Block block3 = new Block("MyBlock3");
            block3.Entities.Add((EntityObject)ins.Clone());
            block3.Entities.Add((EntityObject)ins2.Clone());

            Insert ins3 = new Insert(block3);
            ins3.Position = new Vector3(-10, 10, 0);
            dxf.AddEntity(ins3);

            dxf.Save("nested blocks.dxf");

            dxf = DxfDocument.Load("nested blocks.dxf");

            dxf.Save("nested blocks.dxf");
        }
        private static void ComplexHatch()
        {
            HatchPattern pattern = HatchPattern.FromFile("hatch\\acad.pat", "ESCHER");
            pattern.Scale = 1.5;
            pattern.Angle = 30;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                {
                    new HatchBoundaryPath(new List<EntityObject> {poly})
                };
            Hatch hatch = new Hatch(pattern, boundary, true);
            
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(poly);
            dxf.AddEntity(hatch);
            dxf.Save("complexhatch.dxf");

            DxfDocument dxf2 = DxfDocument.Load("complexhatch.dxf");
            dxf2.Save("complexhatch2.dxf");

        }
        private static void RayAndXLine()
        {
            Ray ray = new Ray(new Vector3(1, 1, 1), new Vector3(1, 1, 1));
            XLine xline = new XLine(Vector2.Zero, new Vector2(1,1));

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(ray);
            dxf.AddEntity(xline);
            dxf.Save("RayAndXLine.dxf");


            dxf = DxfDocument.Load("RayAndXLine.dxf");

        }
        private static void UserCoordinateSystems()
        {
            DxfDocument dxf = new DxfDocument();
            UCS ucs1 = new UCS("user1", Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
            UCS ucs2 = UCS.FromXAxisAndPointOnXYplane("user2", Vector3.Zero, new Vector3(1,1,0), new Vector3(1,1,1));
            UCS ucs3 = UCS.FromNormal("user3", Vector3.Zero, new Vector3(1, 1, 1), 0);
            dxf.UCSs.Add(ucs1);
            dxf.UCSs.Add(ucs2);
            dxf.UCSs.Add(ucs3);

            dxf.Save("ucs.dxf");

            dxf = DxfDocument.Load("ucs.dxf");

        }
        private static void ImageUsesAndRemove()
        {
            ImageDefinition imageDef1 = new ImageDefinition("img\\image01.jpg");
            Image image1 = new Image(imageDef1, Vector3.Zero, 10, 10);

            ImageDefinition imageDef2 = new ImageDefinition("img\\image02.jpg");
            Image image2 = new Image(imageDef2, new Vector3(0, 220, 0), 10, 10);
            Image image3 = new Image(imageDef2, image2.Position + new Vector3(280, 0, 0), 10, 10);

            Block block =new Block("MyImageBlock");
            block.Entities.Add(image1);

            Insert insert = new Insert(block);

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(insert);
            dxf.AddEntity(image2);
            dxf.AddEntity(image3);
            dxf.Save("test netDxf.dxf");

           
            dxf.RemoveEntity(insert);
            dxf.Blocks.Remove(insert.Block.Name);
            // imageDef1 has no references in the document
            List<DxfObject> uses = dxf.ImageDefinitions.GetReferences(imageDef1.Name);
            dxf.Save("test netDxf with unreferenced imageDef.dxf");
            dxf = DxfDocument.Load("test netDxf with unreferenced imageDef.dxf");

            // once we have removed the insert and then the block that contained image1 we don't have more references to imageDef1
            dxf.ImageDefinitions.Remove(imageDef1.Name);
            dxf.Save("test netDxf with deleted imageDef.dxf");

        }
        private static void LayerAndLineTypesUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.Layer = layer1;
            dxf.AddEntity(poly);

            Ellipse ellipse = new Ellipse(new Vector3(2, 2, 0), 5, 3);
            ellipse.Rotation = 30;
            ellipse.Layer = layer1;
            dxf.AddEntity(ellipse);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));
            line.Layer = layer2;
            line.LineType = LineType.DashDot;
            dxf.AddEntity(line);


            bool ok;

            // this will return false since layer1 is not empty
            ok = dxf.Layers.Remove(layer1.Name);

            List<DxfObject> entities = dxf.Layers.GetReferences(layer1.Name);
            foreach (DxfObject o in entities)
            {
                dxf.RemoveEntity(o as EntityObject);
            }

            // now this should return true since layer1 is empty
            ok = dxf.Layers.Remove(layer1.Name);

            // blocks needs an special attention
            Layer layer3 = new Layer("Layer3");
            layer3.Color = AciColor.Yellow;

            Circle circle = new Circle(Vector3.Zero, 15);
            // it is always recommended that all block entities will be located in layer 0, but this is up to the user.
            circle.Layer = new Layer("circle");
            circle.Layer.Color = AciColor.Green;

            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            block.Layer = new Layer("blockLayer");
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Layer = new Layer("attDefLayer");
            attdef.LineType = LineType.Center;
            block.AttributeDefinitions.Add(attdef);

            Insert insert = new Insert(block, new Vector2(5, 5));
            insert.Layer = layer3;
            insert.Attributes[0].Layer = new Layer("attLayer");
            insert.Attributes[0].LineType = LineType.Dashed;
            dxf.AddEntity(insert);

            dxf.Save("test.dxf");

            DxfDocument dxf2 = DxfDocument.Load("test.dxf");

            // this list will contain the circle entity
            List<DxfObject> dxfObjects;
            dxfObjects = dxf.Layers.GetReferences("circle");

            // but we cannot removed since it is part of a block
            ok = dxf.RemoveEntity(circle);
            // we need to remove first the block, but to do this we need to make sure there are no references of that block in the document
            dxfObjects = dxf.Blocks.GetReferences(block.Name);
            foreach (DxfObject o in dxfObjects)
            {
                dxf.RemoveEntity(o as EntityObject);
            }


            // now it is safe to remove the block since we do not have more references in the document
            ok = dxf.Blocks.Remove(block.Name);
            // now it is safe to remove the layer "circle", the circle entity was removed with the block since it was part of it
            ok = dxf.Layers.Remove("circle");

            // purge all document layers, only empty layers will be removed
            dxf.Layers.Clear();

            // purge all document line types, only line types without references will be removed
            dxf.LineTypes.Clear();

            dxf.Save("test2.dxf");
        }
        private static void TextAndDimensionStyleUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            // blocks needs an special attention
            Layer layer3 = new Layer("Layer3");
            layer3.Color = AciColor.Yellow;

            Circle circle = new Circle(Vector3.Zero, 15);
            // it is always recommended that all block entities will be located in layer 0, but this is up to the user.
            circle.Layer = new Layer("circle");
            circle.Layer.Color = AciColor.Green;

            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");

            block.AttributeDefinitions.Add(attdef);

            Insert insert = new Insert(block, new Vector2(5, 5));
            insert.Attributes[0].Style = new TextStyle("Arial.ttf");

            dxf.AddEntity(insert);

            dxf.Save("style.dxf");
            DxfDocument dxf2;
            dxf2 = DxfDocument.Load("style.dxf");

            dxf.RemoveEntity(circle);

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            dxf.AddEntity(line);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMTXSTY = new TextStyle("Tahoma.ttf");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            double offset = 7;
            LinearDimension dimX = new LinearDimension(line, offset, 0.0, myStyle);
            dimX.Rotation += 30.0;
            LinearDimension dimY = new LinearDimension(line, offset, 90.0, myStyle);
            dimY.Rotation += 30.0;

            dxf.AddEntity(dimX);
            dxf.AddEntity(dimY);

            dxf.Save("style2.dxf");
            dxf2 = DxfDocument.Load("style2.dxf");


            dxf.RemoveEntity(dimX);
            dxf.RemoveEntity(dimY);

            bool ok;

            // we can remove myStyle it was only referenced by dimX and dimY
            ok = dxf.DimensionStyles.Remove(myStyle.Name);

            // we cannot remove myStyle.TextStyle since it is in use by the internal blocks created by the dimension entities
            ok = dxf.Blocks.Remove(dimX.Block.Name);
            ok = dxf.Blocks.Remove(dimY.Block.Name);

            // no we can remove the unreferenced textStyle
            ok = dxf.TextStyles.Remove(myStyle.DIMTXSTY.Name);

            dxf.Save("style3.dxf");
            dxf2 = DxfDocument.Load("style3.dxf");
        }
        private static void MLineStyleUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();
            //MLineStyle style = MLineStyle.Default;
            //dxf.AddMLineStyle(style);

            List<Vector2> vertexes = new List<Vector2>
                                            {
                                                new Vector2(0, 0),
                                                new Vector2(0, 150),
                                                new Vector2(150, 150),
                                                new Vector2(150, 0)
                                            };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;
            //mline.IsClosed = true;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;
            //style.StartAngle = 25.0;
            //style.EndAngle = 160.0;
            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
            mline.Style = style;
            // we have modified the mline after setting its vertexes so we need to manually call this method.
            // also when manually editting the vertex distances
            mline.Update();

            // we can manually create cuts or gaps in the individual elements that made the multiline.
            // the cuts are defined as distances from the start point of the element along its direction.
            mline.Vertexes[0].Distances[0].Add(50);
            mline.Vertexes[0].Distances[0].Add(100);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count - 1].Add(50);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count - 1].Add(100);
            dxf.AddEntity(mline);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("MLine.dxf");

            DxfDocument dxf2 = DxfDocument.Load("MLine.dxf");

            // "MyStyle" is used only once
            List<DxfObject> uses;
            uses = dxf.MlineStyles.GetReferences(mline.Style.Name);

            // if we try to get the LineTypeUses, we will find out that "MyStyle" appears several times,
            // this is due to that each MLineStyleElement of a MLineStyle has an associated LineType
            uses = dxf.LineTypes.GetReferences(LineType.ByLayerName);

            bool ok;
            ok = dxf.RemoveEntity(mline);

            // "MyStyle" is not used its reference has been deleted
            uses = dxf.MlineStyles.GetReferences(mline.Style.Name);
            // we can safely remove it
            dxf.MlineStyles.Remove(mline.Style.Name);

            dxf.Save("MLine2.dxf");

            dxf.Layers.Clear();

            dxf.Save("MLine2.dxf");
        }
        private static void AppRegUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            List<PolylineVertex> vertexes = new List<PolylineVertex>{
                                                                        new PolylineVertex(0, 0, 0), 
                                                                        new PolylineVertex(10, 0, 10), 
                                                                        new PolylineVertex(10, 10, 20), 
                                                                        new PolylineVertex(0, 10, 30)
                                                                        };

            Polyline poly = new Polyline(vertexes, true);

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));

            poly.XData.Add(xdata1);

            dxf.AddEntity(poly);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));

            ApplicationRegistry myAppReg = new ApplicationRegistry("MyAppReg");
            XData xdata2 = new XData(myAppReg);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Distance, Vector3.Distance(line.StartPoint, line.EndPoint)));
            line.XData.Add(xdata2);

            dxf.AddEntity(line);

            Circle circle = new Circle(Vector3.Zero, 15);
            XData xdata3 = new XData(myAppReg);
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Real, circle.Radius));
            circle.XData.Add(xdata3);

            dxf.AddEntity(circle);

            dxf.Save("appreg.dxf");

            DxfDocument dxf2 = DxfDocument.Load("appreg.dxf");

            // will return false the "MyAppReg" is in use
            bool ok;
            ok = dxf.ApplicationRegistries.Remove(myAppReg.Name);
            dxf.RemoveEntity(line);
            dxf.RemoveEntity(circle);
            // "MyAppReg" is not used anymore
            IList<DxfObject> uses = dxf.ApplicationRegistries.GetReferences(myAppReg.Name);
            // it is safe to delete it
            ok = dxf.ApplicationRegistries.Remove(myAppReg.Name);
            
            // we can even make a full cleanup
            dxf.ApplicationRegistries.Clear();

            dxf.Save("appreg2.dxf");


        }
        private static void ExplodePolyfaceMesh()
        {
            DxfDocument dxf = DxfDocument.Load("polyface mesh.dxf");
            DxfDocument dxfOut = new DxfDocument(dxf.DrawingVariables);
            foreach (PolyfaceMesh polyfaceMesh in dxf.PolyfaceMeshes)
            {
                List<EntityObject> entities = polyfaceMesh.Explode();
                dxfOut.AddEntity(entities);
            }

            dxfOut.Save("polyface mesh exploded.dxf");
        }
        private static void ApplicationRegistries()
        {
            DxfDocument dxf = new DxfDocument();
            // add a new application registry to the document (optional), if not present it will be added when the entity is passed to the document
            ApplicationRegistry newAppReg = dxf.ApplicationRegistries.Add(new ApplicationRegistry("NewAppReg"));

            Line line = new Line(Vector2.Zero, 100 * Vector2.UnitX);
            XData xdata = new XData(newAppReg);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "string of the new application registry"));
            line.XData.Add(xdata);

            dxf.AddEntity(line);
            dxf.Save("ApplicationRegistryTest.dxf");

            // gets the complete application registries present in the document
            ICollection<ApplicationRegistry> appRegs = dxf.ApplicationRegistries.Items;

            // get an application registry by name
            //ApplicationRegistry netDxfAppReg = dxf.ApplicationRegistries[appRegs[dxf.ApplicationRegistries.Count - 1].Name];
        }
        private static void TestOCStoWCS()
        {
            // vertexes of the light weight polyline, they are defined in OCS (Object Coordinate System)
            LwPolylineVertex v1 = new LwPolylineVertex(1, -5);
            LwPolylineVertex v2 = new LwPolylineVertex(-3, 2);
            LwPolylineVertex v3 = new LwPolylineVertex(8, 15);

            LwPolyline lwp = new LwPolyline(new List<LwPolylineVertex> {v1, v2, v3});
            // the normal will define the plane where the lwpolyline is defined
            lwp.Normal = new Vector3(1, 1, 0);
            // the entity elevation defines the z vector of the vertexes along the entity normal
            lwp.Elevation = 2.5;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(lwp);
            dxf.Save("OCStoWCS.dxf");

            // if you want to convert the vertexes of the polyline to WCS (World Coordinate System), you can
            Vector3 v1OCS = new Vector3(v1.Position.X, v1.Position.Y, lwp.Elevation);
            Vector3 v2OCS = new Vector3(v2.Position.X, v2.Position.Y, lwp.Elevation);
            Vector3 v3OCS = new Vector3(v3.Position.X, v3.Position.Y, lwp.Elevation);
            IList<Vector3> vertexesWCS = MathHelper.Transform(new List<Vector3> { v1OCS, v2OCS, v3OCS }, lwp.Normal, CoordinateSystem.Object, CoordinateSystem.World);


        }
        private static void WriteGradientPattern()
        {
            List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>
                                            {
                                                new LwPolylineVertex(new Vector2(0, 0)),
                                                new LwPolylineVertex(new Vector2(0, 150)),
                                                new LwPolylineVertex(new Vector2(150, 150)),
                                                new LwPolylineVertex(new Vector2(150, 0))
                                            };
            LwPolyline pol = new LwPolyline(vertexes, true);


            Line line1 = new Line(new Vector2(0, 0), new Vector2(0, 150));
            Line line2 = new Line(new Vector2(0, 150), new Vector2(150, 150));
            Line line3 = new Line(new Vector2(150, 150), new Vector2(150, 0));
            Line line4 = new Line(new Vector2(150, 0), new Vector2(0, 0));

            AciColor color = new AciColor(63, 79, 127);
            HatchGradientPattern gradient = new HatchGradientPattern(color, AciColor.Blue, HatchGradientPatternType.Linear);
            //HatchGradientPattern gradient = new HatchGradientPattern(AciColor.Red, 0.75, HatchGradientPatternType.Linear);
            gradient.Angle = 30;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                    {
                                                        new HatchBoundaryPath(new List<EntityObject> {pol})
                                                    };
            Hatch hatch = new Hatch(gradient, boundary, true);
            
            // gradients are only supported for AutoCad2004 and later
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2004);
            dxf.AddEntity(hatch);
            dxf.Save("gradient test.dxf");

            //DxfDocument dxf2 = DxfDocument.Load("gradient test.dxf");

            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;

            //dxf.Save("gradient test 2000.dxf");

        }
        private static void WriteGroup()
        {
            Line line1 = new Line(new Vector2(0, 0), new Vector2(100, 100));
            Line line2 = new Line(new Vector2(100, 0), new Vector2(200, 100));
            Line line3 = new Line(new Vector2(200, 0), new Vector2(300, 100));

            // named group
            Group group1 = new Group("MyGroup", new EntityObject[] {line1, line2});

            //unnamed group
            Group group2 = new Group(new EntityObject[] {line1, line3});

            DxfDocument dxf = new DxfDocument();
            // the AddGroup method will also add the entities contained in a group to the document.
            dxf.Groups.Add(group1);
            dxf.Groups.Add(group2);

            List<DxfObject> list = dxf.Groups.GetReferences(group1);
            dxf.Save("group.dxf");

            dxf = DxfDocument.Load("group.dxf");

            group1 = dxf.Groups[group1.Name];
            group2 = dxf.Groups[group2.Name];
            dxf.Groups.Remove(group1);
            dxf.Groups.Remove(group2);
            dxf.Save("group copy.dxf");


        }
        private static void WriteMLine()
        {
            DxfDocument dxf = new DxfDocument();
            //MLineStyle style = MLineStyle.Default;
            //dxf.AddMLineStyle(style);

            List<Vector2> vertexes = new List<Vector2>
                                            {
                                                new Vector2(0, 0),
                                                new Vector2(0, 150),
                                                new Vector2(150, 150),
                                                new Vector2(150, 0)
                                            };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;
            //mline.IsClosed = true;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;
            //style.StartAngle = 25.0;
            //style.EndAngle = 160.0;
            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
            mline.Style = style;
            // we have modified the multiline after setting its vertexes so we need to manually call this method.
            // also when manually editing the vertex distances
            mline.Update();

            // we can manually create cuts or gaps in the individual elements that made the multiline.
            // the cuts are defined as distances from the start point of the element along its direction.
            mline.Vertexes[0].Distances[0].Add(50);
            mline.Vertexes[0].Distances[0].Add(100);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(50);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(100);

            dxf.AddEntity(mline);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("MLine.dxf");

            //dxf = DxfDocument.Load("Drawing1.dxf");
            //dxf.Save("Drawing1 copy.dxf");

            //dxf = DxfDocument.Load("Drawing3.dxf");
            //dxf.Save("Drawing3 copy.dxf");

            //dxf = DxfDocument.Load("Drawing2.dxf");
            //dxf.Save("Drawing2 copy.dxf");

            // empty mline
            //List<Vector2> vertexes2 = new List<Vector2>
            //                             {
            //                                 new Vector2(0, 0),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(200, 0)
            //                             };

            //MLine mline2 = new MLine(vertexes2){Scale = 20};
            //mline2.CalculateVertexesInfo();

            //DxfDocument dxf2 = new DxfDocument();
            //dxf2.AddEntity(mline2);
            ////dxf2.Save("void mline.dxf");

            //MLine mline3 = new MLine();
            //dxf2.AddEntity(mline3);
            ////dxf2.Save("void mline.dxf");

            //Polyline pol = new Polyline();
            //LwPolyline lwPol = new LwPolyline();
            //dxf2.AddEntity(pol);
            //dxf2.AddEntity(lwPol);
            //dxf2.Save("void mline.dxf");
            //dxf2 = DxfDocument.Load("void mline.dxf");
            
        }
        private static void ObjectVisibility()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0))
                            {
                                Color = AciColor.Yellow
                            };

            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0))
                                {
                                    IsVisible = false
                                };

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("object visibility.dxf");
            dxf = DxfDocument.Load("object visibility.dxf");
            dxf.Save("object visibility 2.dxf");

        }
        private static void WriteInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();

            Block nestedBlock = new Block("Nested block");
            nestedBlock.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            nestedBlock.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));

            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            block.Entities.Add(nestedInsert);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);

            dxf.Save("insert.dxf");
            dxf = DxfDocument.Load("insert.dxf");

        }
        private static void EntityTrueColor()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Color = new AciColor(152, 103, 136);
            // by default a color initialized with rgb components will be exported as true color
            // you can override this behavior with
            // line.Color.UseTrueColor = false;

            Layer layer = new Layer("MyLayer");
            layer.Color = new AciColor(157, 238, 17);
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("line true color.dxf");
            dxf = DxfDocument.Load("line true color.dxf");
        }
        private static void EntityLineWeight()
        {
            // the lineweight is always defined as 1/100 mm, this property is the equivalent of stroke width, outline width in other programs. Do not confuse with line.Thickness
            // it follow the AutoCAD naming style, check the documentation in case of doubt
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Lineweight.Value = 100; // 1.0 mm
            Text text = new Text("Text with lineweight", Vector3.Zero, 10);
            text.Lineweight.Value = 50; // 0.5 mm

            Layer layer = new Layer("MyLayer");
            layer.Lineweight.Value = 200; // 2 mm all entities in the layer with Color.ByLayer will inherit this value
            layer.Color = AciColor.Green;
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.AddEntity(text);
            dxf.Save("line weight.dxf");
            dxf = DxfDocument.Load("line weight.dxf");
        }
        private static void ReadWriteFromStream()
        {
            // Load and Save methods are now able to work directly with a stream.
            // They will return true or false if the operation has been carried out successfully or not.
            // The Save(string file, DxfVersion dxfVersion) and Load(string file) methods will still raise an exception if they are unable to create the FileStream.
            // On Debug mode they will raise any exception that migh occurr during the whole process.
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.Save("test.dxf");
            
            // saving to memory stream always use the default constructor, a fixed size stream will not work.
            MemoryStream memoryStream = new MemoryStream();
            if(!dxf.Save(memoryStream))
                throw new Exception("Error saving to memory stream.");
            
            // loading from memory stream
            DxfDocument dxf2 = DxfDocument.Load(memoryStream);
            memoryStream.Close(); // once the stream is not need anymore we need to close the stream

            // saving to file stream
            FileStream fileStream = new FileStream("test fileStream.dxf", FileMode.Create);
            if (!dxf2.Save(fileStream, true))
                throw new Exception("Error saving to file stream.");

            fileStream.Close(); // you will need to close the stream manually to avoid file already open conflicts

            FileStream fileStreamLoad = new FileStream("test fileStream.dxf", FileMode.Open, FileAccess.Read);
            DxfDocument dxf3 = DxfDocument.Load(fileStreamLoad);
            fileStreamLoad.Close();

            DxfDocument dxf4 = DxfDocument.Load("test fileStream.dxf");

        }
        private static void Text()
        {
            // use a font that has support for Chinese characters
            TextStyle textStyle = new TextStyle("Chinese text", "simsun.ttf");

            // for dxf database version 2007 and later you can use directly the characters,
            DxfDocument dxf1 = new DxfDocument(DxfVersion.AutoCad2010);
            Text text1 = new Text("这是中国文字", Vector2.Zero, 10, textStyle);
            MText mtext1 = new MText("这是中国文字", new Vector2(0, 30), 10, 0, textStyle);
            dxf1.AddEntity(text1);
            dxf1.AddEntity(mtext1);
            dxf1.Save("textCad2010.dxf");

            foreach (Text text in dxf1.Texts)
            {
                Console.WriteLine(text.Value);
            }
            foreach (MText text in dxf1.MTexts)
            {
                Console.WriteLine(text.Value);
            }

            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();

            DxfDocument loadDxf = DxfDocument.Load("textCad2010.dxf");

            // for previous version (this method will also work for later ones) you will need to supply the unicode value (U+value),
            // you can get this value with the Windows Character Map application
            DxfDocument dxf2 = new DxfDocument(DxfVersion.AutoCad2010);
            Text text2 = new Text("\\U+8FD9\\U+662F\\U+4E2D\\U+56FD\\U+6587\\U+5B57", Vector2.Zero, 10, textStyle);
            MText mtext2 = new MText("\\U+8FD9\\U+662F\\U+4E2D\\U+56FD\\U+6587\\U+5B57", new Vector2(0, 30), 10, 0, textStyle);
            dxf2.AddEntity(text2);
            dxf2.AddEntity(mtext2);
            dxf2.Save("textCad2000.dxf");
        }
        private static void WriteNoAsciiText()
        {
            TextStyle textStyle = new TextStyle("Arial.ttf");
            DxfDocument dxf = new DxfDocument();
            dxf.DrawingVariables.LastSavedBy = "ЉЊЋЌЍжзицрлЯ";
            //Text text = new Text("ÁÉÍÓÚ áéíóú Ññ àèìòù âêîôû", Vector2.Zero,10);
            Text text = new Text("ЉЊЋЌЍжзицрлЯ", Vector2.Zero, 10, textStyle);
            MText mtext = new MText("ЉЊЋЌЍжзицрлЯ", new Vector2(0, 50), 10, 0, textStyle);

            dxf.AddEntity(text);
            dxf.AddEntity(mtext);
            foreach (Text t in dxf.Texts)
            {
                Console.WriteLine(t.Value);
            }
            foreach (MText t in dxf.MTexts)
            {
                Console.WriteLine(t.Value);
            }
            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
            dxf.Save("text1.dxf");

            dxf = DxfDocument.Load("text1.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("text2.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("text3.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("text4.dxf");

        }
        private static void WriteSplineBoundaryHatch()
        {

            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0)),
                                                    new SplineVertex(new Vector3(25, 50, 0)),
                                                    new SplineVertex(new Vector3(50, 0, 0)),
                                                    new SplineVertex(new Vector3(75, 50, 0)),
                                                    new SplineVertex(new Vector3(100, 0, 0))
                                                };

            // hatch with single closed spline boundary path
            Spline spline = new Spline(ctrlPoints, 3, true); // closed periodic

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>();

            HatchBoundaryPath path = new HatchBoundaryPath(new List<EntityObject> {spline});
            boundary.Add(path);
            Hatch hatch = new Hatch(HatchPattern.Line, boundary, true);
            hatch.Pattern.Angle = 45;
            hatch.Pattern.Scale = 10;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(hatch);
            dxf.AddEntity(spline);
            dxf.Save("hatch closed spline.dxf");
            dxf = DxfDocument.Load("hatch closed spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("hatch closed spline 2010.dxf");

            // hatch boundary path with spline and line
            Spline openSpline = new Spline(ctrlPoints, 3);
            Line line = new Line(ctrlPoints[0].Position, ctrlPoints[ctrlPoints.Count - 1].Position);

            List<HatchBoundaryPath> boundary2 = new List<HatchBoundaryPath>();
            HatchBoundaryPath path2 = new HatchBoundaryPath(new List<EntityObject> { openSpline, line });
            boundary2.Add(path2);
            Hatch hatch2 = new Hatch(HatchPattern.Line, boundary2, true);
            hatch2.Pattern.Angle = 45;
            hatch2.Pattern.Scale = 10;

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(hatch2);
            dxf2.AddEntity(openSpline);
            dxf2.AddEntity(line);
            dxf2.Save("hatch open spline.dxf");
            dxf2 = DxfDocument.Load("hatch open spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf2.Save("hatch open spline 2010.dxf");

        }
        private static void WriteNoInsertBlock()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            // create the block definition
            Block block = new Block("MyBlock");
            // add the entities that you want in the block
            block.Entities.Add(line);
            block.Entities.Add(line2);
            
            
            // create the document
            DxfDocument dxf = new DxfDocument();
            // add the block definition to the block table list (this is the function that was private in earlier versions, check the changelog.txt)
            dxf.Blocks.Add(block);

            // and save file, no visible entities will appear if you try to open the drawing but the block will be there
            dxf.Save("Block definiton.dxf");

        }
        private static void WriteImage()
        {
            ImageDefinition imageDefinition = new ImageDefinition("img\\image01.jpg");
            Image image = new Image(imageDefinition, Vector3.Zero, 10, 10);

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "xData image position"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, image.Position.X));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, image.Position.Y));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, image.Position.Z));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);
            image.XData.Add(xdata1);

            //image.Normal = new Vector3(1, 1, 1);
            //image.Rotation = 30;

            // you can pass a name that must be unique for the image definiton, by default it will use the file name without the extension
            ImageDefinition imageDef2 = new ImageDefinition("img\\image02.jpg", "MyImage");
            Image image2 = new Image(imageDef2, new Vector3(0, 150, 0), 10, 10);
            Image image3 = new Image(imageDef2, new Vector3(150, 150, 0), 10, 10);

            // clipping boundary definition in local coordinates
            ClippingBoundary clip = new ClippingBoundary(100, 100, 500, 300);
            image.ClippingBoundary = clip;
            // set to null to restore the default clipping boundary (full image)
            image.ClippingBoundary = null;

            // images can be part of a block definition
            Block block = new Block("ImageBlock");
            block.Entities.Add(image2);
            block.Entities.Add(image3);
            Insert insert = new Insert(block, new Vector3(0, 100, 0));

            DxfDocument dxf = new DxfDocument();

            dxf.AddEntity(image);
            //dxf.AddEntity(image2);
            //dxf.AddEntity(image3);
            dxf.AddEntity(insert);

            dxf.Save("image.dxf");
            dxf = DxfDocument.Load("image.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("test.dxf");

            //dxf.RemoveEntity(image2);
            //dxf.Save("image2.dxf");
            //dxf.RemoveEntity(image3);
            //dxf.RemoveEntity(image);
            //dxf.Save("image3.dxf");

        }
        private static void AddAndRemove()
        {
            Layer layer1 = new Layer("layer1") { Color = AciColor.Blue };
            Layer layer2 = new Layer("layer2") { Color = AciColor.Green };

            Line line = new Line(new Vector2(0, 0), new Vector2(10, 10));
            line.Layer = layer1;
            Circle circle = new Circle(new Vector2(0, 0), 10);
            circle.Layer = layer2;

            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyDimStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            //text
            TextStyle style = new TextStyle("MyTextStyle", "Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f, style)
                            {
                                Layer = new Layer("text")
                                            {
                                                Color = {Index = 8}
                                            }
                            };
            text.Alignment = TextAlignment.TopRight;

            HeaderVariables variables = new HeaderVariables
                                            {
                                                AcadVer = DxfVersion.AutoCad2004
                                            };
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(new EntityObject[] {line, circle, dim1, text});
            dxf.Save("before remove.dxf");

            dxf.RemoveEntity(circle);
            dxf.Save("after remove.dxf");

            dxf.AddEntity(circle);
            dxf.Save("after remove and add.dxf");

            dxf.RemoveEntity(dim1);
            dxf.Save("remove dim.dxf");

            dxf.AddEntity(dim1);
            dxf.Save("add dim.dxf");

            DxfDocument dxf2 = DxfDocument.Load("dim block names.dxf");
            dxf2.AddEntity(dim1);
            dxf2.Save("dim block names2.dxf");
        }
        private static void LoadAndSave()
        {
            DxfDocument dxf = DxfDocument.Load("block sample.dxf");
            dxf.Save("block sample1.dxf");

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(dxf.Inserts[0]);
            dxf2.Save("block sample2.dxf");

            dxf.Save("clean2.dxf");
            dxf = DxfDocument.Load("clean.dxf");
            dxf.Save("clean1.dxf");

            // open a dxf saved with autocad
            dxf = DxfDocument.Load("sample.dxf");
            dxf.Save("sample4.dxf");

            Line cadLine = dxf.Lines[0];
            Layer layer = new Layer("netLayer");
            layer.Color = AciColor.Yellow;

            Line line = new Line(new Vector2(20, 40), new Vector2(100, 200));
            line.Layer = layer;
            // add a new entity to the document
            dxf.AddEntity(line);

            dxf.Save("sample2.dxf");

            DxfDocument dxf3 = new DxfDocument();
            dxf3.AddEntity(cadLine);
            dxf3.AddEntity(line);
            dxf3.Save("sample3.dxf");
        }
        private static void CleanDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            dxf.Save("clean drawing.dxf");
        }
        private static void OrdinateDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 origin = new Vector3(2, 1, 0);
            Vector2 refX = new Vector2(1, 0);
            Vector2 refY = new Vector2(0, 2);
            double length = 3;
            double angle = 30;
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;

            OrdinateDimension dimX1 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, 0, myStyle);
            OrdinateDimension dimX2 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, angle, myStyle);
            OrdinateDimension dimY1 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, 0, myStyle);
            OrdinateDimension dimY2 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, angle, myStyle);

            dxf.AddEntity(dimX1);
            dxf.AddEntity(dimY1);
            dxf.AddEntity(dimX2);
            dxf.AddEntity(dimY2);

            Line lineX = new Line(origin, origin+5 * Vector3.UnitX);
            Line lineY = new Line(origin, origin+5 * Vector3.UnitY);

            Vector2 point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad);
            Line lineXRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad + MathHelper.HalfPI);
            Line lineYRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            dxf.AddEntity(lineX);
            dxf.AddEntity(lineY);
            dxf.AddEntity(lineXRotate);
            dxf.AddEntity(lineYRotate);

            dxf.Save("ordinate dimension.dxf");

            dxf = DxfDocument.Load("ordinate dimension.dxf");
        }
        private static void Angular2LineDimensionDrawing()
        {
            double offset = 7.5;
            
            Line line1 = new Line(new Vector2(1, 2), new Vector2(6, 0));
            Line line2 = new Line(new Vector2(2, 1), new Vector2(4,5));

            Angular2LineDimension dim = new Angular2LineDimension(line1, line2, offset);
            
            DxfDocument dxf = new DxfDocument();
            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim);

            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim);
            Insert insert = new Insert(block);

            dxf.AddEntity(insert);

            dxf.Save("angular 2 line dimension.dxf");
            dxf = DxfDocument.Load("angular 2 line dimension.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("angular 2 line dimension.dxf");
        }
        private static void Angular3PointDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Arc arc = new Arc(center, radius, -30, 60);
            //arc.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");

            Angular3PointDimension dim = new Angular3PointDimension(arc, 5, myStyle);
            dxf.AddEntity(arc);
            dxf.AddEntity(dim);
            dxf.Save("angular 3 point dimension.dxf");

            dxf = DxfDocument.Load("angular 3 point dimension.dxf");
        }
        private static void DiametricDimensionDrawing()
        {
            //DxfDocument dxf = new DxfDocument();

            //Vector3 center = new Vector3(1, 2, 0);
            //double radius = 2.42548;
            //Circle circle = new Circle(center, radius);
            ////circle.Normal = new Vector3(1, 1, 1);
            //DimensionStyle myStyle = new DimensionStyle("MyStyle");
            //myStyle.DIMPOST = "<>mm";
            //myStyle.DIMDEC = 2;
            //myStyle.DIMDSEP = ',';

            //DiametricDimension dim = new DiametricDimension(circle, 30, myStyle);
            //dxf.AddEntity(circle);
            //dxf.AddEntity(dim);
            //dxf.Save("diametric dimension.dxf");

            //dxf.RemoveEntity(dim);
            //dxf.Save("diametric dimension removed.dxf");

            //dxf = DxfDocument.Load("diametric dimension.dxf");
            //// remove entitiy with a handle
            //Dimension dimLoaded = (Dimension) dxf.GetObjectByHandle(dim.Handle);
            //dxf.RemoveEntity(dimLoaded);
            //dxf.Save("diametric dimension removed 2.dxf");

        }
        private static void RadialDimensionDrawing()
        {
            //DxfDocument dxf = new DxfDocument();

            //Vector3 center = new Vector3(1, 2, 0);
            //double radius = 2.42548;
            //Circle circle = new Circle(center, radius);
            //circle.Normal = new Vector3(1, 1, 1);
            //DimensionStyle myStyle = new DimensionStyle("MyStyle");
            //myStyle.DIMPOST = "<>mm";
            //myStyle.DIMDEC = 2;
            //myStyle.DIMDSEP = ',';
            
            //RadialDimension dim = new RadialDimension(circle, 30, myStyle);
            //dxf.AddEntity(circle);
            //dxf.AddEntity(dim);
            //dxf.Save("radial dimension.dxf");

            //dxf = DxfDocument.Load("radial dimension.dxf");
        }
        private static void LinearDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            
            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            dxf.AddEntity(line);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            double offset = 7;
            LinearDimension dimX = new LinearDimension(line, offset,0.0, myStyle);
            dimX.Rotation += 30.0;
            LinearDimension dimY = new LinearDimension(line, offset, 90.0, myStyle);
            dimY.Rotation += 30.0;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Linear Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dimX.XData.Add(xdata);
            dimY.XData.Add(xdata);
            dxf.AddEntity(dimX);
            dxf.AddEntity(dimY);
            dxf.Save("linear dimension.dxf");
            // dxf = DxfDocument.Load("linear dimension.dxf");
        }
        private static void AlignedDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            Vector3 p3 = p1 + new Vector3(4, 0, 0);
            Vector3 p4 = p2 + new Vector3(4, 0, 0);
            Line line2 = new Line(p3, p4);
            AlignedDimension dim2 = new AlignedDimension(p3, p4, -offset, myStyle);


            Vector2 perp = Vector2.Perpendicular(new Vector2(p2.X, p2.Y) - new Vector2(p1.X, p1.Y));
            //dim.Normal = -new Vector3(perp.X, perp.Y, 0.0) ;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Aligned Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dim1.XData.Add(xdata);

            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim1);
            //dxf.AddEntity(dim2);



            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim1);
            block.Entities.Add(dim2);
            Insert insert = new Insert(block);
            dxf.AddEntity(insert);

            dxf.Save("aligned dimension.dxf");

            dxf = DxfDocument.Load("aligned dimension.dxf");

        }
        private static void WriteMText()
        {
            DxfDocument dxf = new DxfDocument();

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            //text
            TextStyle style = new TextStyle("Times.ttf");
            //TextStyle style = TextStyle.Default;
            MText mText = new MText(new Vector3(3,2,0), 1.0f, 100.0f, style);
            mText.Layer = new Layer("Multiline Text");
            //mText.Layer.Color.Index = 8;
            mText.Rotation = 0;
            mText.LineSpacingFactor = 1.0;
            mText.ParagraphHeightFactor = 1.0;

            //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;
            //mText.Write("Hello World!");
            //mText.Write(" we keep writting on the same line.");
            //mText.WriteLine("This text is in a new line");

            //mText.Write("Hello World! ");
            //for (int i = 0; i < 50; i++)
            //{
            //    mText.Write("1234567890");
            //}
            //mText.Write(" This text is over the limit of the 250 character chunk");
            //mText.NewParagraph();
            //mText.Write("This is a text in a new paragraph");
            //mText.Write(" and we continue writing in the previous paragraph");
            //mText.NewParagraph();
            MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
            options.Bold = true;
            mText.Write("Bold text in mText.Style", options);
            mText.EndParagraph();
            options.Italic = true;
            mText.Write("Bold and italic text in mText.Style", options);
            mText.EndParagraph();
            options.Bold = false;
            options.FontName = "Arial";
            options.Color = AciColor.Blue;
            mText.ParagraphHeightFactor = 2;
            mText.Write("Italic text in Arial", options);
            mText.EndParagraph();
            options.Italic = false;
            options.Color = null; // back to the default text color
            mText.Write("Normal text in Arial with the default paragraph height factor", options);
            mText.EndParagraph();
            mText.ParagraphHeightFactor = 1;
            mText.Write("No formatted text uses mText.Style");
            mText.Write(" and the text continues in the same paragraph.");
            mText.EndParagraph();

            //options.HeightPercentage = 2.5;
            //options.Color = AciColor.Red;
            //options.Overstrike = true;
            //options.Underline = true;
            //options.FontFile = "times.ttf";
            //options.ObliqueAngle = 15;
            //options.CharacterSpacePercentage = 2.35;
            //options.WidthFactor = 1.8;
            
            //for unknown reasons the aligment doesn't seem to change anything
            //mText.Write("Formatted text", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Center;
            //mText.Write("Center", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Top;
            //mText.Write("Top", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Bottom;
            //mText.Write("Bottom", options);

            mText.XData.Add(xdata);
            
            dxf.AddEntity(mText);

            dxf.Save("MText sample.dxf");

        }
        private static void HatchCircleBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            // create a circle that will be our hatch boundary in this case it is a circle with center (5.5, -5.5, 0.0) and a radius 10.0
            Circle circle = new Circle(new Vector3(5.5, -5.5, 0), 10);

            // create the hatch boundary path with only the circle (a circle is already a closed loop it is all we need to define a valid boundary path)
            // a hatch can have many boundaries (closed loops) and every boundary path can be made of several entities (lines, polylines, arcs, circles and ellipses)
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                        {
                                                            new HatchBoundaryPath(new List<EntityObject>{circle})
                                                        };  

            // create the hatch in this case we will use the predefined Solid hatch pattern and our circle as the boundary path
            //Hatch hatch = new Hatch(HatchPattern.Solid, boundary);
            Hatch hatch = new Hatch(HatchPattern.Line, boundary, true);
            // to give a color to the hatch, we have to options:

            // create a new layer with a color for the hatch (in this case by default the hatch will have a ByLayer color)
            //Layer hatchLayer = new Layer("HathLayer") {Color = AciColor.Green};
            //hatch.Layer = hatchLayer;

            // or give the hatch a color just for it
            // old AutoCAD versions only had 255 colors (indexed color), now in AutoCAD you can use true colors (8 bits per channel) but at the moment this is not supported.
            // if you try to give r, g, b values to define a color it will be converted to an indexed color
            // (I haven't tested this code a lot, so errors might appear and the result might not be what you expected).
            hatch.Color = AciColor.Red;

            // the hatch by itself will not show the boundary, but we can use the same entity to show the limits of the hatch, adding it to the document 
            dxf.AddEntity(circle);

            // add the hatch to the document
            dxf.AddEntity(hatch);

            dxf.Save("circle solid fill.dxf");
        }
        private static void LineWidth()
        {
            // the line thickness works as expected, according to the AutoCAD way of doing things
            Line thickLine = new Line(new Vector3(0,10,0),  new Vector3(10,20,0));

            // when you assign a thickness to a line, the result is like a wall, it is like a 3d face whose vertexes are defined by the
            // start and end points of the line and the thickness along the normal of the line.
            thickLine.Thickness = 5;

            // maybe what you are trying to do is create a line with a width (something that we can read it as a line with thickness), the only way to do this is to create a polyline
            // the kind of result you will get if you give a width to a 2d polyline 
            // you can only give a width to a vertex of a Polyline or a LightweigthPolyline
            // I am planning to drop support to AutoCAD 12 dxf files, so to define a bidimensional polyline the only way will be to use lightweight polyline
            // (the Polyline class and the LightWeightPolyline are basically the same).
            LwPolyline widthLine = new LwPolyline();
            LwPolylineVertex startVertex = new LwPolylineVertex(new Vector2(0, 0));
            LwPolylineVertex endVertex = new LwPolylineVertex(new Vector2(10, 10));
            widthLine.Vertexes.AddRange(new[] { startVertex, endVertex });

            // the easy way to give a constant width to a polyline, but you can also give a polyline width by vertex
            // there is a mistake on my part, following the AutoCAD documentation I should have called the PolylineVertex.StartThickness and PolylineVertex.EndThickness as
            // PolylineVertex.StartWidth and PolylineVertex.EndWidth
            // SetConstantWidth is a sort cut that will assign the given value to every start width and end width of every vertex of the polyline
            widthLine.SetConstantWidth(0.5);

            DxfDocument dxf = new DxfDocument();

            // add the entities to the document (both of them to see the difference)
            dxf.AddEntity(thickLine);
            dxf.AddEntity(widthLine);

            dxf.Save("line width.dxf");

        }
        private static void ToPolyline()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 8, -7);
            Vector3 normal = new Vector3(1, 1, 1);

            Circle circle = new Circle(center, 7.5);
            circle.Normal = normal;

            Arc arc = new Arc(center, 5, -45, 45);
            arc.Normal = normal;

            Ellipse ellipse = new Ellipse(center, 15, 7.5);
            ellipse.Rotation = 35;
            ellipse.Normal = normal;

            Ellipse ellipseArc = new Ellipse(center, 10, 5);
            ellipseArc.StartAngle = 315;
            ellipseArc.EndAngle = 45;
            ellipseArc.Rotation = 35;
            ellipseArc.Normal = normal;

            dxf.AddEntity(circle);
            dxf.AddEntity(circle.ToPolyline(10));

            dxf.AddEntity(arc);
            dxf.AddEntity(arc.ToPolyline(10));

            dxf.AddEntity(ellipse);
            dxf.AddEntity(ellipse.ToPolyline(10));

            dxf.AddEntity(ellipseArc);
            dxf.AddEntity(ellipseArc.ToPolyline(10));

            dxf.Save("to polyline.dxf");

            dxf = DxfDocument.Load("to polyline.dxf");

            dxf.Save("to polyline2.dxf");
        }
        private static void CustomHatchPattern()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                            };

            HatchPattern pattern = new HatchPattern("MyPattern", "A custom hatch pattern");

            HatchPatternLineDefinition line1 = new HatchPatternLineDefinition();
            line1.Angle = 45;
            line1.Origin = Vector2.Zero;
            line1.Delta=new Vector2(4,4);
            line1.DashPattern.Add(12);
            line1.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line1);

            HatchPatternLineDefinition line2 = new HatchPatternLineDefinition();
            line2.Angle = 135;
            line2.Origin = new Vector2(2.828427125, 2.828427125);
            line2.Delta = new Vector2(4,-4);
            line2.DashPattern.Add(12);
            line2.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line2);

            Hatch hatch = new Hatch(pattern, boundary, true);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Continuous
            };
            hatch.Pattern.Angle = 0;
            hatch.Pattern.Scale = 1;
            dxf.AddEntity(poly);
            dxf.AddEntity(poly2);
            dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest.dxf");
        }
        private static void FilesTest()
        {
            LineType lineType = LineType.FromFile("acad.lin", "ACAD_ISO15W100");
            HatchPattern hatch = HatchPattern.FromFile("acad.pat", "zigzag");

        }
        private static void LoadSaveHatchTest()
        {
            DxfDocument dxf = DxfDocument.Load("Hatch2.dxf");
            dxf.Save("HatchTest.dxf");
        }
        private static void ExplodeTest()
        {
            DxfDocument dxf = new DxfDocument();
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -23.5));
            polyVertex.Bulge = 1.33;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(34.8, -42.7));
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(65.3, 54.7));
            polyVertex.Bulge = -0.47;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-48.2, 42.5));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;

            dxf.AddEntity(polyline2d);
            dxf.AddEntity(polyline2d.Explode());

            dxf.Save("explode.dxf");
        }
        private static void HatchTestLinesBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            Line line1 = new Line(new Vector3(-10,-10,0),new Vector3(10,-10,0));
            Line line2 = new Line(new Vector3(10, -10, 0), new Vector3(10, 10, 0));
            Line line3 = new Line(new Vector3(10, 10, 0), new Vector3(-10, 10, 0));
            Line line4 = new Line(new Vector3(-10, 10, 0), new Vector3(-10, -10, 0));


            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{line1, line2, line3, line4})
                                                                            };
            Hatch hatch = new Hatch(HatchPattern.Line, boundary, true);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Dashed
            };
            hatch.Elevation = 52;
            hatch.Pattern.Angle = 45;
            hatch.Pattern.Scale = 10;
            hatch.Normal = new Vector3(1, 1, 1);

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf hatch"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Distance, hatch.Pattern.Scale));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, hatch.Pattern.Angle));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            hatch.XData.Add(xdata);

            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(line3);
            //dxf.AddEntity(line4);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateBoundary(true));

            dxf.Save("hatchTest.dxf");
        }
        private static void HatchTest1()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            Line line = new Line(new Vector2(-5, -5), new Vector2(5, -5));
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{line, poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                            };
            Hatch hatch = new Hatch(HatchPattern.Net, boundary, true);
            hatch.Layer = new Layer("hatch")
                                {
                                    Color = AciColor.Red,
                                    LineType = LineType.Continuous
                                };
            hatch.Pattern.Angle = 30;
            hatch.Elevation = 52;
            hatch.Normal = new Vector3(1,1,0);
            hatch.Pattern.Scale = 1 / hatch.Pattern.LineDefinitions[0].Delta.Y;
            //dxf.AddEntity(poly);
            //dxf.AddEntity(poly2);
            //dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateBoundary(true));

            dxf.Save("hatchTest1.dxf");
            dxf = DxfDocument.Load("hatchTest1.dxf");
        }
        private static void HatchTest2()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Circle circle = new Circle(Vector3.Zero, 5);

            Ellipse ellipse = new Ellipse(Vector3.Zero,16,10);
            ellipse.Rotation = 30;
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{circle}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{ellipse})
                                                                            };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary, false);
            hatch.Pattern.Angle = 150;
            hatch.Pattern.Scale = 5;
            //hatch.Normal = new Vector3(1,1,1);
            //hatch.Elevation = 23;
            //dxf.AddEntity(poly);
            //dxf.AddEntity(circle);
            //dxf.AddEntity(ellipse);
            dxf.AddEntity(hatch);
            hatch.CreateBoundary(true);
            dxf.Save("hatchTest2.dxf");
            dxf = DxfDocument.Load("hatchTest2.dxf");
            dxf.Save("hatchTest2 copy.dxf");
        }
        private static void HatchTest3()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Ellipse ellipse = new Ellipse(Vector3.Zero, 16, 10);
            ellipse.Rotation = 0;
            ellipse.StartAngle = 0;
            ellipse.EndAngle = 180;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-8, 0));
            poly2.Vertexes.Add(new LwPolylineVertex(0, -4));
            poly2.Vertexes.Add(new LwPolylineVertex(8, 0));

            //Arc arc = new Arc(Vector3.Zero,8,180,0);
            //Line line =new Line(new Vector3(8,0,0), new Vector3(-8,0,0));

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2, ellipse})
                                                                            };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary, true);
            hatch.Pattern.Angle = 45;
            //dxf.AddEntity(poly);
            //dxf.AddEntity(ellipse);
            ////dxf.AddEntity(arc);
            ////dxf.AddEntity(line);
            //dxf.AddEntity(poly2);
            dxf.AddEntity(hatch);


            dxf.Save("hatchTest3.dxf");
        }
        private static void HatchTest4()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>() { new HatchBoundaryPath(new List<EntityObject>()) }; ;
            //List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath> {new HatchBoundaryPath(new List<Entity> {poly})};
            HatchGradientPattern pattern = new HatchGradientPattern(AciColor.Yellow, AciColor.Blue, HatchGradientPatternType.Linear);
            pattern.Origin = new Vector2(120, -365);
            Hatch hatch = new Hatch(pattern, boundary, true);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateBoundary(true));
            
            dxf.Save("HatchTest4.dxf");
            dxf = DxfDocument.Load("HatchTest4.dxf");
            dxf.Save("HatchTest4 copy.dxf");

        }
        private static void Dxf2000()
        {
            DxfDocument dxf = new DxfDocument();
            //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;

            dxf.AddEntity(line);

            dxf.Save("test2000.dxf");
        }
        private static void LwPolyline()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.SetConstantWidth(2);
            //poly.IsClosed = true;
            dxf.AddEntity(poly);

            dxf.Save("lwpolyline.dxf");

            dxf = DxfDocument.Load("lwpolyline.dxf");
        }
        private static void Polyline()
        {
            DxfDocument dxf = new DxfDocument();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            Polyline poly = new Polyline();
            poly.Vertexes.Add(new PolylineVertex(0, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(10, 10, 0));
            poly.Vertexes.Add(new PolylineVertex(20, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(30, 10, 0));
            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");

        }
        private static void Solid()
        {

            DxfDocument dxf = new DxfDocument();

            Solid solid = new Solid();
            solid.FirstVertex=new Vector2(0,0);
            solid.SecondVertex  = new Vector2(1, 0);
            solid.ThirdVertex  = new Vector2(0, 1);
            solid.FourthVertex  = new Vector2(1, 1);
            dxf.AddEntity(solid);

            dxf.Save("solid.dxf");
            //dxf = DxfDocument.Load("solid.dxf");
            //dxf.Save("solid.dxf");

        }
        private static void Face3d()
        {

            DxfDocument dxf = new DxfDocument();

            Face3d face3d = new Face3d();
            face3d.FirstVertex = new Vector3(0, 0, 0);
            face3d.SecondVertex = new Vector3(1, 0, 0);
            face3d.ThirdVertex = new Vector3(1, 1, 0);
            face3d.FourthVertex = new Vector3(0, 1, 0);
            dxf.AddEntity(face3d);

            dxf.Save("face.dxf");
            dxf = DxfDocument.Load("face.dxf");
            dxf.Save("face return.dxf");

        }
        private static void Ellipse()
        {
           
            DxfDocument dxf = new DxfDocument();

            //Line line = new Line(new Vector3(0, 0, 0), new Vector3(2 * Math.Cos(Math.PI / 4),2 * Math.Cos(Math.PI / 4), 0));

            //dxf.AddEntity(line);

            //Line line2 = new Line(new Vector3(0, 0, 0), new Vector3(0, -2, 0));
            //dxf.AddEntity(line2);

            //Arc arc=new Arc(Vector3.Zero,2,45,270);
            //dxf.AddEntity(arc);

            Ellipse ellipse = new Ellipse(new Vector3(2,2,0), 5,3);
            ellipse.Rotation = 30;
            ellipse.Normal=new Vector3(1,1,1);
            ellipse.Thickness = 2;
            dxf.AddEntity(ellipse);

            Ellipse ellipseArc = new Ellipse(new Vector3(2, 10, 0), 5, 3);
            ellipseArc.StartAngle = -45;
            ellipseArc.EndAngle = 45;
            dxf.AddEntity(ellipseArc);

            dxf.Save("ellipse.dxf");
            dxf = new DxfDocument();
            dxf = DxfDocument.Load("ellipse.dxf");

            DxfDocument load = DxfDocument.Load("test ellipse.dxf");
            load.Save("saved test ellipse.dxf");

        }
        private static void SpeedTest()
        {
            Stopwatch crono = new Stopwatch();
            const int numLines = (int)1e6; // create # lines
            string layerName = "MyLayer";
            float totalTime=0;
            
            List<EntityObject> lines = new List<EntityObject>(numLines);
            DxfDocument dxf = new DxfDocument();

            crono.Start();
            for (int i = 0; i < numLines; i++)
            {
                    //line
                Line line = new Line(new Vector3(0, i, 0), new Vector3(5, i, 0));
                line.Layer = new Layer(layerName);
                line.Layer.Color.Index = 6;
                lines.Add(line);
            }
            Console.WriteLine("Time creating entities : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.AddEntity(lines);
            Console.WriteLine("Time adding entities to document : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("speedtest (netDxf 2000).dxf");
            Console.WriteLine("Time saving file 2000 : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("speedtest (binary netDxf 2000).dxf", true);
            Console.WriteLine("Time saving binary file 2000 : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("speedtest (netDxf 2010).dxf");
            Console.WriteLine("Time saving file 2010 : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();


            crono.Start();
            dxf = DxfDocument.Load("speedtest (netDxf 2000).dxf");
            Console.WriteLine("Time loading file 2000: " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Stop();
            crono.Reset();

            crono.Start();
            dxf = DxfDocument.Load("speedtest (binary netDxf 2000).dxf");
            Console.WriteLine("Time loading binary file 2000: " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Stop();
            crono.Reset();

            crono.Start();
            dxf = DxfDocument.Load("speedtest (netDxf 2010).dxf");
            Console.WriteLine("Time loading file 2010: " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Stop();
            crono.Reset();

            Console.WriteLine("Total time : " + totalTime / 1000.0f);
            Console.ReadLine();

        }
        private static void WriteNestedInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();
            
            Block nestedBlock = new Block("Nested block");
            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            nestedBlock.Entities.Add(circle);
            
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Prompt = "InfoText";
            attdef.Alignment = TextAlignment.MiddleCenter;
            nestedBlock.AttributeDefinitions.Add(attdef);

            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert.Attributes[0].Value = 24;

            Insert nestedInsert2 = new Insert(nestedBlock, new Vector3(-20, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert2.Attributes[0].Value = -20;

            Block block = new Block("MyBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            block.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));
            block.Entities.Add(nestedInsert);
            block.Entities.Add(nestedInsert2);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);
            //dxf.AddEntity(circle); // this is not allowed the circle is already part of a block

            dxf.Save("nested insert.dxf");
            dxf = DxfDocument.Load("nested insert.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("nested insert copy.dxf");

        }
        private static void WritePolyfaceMesh()
        {
            DxfDocument dxf = new DxfDocument();


            List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                                {
                                                    new PolyfaceMeshFace(new short[] {1, 2, -3}),
                                                    new PolyfaceMeshFace(new short[] {-1, 3, -4}),
                                                    new PolyfaceMeshFace(new short[] {-1, 4, 5})
                                                };

            PolyfaceMesh mesh = new PolyfaceMesh(vertexes, faces);
            dxf.AddEntity(mesh);

            dxf.Save("mesh.dxf");
        }
        private static void WriteDxfFile()
        {
            DxfDocument dxf = new DxfDocument();

            //arc
            Arc arc = new Arc(new Vector3(10, 10, 0), 10, 45, 135);
            arc.Layer = new Layer("arc");
            arc.Layer.Color.Index = 1;
            dxf.AddEntity(arc);

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            XData xdata2 = new XData(new ApplicationRegistry("other application"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            //circle
            Vector3 extrusion = new Vector3(1, 1, 1);
            Vector3 centerWCS = new Vector3(1, 1, 1);
            Vector3 centerOCS = MathHelper.Transform(centerWCS,
                                                        extrusion,
                                                        CoordinateSystem.World,
                                                        CoordinateSystem.Object);

            Circle circle = new Circle(centerOCS, 5);
            circle.Layer = new Layer("circle with spaces");
            circle.Layer.Color=AciColor.Yellow;
            circle.LineType = LineType.Dashed;
            circle.Normal = extrusion;
            circle.XData.Add(xdata);
            circle.XData.Add(xdata2);

            dxf.AddEntity(circle);

            //points
            Point point1 = new Point(new Vector3(-3, -3, 0));
            point1.Layer = new Layer("point");
            point1.Color = new AciColor(30);
            Point point2 = new Point(new Vector3(1, 1, 1));
            point2.Layer = point1.Layer;
            point2.Layer.Color.Index = 9;
            point2.Normal = new Vector3(1, 1, 1);
            dxf.AddEntity(point1);
            dxf.AddEntity(point2);

            //3dface
            Face3d face3D = new Face3d(new Vector3(-5, -5, 5),
                                        new Vector3(5, -5, 5),
                                        new Vector3(5, 5, 5),
                                        new Vector3(-5, 5, 5));
            face3D.Layer = new Layer("3dface");
            face3D.Layer.Color.Index = 3;
            dxf.AddEntity(face3D);
            
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -50));
            polyVertex.StartWidth = 2;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, -50));
            polyVertex.StartWidth = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, 50));
            polyVertex.Bulge = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-50, 50));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes, true);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;
            dxf.AddEntity(polyline2d);

            //lightweight polyline
            LwPolylineVertex lwVertex;
            List<LwPolylineVertex> lwVertexes = new List<LwPolylineVertex>();
            lwVertex = new LwPolylineVertex(new Vector2(-25, -25));
            lwVertex.StartWidth = 2;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, -25));
            lwVertex.StartWidth = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, 25));
            lwVertex.Bulge = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(-25, 25));
            lwVertexes.Add(lwVertex);
            LwPolyline lwPolyline = new LwPolyline(lwVertexes, true);
            lwPolyline.Layer = new Layer("lwpolyline");
            lwPolyline.Layer.Color.Index = 5;
            lwPolyline.Normal = new Vector3(1, 1, 1);
            lwPolyline.Elevation = 100.0f;
            dxf.AddEntity(lwPolyline);

            // polyfaceMesh
            List<PolyfaceMeshVertex> meshVertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                                {
                                                    new PolyfaceMeshFace(new short[] {1, 2, -3}),
                                                    new PolyfaceMeshFace(new short[] {-1, 3, -4}),
                                                    new PolyfaceMeshFace(new short[] {-1, 4, 5})
                                                };

            PolyfaceMesh mesh = new PolyfaceMesh(meshVertexes, faces);
            mesh.Layer = new Layer("polyfacemesh");
            mesh.Layer.Color.Index = 104;
            dxf.AddEntity(mesh);

            //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;
            dxf.AddEntity(line);

            //3d polyline
            PolylineVertex vertex;
            List<PolylineVertex> vertexes = new List<PolylineVertex>();
            vertex = new PolylineVertex(new Vector3(-50, -50, 0));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, -50, 10));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, 50, 25));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(-50, 50, 50));
            vertexes.Add(vertex);
            Polyline polyline = new Polyline(vertexes, true);
            polyline.Layer = new Layer("polyline3d");
            polyline.Layer.Color.Index = 24;
            dxf.AddEntity(polyline);

            //block definition
            Block block = new Block("TestBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 5), new Vector3(5, 5, 5)));
            block.Entities.Add(new Line(new Vector3(5, -5, 5), new Vector3(-5, 5, 5)));
           
            //insert
            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");
            insert.Layer.Color.Index = 4;
            dxf.AddEntity(insert);

            //text
            TextStyle style=new TextStyle("True type font","Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f,style);
            text.Layer = new Layer("text");
            text.Layer.Color.Index = 8;
            text.Alignment = TextAlignment.TopRight;
            dxf.AddEntity(text);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("AutoCad2010.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("AutoCad2007.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("AutoCad2004.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("AutoCad2000.dxf");
            dxf = DxfDocument.Load("AutoCad2000.dxf");
            dxf.Save("AutoCad2000 result.dxf");
        }
        private static void WritePolyline3d()
        {
            DxfDocument dxf = new DxfDocument();

            List<PolylineVertex> vertexes = new List<PolylineVertex>{
                                                                        new PolylineVertex(0, 0, 0), 
                                                                        new PolylineVertex(10, 0, 10), 
                                                                        new PolylineVertex(10, 10, 20), 
                                                                        new PolylineVertex(0, 10, 30)
                                                                        };

            Polyline poly = new Polyline(vertexes, true);

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf polyline3d"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int16, poly.Vertexes.Count));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            poly.XData.Add(xdata);

            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");           
        }
        private static void TestLayoutRemoval()
        {
            Line line1 = new Line(new Vector2(0, 0), new Vector2(100, 100));
            Line line2 = new Line(new Vector2(100, 0), new Vector2(200, 100));
            Line line3 = new Line(new Vector2(200, 0), new Vector2(300, 100));
            Line line4 = new Line(new Vector2(200, 0), new Vector2(300, 100));
            Line line5 = new Line(new Vector2(200, 0), new Vector2(300, 100));

            Layout layout1 = new Layout("Layout1");
            Layout layout2 = new Layout("Layout2");
            Layout layout3 = new Layout("Layout3");
            Layout layout4 = new Layout("Layout4");

            DxfDocument doc = new DxfDocument();
            doc.Layouts.Add(layout1);
            doc.Layouts.Add(layout2);
            doc.Layouts.Add(layout3);
            doc.Layouts.Add(layout4);

            doc.AddEntity(line1);   // *Model_Space

            doc.ActiveLayout = layout1.Name;
            doc.AddEntity(line2);   // *Paper_Space

            doc.ActiveLayout = layout2.Name;
            doc.AddEntity(line3);   // *Paper_Space0

            doc.ActiveLayout = layout3.Name;
            doc.AddEntity(line4);   // *Paper_Space1

            doc.ActiveLayout = layout4.Name;
            doc.AddEntity(line5);   // *Paper_Space2

            doc.Layouts.Remove(layout1);

            Console.WriteLine(line2.Owner);
            Console.WriteLine(ReferenceEquals(line3.Owner, layout2.AssociatedBlock));
            Console.WriteLine(ReferenceEquals(line4.Owner, layout3.AssociatedBlock));
            Console.WriteLine(ReferenceEquals(line5.Owner, layout4.AssociatedBlock));

            Console.ReadKey();
        }
        private static void NestedBlock()
        {
            Block blockMM = new Block("BlockMM");
            blockMM.Record.Units = DrawingUnits.Millimeters;
            AttributeDefinition attDefMM = new AttributeDefinition("MyAttributeMM");
            attDefMM.Height = 1.0;
            attDefMM.Value = "This is block mm";
            blockMM.AttributeDefinitions.Add(attDefMM);
            Line line1MM = new Line(Vector2.Zero, Vector2.UnitX);
            blockMM.Entities.Add(line1MM);
            Insert insMM = new Insert(blockMM);
            insMM.TransformAttributes();

            Block blockCM = new Block("BlockCM");
            blockCM.Record.Units = DrawingUnits.Centimeters;
            AttributeDefinition attDefCM = new AttributeDefinition("MyAttributeCM");
            attDefCM.Height = 1.0;
            attDefCM.Value = "This is block cm";
            blockCM.AttributeDefinitions.Add(attDefCM);
            Line line1CM = new Line(Vector2.Zero, Vector2.UnitY);
            blockCM.Entities.Add(line1CM);
            blockCM.Entities.Add(insMM);
            Insert insCM = new Insert(blockCM);

            DxfDocument doc = new DxfDocument();
            doc.DrawingVariables.InsUnits = DrawingUnits.Meters;
            //doc.AddEntity(insMM);
            doc.AddEntity(insCM);

            doc.Save("test.dxf");
        }
        private static void BlockAttributeTransformation()
        {

            DxfDocument doc = DxfDocument.Load("Drawing1.dxf");
            Insert ins = doc.Inserts[0];
            Console.WriteLine(ins.Attributes[0].Position);
            Console.WriteLine(ins.Attributes[0].Rotation);
            Console.WriteLine(ins.Attributes[0].Normal);
            Console.WriteLine(ins.Attributes[0].Height);
            Console.WriteLine(ins.Attributes[0].WidthFactor);
            Console.WriteLine(ins.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            ins.TransformAttributes();
            Console.WriteLine(ins.Attributes[0].Position);
            Console.WriteLine(ins.Attributes[0].Rotation);
            Console.WriteLine(ins.Attributes[0].Normal);
            Console.WriteLine(ins.Attributes[0].Height);
            Console.WriteLine(ins.Attributes[0].WidthFactor);
            Console.WriteLine(ins.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            Console.WriteLine("...");
            Console.WriteLine("...");
            Console.WriteLine("...");
            DxfDocument doc2 = DxfDocument.Load("Drawing2.dxf");
            Insert ins2 = doc2.Inserts[0];
            Console.WriteLine(ins2.Attributes[0].Position);
            Console.WriteLine(ins2.Attributes[0].Rotation);
            Console.WriteLine(ins2.Attributes[0].Normal);
            Console.WriteLine(ins2.Attributes[0].Height);
            Console.WriteLine(ins2.Attributes[0].WidthFactor);
            Console.WriteLine(ins2.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            ins2.TransformAttributes();
            Console.WriteLine(ins2.Attributes[0].Position);
            Console.WriteLine(ins2.Attributes[0].Rotation);
            Console.WriteLine(ins2.Attributes[0].Normal);
            Console.WriteLine(ins2.Attributes[0].Height);
            Console.WriteLine(ins2.Attributes[0].WidthFactor);
            Console.WriteLine(ins2.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            Console.WriteLine("...");
            Console.WriteLine("...");
            Console.WriteLine("...");
            DxfDocument doc3 = DxfDocument.Load("Drawing3.dxf");
            Insert ins3 = doc3.Inserts[0];
            Console.WriteLine(ins3.Attributes[0].Position);
            Console.WriteLine(ins3.Attributes[0].Rotation);
            Console.WriteLine(ins3.Attributes[0].Normal);
            Console.WriteLine(ins3.Attributes[0].Height);
            Console.WriteLine(ins3.Attributes[0].WidthFactor);
            Console.WriteLine(ins3.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            ins3.TransformAttributes();
            Console.WriteLine(ins3.Attributes[0].Position);
            Console.WriteLine(ins3.Attributes[0].Rotation);
            Console.WriteLine(ins3.Attributes[0].Normal);
            Console.WriteLine(ins3.Attributes[0].Height);
            Console.WriteLine(ins3.Attributes[0].WidthFactor);
            Console.WriteLine(ins3.Attributes[0].ObliqueAngle);
            Console.WriteLine("...");
            Console.ReadKey();


            doc3.Save("test.dxf");

            //Block block = new Block("MyBlock");
            //block.Entities.Add(new Line(new Vector2(-5, 0), new Vector2(5, 0)));

            //AttributeDefinition attDef = new AttributeDefinition("MyAttribute");
            //attDef.Prompt = "Enter a value:";
            //attDef.Value = 0.0;
            ////attDef.Normal = Vector3.UnitX;
            //block.AttributeDefinitions.Add(attDef);

            //Insert ins1 = new Insert(block);
            //ins1.Attributes["MyAttribute"].Value = "Text";

            //Insert ins2 = new Insert(block);
            //ins2.Position = new Vector3(5, 7.5, 1);
            //ins2.Rotation = 30;
            //ins2.TransformAttributes();

            //Insert ins3 = new Insert(block);
            //ins3.Position = new Vector3(-5, -7.5, -1);
            //ins3.Rotation = 30;
            ////ins3.Normal = Vector3.UnitX;
            //ins3.TransformAttributes();

            //DxfDocument doc = new DxfDocument();
            //doc.AddEntity(ins1);
            ////doc.AddEntity(ins2);
            ////doc.AddEntity(ins3);

            //doc.Save("BlockAttributeTransformation.dxf");
        }
    }
}