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
using System.Threading;
using netDxf.Blocks;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Entities
{
    /// <summary>
    /// Holds methods to build the dimension blocks.
    /// </summary>
    internal class DimensionBlock
    {
        #region private methods

        private static string FormatDimensionText(double measure, bool angular, string userText, DimensionStyle style, Layout layout)
        {
            if (userText == " ") return null;

            string text = string.Empty;

            UnitStyleFormat unitFormat = new UnitStyleFormat
            {
                LinearDecimalPlaces = style.DIMDEC,
                AngularDecimalPlaces = style.DIMADEC == -1 ? style.DIMDEC : style.DIMADEC,
                DecimalSeparator = style.DIMDSEP.ToString(),
                FractionType = style.DIMFRAC,
                SupressLinearLeadingZeros = style.SuppressLinearLeadingZeros,
                SupressLinearTrailingZeros = style.SuppressLinearTrailingZeros,
                SupressAngularLeadingZeros = style.SuppressAngularLeadingZeros,
                SupressAngularTrailingZeros = style.SuppressAngularTrailingZeros,
                SupressZeroFeet = style.SuppressZeroFeet,
                SupressZeroInches = style.SuppressZeroInches
            };
            
            if (angular)
            {
                switch (style.DIMAUNIT)
                {
                    case AngleUnitType.DecimalDegrees:
                        text = AngleUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                    case AngleUnitType.DegreesMinutesSeconds:
                        text = AngleUnitFormat.ToDegreesMinutesSeconds(measure, unitFormat);
                        break;
                    case AngleUnitType.Gradians:
                        text = AngleUnitFormat.ToGradians(measure, unitFormat);
                        break;
                    case AngleUnitType.Radians:
                        text = AngleUnitFormat.ToRadians(measure, unitFormat);
                        break;
                    case AngleUnitType.SurveyorUnits:
                        text = AngleUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                }
            }
            else
            {
                double scale = Math.Abs(style.DIMLFAC);
                if (layout != null)
                {
                    // if DIMLFAC is negative the scale value is only applied to dimensions in PaperSpace
                    if (style.DIMLFAC < 0 && !layout.IsPaperSpace)
                        scale = 1.0;
                }

                if (style.DIMRND > 0.0)
                    measure = MathHelper.RoundToNearest(measure * scale, style.DIMRND);
                else
                    measure *= scale;

                switch (style.DIMLUNIT)
                {
                    case LinearUnitType.Architectural:
                        text = LinearUnitFormat.ToArchitectural(measure, unitFormat);
                        break;
                    case LinearUnitType.Decimal:
                        text = LinearUnitFormat.ToDecimal(measure, unitFormat);
                        break;
                    case LinearUnitType.Engineering:
                        text = LinearUnitFormat.ToEngineering(measure, unitFormat);
                        break;
                    case LinearUnitType.Fractional:
                        text = LinearUnitFormat.ToFractional(measure, unitFormat);
                        break;
                    case LinearUnitType.Scientific:
                        text = LinearUnitFormat.ToScientific(measure, unitFormat);
                        break;
                    case LinearUnitType.WindowsDesktop:
                        unitFormat.LinearDecimalPlaces = (short) Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalDigits;
                        unitFormat.DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        text = LinearUnitFormat.ToDecimal(measure * style.DIMLFAC, unitFormat);
                        break;
                }
            }
            text = style.DIMPOST.Replace("<>", text);

            if (!string.IsNullOrEmpty(userText)) text = userText.Replace("<>", text);

            return text;
        }

        private static Line DimensionLine(Vector2 start, Vector2 end, double rotation, DimensionStyle style)
        {
            double ext1 = style.DIMASZ*style.DIMSCALE;
            double ext2 = -style.DIMASZ*style.DIMSCALE;

            Block block;

            block = style.DIMSAH ? style.DIMBLK1 : style.DIMBLK;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                    ext1 = -style.DIMDLE*style.DIMSCALE;
            }

            block = style.DIMSAH ? style.DIMBLK2 : style.DIMBLK;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                    ext2 = style.DIMDLE*style.DIMSCALE;
            }

            start = Vector2.Polar(start, ext1, rotation);
            end = Vector2.Polar(end, ext2, rotation);

            return new Line(start, end)
            {
                Color = style.DIMCLRD,
                LineType = style.DIMLTYPE,
                Lineweight = style.DIMLWD
            };
        }

        private static Arc DimensionArc(Vector2 center, Vector2 start, Vector2 end, double startAngle, double endAngle, double radius, short reversed, DimensionStyle style, out double e1, out double e2)
        {
            double ext1 = style.DIMASZ*style.DIMSCALE;
            double ext2 = -style.DIMASZ*style.DIMSCALE;
            e1 = ext1;
            e2 = ext2;

            Block block;

            block = style.DIMSAH ? style.DIMBLK1 : style.DIMBLK;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext1 = 0.0;
                    e1 = 0.0;
                }
            }

            block = style.DIMSAH ? style.DIMBLK2 : style.DIMBLK;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                {
                    ext2 = 0.0;
                    e2 = 0.0;
                }
            }

            start = Vector2.Polar(start, reversed*ext1, startAngle + MathHelper.HalfPI);
            end = Vector2.Polar(end, reversed*ext2, endAngle + MathHelper.HalfPI);

            startAngle = Vector2.Angle(center, start)*MathHelper.RadToDeg;
            endAngle = Vector2.Angle(center, end)*MathHelper.RadToDeg;
            return new Arc(center, radius, startAngle, endAngle)
            {
                Color = style.DIMCLRD,
                LineType = style.DIMLTYPE,
                Lineweight = style.DIMLWD
            };
        }

        private static Line DimensionRadialLine(Vector2 start, Vector2 end, double rotation, short reversed, DimensionStyle style)
        {
            double ext = -style.DIMASZ * style.DIMSCALE;
            Block block;

            // the radial dimension only has an arrowhead at its end
            block = style.DIMSAH ? style.DIMBLK2 : style.DIMBLK;
            if (block != null)
            {
                if (block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase) ||
                    block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase))
                    ext = style.DIMDLE * style.DIMSCALE;
            }

            //start = Vector2.Polar(start, reversed * ext1, rotation);
            end = Vector2.Polar(end, reversed * ext, rotation);

            return new Line(start, end)
            {
                Color = style.DIMCLRD,
                LineType = style.DIMLTYPE,
                Lineweight = style.DIMLWD
            };
        }

        private static Line ExtensionLine(Vector2 start, Vector2 end, DimensionStyle style, LineType linetype)
        {
            return new Line(start, end)
            {
                Color = style.DIMCLRE,
                LineType = linetype,
                Lineweight = style.DIMLWE
            };
        }

        private static EntityObject StartArrowHead(Vector2 position, double rotation, DimensionStyle style)
        {
            Block block = style.DIMSAH ? style.DIMBLK1 : style.DIMBLK;

            if (block == null)
            {
                Vector2 arrowRef = Vector2.Polar(position, -style.DIMASZ*style.DIMSCALE, rotation);
                Solid arrow = new Solid(position,
                    Vector2.Polar(arrowRef, -(style.DIMASZ/6)*style.DIMSCALE, rotation + MathHelper.HalfPI),
                    Vector2.Polar(arrowRef, (style.DIMASZ/6)*style.DIMSCALE, rotation + MathHelper.HalfPI))
                {
                    Color = style.DIMCLRD
                };
                return arrow;
            }
            else
            {
                Insert arrow = new Insert(block, position)
                {
                    Color = style.DIMCLRD,
                    Scale = new Vector3(style.DIMASZ*style.DIMSCALE),
                    Rotation = rotation*MathHelper.RadToDeg
                };
                return arrow;
            }
        }

        private static EntityObject EndArrowHead(Vector2 position, double rotation, DimensionStyle style)
        {
            Block block = style.DIMSAH ? style.DIMBLK2 : style.DIMBLK;

            if (block == null)
            {
                Vector2 arrowRef = Vector2.Polar(position, -style.DIMASZ*style.DIMSCALE, rotation);
                Solid arrow = new Solid(position,
                    Vector2.Polar(arrowRef, -(style.DIMASZ/6)*style.DIMSCALE, rotation + MathHelper.HalfPI),
                    Vector2.Polar(arrowRef, (style.DIMASZ/6)*style.DIMSCALE, rotation + MathHelper.HalfPI))
                {
                    Color = style.DIMCLRD
                };
                return arrow;
            }
            else
            {
                Insert arrow = new Insert(block, position)
                {
                    Color = style.DIMCLRD,
                    Scale = new Vector3(style.DIMASZ*style.DIMSCALE),
                    Rotation = rotation*MathHelper.RadToDeg
                };
                return arrow;
            }
        }

        private static MText DimensionText(Vector2 position, double rotation, string text, DimensionStyle style)
        {
            if (string.IsNullOrEmpty(text)) return null;

            MText mText = new MText(text, position, style.DIMTXT*style.DIMSCALE, 0.0, style.DIMTXSTY)
            {
                Color = style.DIMCLRT,
                AttachmentPoint = MTextAttachmentPoint.BottomCenter,
                Rotation = rotation * MathHelper.RadToDeg
            };

            return mText;
        }

        private static IEnumerable<EntityObject> CenterCross(Vector2 center, double radius, DimensionStyle style)
        {
            List<EntityObject> lines = new List<EntityObject>();
            if (MathHelper.IsZero(style.DIMCEN))
                return lines;

            Vector2 c1;
            Vector2 c2;
            double dist = Math.Abs(style.DIMCEN * style.DIMSCALE);

            // center mark
            c1 = new Vector2(0.0, -dist) + center;
            c2 = new Vector2(0.0, dist) + center;
            lines.Add(new Line(c1, c2) { Color = style.DIMCLRE, Lineweight = style.DIMLWE });
            c1 = new Vector2(-dist, 0.0) + center;
            c2 = new Vector2(dist, 0.0) + center;
            lines.Add(new Line(c1, c2) { Color = style.DIMCLRE, Lineweight = style.DIMLWE });

            // center lines
            if (style.DIMCEN < 0)
            {
                c1 = new Vector2(2 * dist, 0.0) + center;
                c2 = new Vector2(radius + dist, 0.0) + center;
                lines.Add(new Line(c1, c2) { Color = style.DIMCLRE, Lineweight = style.DIMLWE });

                c1 = new Vector2(-2 * dist, 0.0) + center;
                c2 = new Vector2(-radius - dist, 0.0) + center;
                lines.Add(new Line(c1, c2){Color = style.DIMCLRE,Lineweight = style.DIMLWE});

                c1 = new Vector2(0.0, 2 * dist) + center;
                c2 = new Vector2(0.0, radius + dist) + center;
                lines.Add(new Line(c1, c2) { Color = style.DIMCLRE, Lineweight = style.DIMLWE });

                c1 = new Vector2(0.0, -2 * dist) + center;
                c2 = new Vector2(0.0, -radius - dist) + center;
                lines.Add(new Line(c1, c2) { Color = style.DIMCLRE, Lineweight = style.DIMLWE });
            }
            return lines;
        }

        private static Vector2 FindIntersection(Vector2 point0, Vector2 dir0, Vector2 point1, Vector2 dir1, double threshold = MathHelper.Epsilon)
        {
            // test for parallelism.
            if (Vector2.AreParallel(dir0, dir1, threshold))
                return new Vector2(double.NaN, double.NaN);

            // lines are not parallel
            Vector2 vect = point1 - point0;
            double cross = Vector2.CrossProduct(dir0, dir1);
            double s = (vect.X*dir1.Y - vect.Y*dir1.X)/cross;
            return point0 + s*dir0;
        }

        #endregion

        #region public methods

        public static Block Build(LinearDimension dim, string name)
        {
            Vector2 ref1;
            Vector2 ref2;
            Vector2 dimRef1;
            Vector2 dimRef2;
            double measure = dim.Measurement;
            bool reversed = false;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal
            Vector3 refPoint;

            refPoint = MathHelper.Transform(dim.FirstReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            ref1 = new Vector2(refPoint.X, refPoint.Y);
            double elevation = refPoint.Z;

            refPoint = MathHelper.Transform(dim.SecondReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            ref2 = new Vector2(refPoint.X, refPoint.Y);
            Vector2 midRef = Vector2.MidPoint(ref1, ref2);
            //double refAngle = Vector2.Angle(ref1, ref2);
            //if (refAngle > MathHelper.HalfPI && refAngle <= MathHelper.ThreeHalfPI)
            //{
            //    Vector2 tmp = ref1;
            //    ref1 = ref2;
            //    ref2 = tmp;
            //}

            double dimRotation = dim.Rotation * MathHelper.DegToRad;
            Vector2 dimRef1tmp = new Vector2(-measure * 0.5, dim.Offset);
            Vector2 dimRef2tmp = new Vector2(measure * 0.5, dim.Offset);
            dimRef1 = MathHelper.Transform(dimRef1tmp, dimRotation, CoordinateSystem.Object, CoordinateSystem.World) + midRef;
            dimRef2 = MathHelper.Transform(dimRef2tmp, dimRotation, CoordinateSystem.Object, CoordinateSystem.World) + midRef;
            Vector2 midDim = Vector2.MidPoint(dimRef1, dimRef2);
            double relativeAngle = Vector2.AngleBetween(ref2 - ref1, dimRef2 - dimRef1);
            if (relativeAngle > MathHelper.HalfPI && relativeAngle <= MathHelper.ThreeHalfPI)
            {
                Vector2 tmp = ref1;
                ref1 = ref2;
                ref2 = tmp;
                reversed = true;
            }

            // reference points
            Layer defPointLayer = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPointLayer });
            entities.Add(new Point(ref2) { Layer = defPointLayer });
            entities.Add(reversed
                ? new Point(dimRef1) {Layer = defPointLayer}
                : new Point(dimRef2) {Layer = defPointLayer});

            // dimension line
            entities.Add(DimensionLine(dimRef1, dimRef2, dimRotation, style));

            // extension lines
            Vector2 dirRef1 = Vector2.Normalize(dimRef1 - ref1);
            Vector2 dirRef2 = Vector2.Normalize(dimRef2 - ref2);
            double dimexo = style.DIMEXO * style.DIMSCALE;
            double dimexe = style.DIMEXE * style.DIMSCALE;
            if (!style.DIMSE1)
                entities.Add(ExtensionLine(ref1 + dimexo * dirRef1, dimRef1 + dimexe * dirRef1, style, style.DIMLTEX1));
            if (!style.DIMSE2)
                entities.Add(ExtensionLine(ref2 + dimexo * dirRef2, dimRef2 + dimexe * dirRef2, style, style.DIMLTEX2));

            // dimension arrowheads
            if (reversed)
            {
                entities.Add(StartArrowHead(dimRef2, dimRotation, style));
                entities.Add(EndArrowHead(dimRef1, dimRotation + MathHelper.PI, style));
            }
            else
            {
                entities.Add(StartArrowHead(dimRef1, dimRotation + MathHelper.PI, style));
                entities.Add(EndArrowHead(dimRef2, dimRotation, style));
            }

            // dimension text
            string text = FormatDimensionText(measure, false, dim.UserText, style, dim.Owner.Record.Layout);

            double textRot = dimRotation;
            if (textRot > MathHelper.HalfPI && textRot <= MathHelper.ThreeHalfPI)
                textRot += MathHelper.PI;

            MText mText = DimensionText(Vector2.Polar(midDim, style.DIMGAP * style.DIMSCALE, textRot + MathHelper.HalfPI), textRot, text, style);
            if (mText != null) entities.Add(mText);

            Vector3 defPoint = reversed ? new Vector3(dimRef1.X, dimRef1.Y, elevation) : new Vector3(dimRef2.X, dimRef2.Y, elevation);
            dim.DefinitionPoint = MathHelper.Transform(defPoint, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(midDim.X, midDim.Y, elevation); // this value is in OCS

            // drawing block
            return new Block(name, false, entities, null) { Flags = BlockTypeFlags.AnonymousBlock };
        }

        public static Block Build(AlignedDimension dim, string name)
        {
            Vector2 ref1;
            Vector2 ref2;
            Vector2 dimRef1;
            Vector2 dimRef2;
            bool reversed = false;
            double measure = dim.Measurement;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal

            Vector3 refPoint = MathHelper.Transform(dim.FirstReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            ref1 = new Vector2(refPoint.X, refPoint.Y);
            double elevation = refPoint.Z;

            refPoint = MathHelper.Transform(dim.SecondReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            ref2 = new Vector2(refPoint.X, refPoint.Y);

            double refAngle = Vector2.Angle(ref1, ref2);
            if (refAngle > MathHelper.HalfPI && refAngle <= MathHelper.ThreeHalfPI)
            {
                Vector2 tmp = ref1;
                ref1 = ref2;
                ref2 = tmp;
                refAngle += MathHelper.PI;
                reversed = true;
            }

            dimRef1 = Vector2.Polar(ref1, dim.Offset, refAngle + MathHelper.HalfPI);
            dimRef2 = Vector2.Polar(ref2, dim.Offset, refAngle + MathHelper.HalfPI);
            Vector2 midDim = Vector2.MidPoint(dimRef1, dimRef2);

            // reference points
            Layer defPointLayer = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPointLayer });
            entities.Add(new Point(ref2) { Layer = defPointLayer });
            entities.Add(reversed
                ? new Point(dimRef1) {Layer = defPointLayer}
                : new Point(dimRef2) {Layer = defPointLayer});

            // dimension lines
            entities.Add(DimensionLine(dimRef1, dimRef2, refAngle, style));

            // extension lines    
            double dimexo = Math.Sign(dim.Offset) * style.DIMEXO * style.DIMSCALE;
            double dimexe = Math.Sign(dim.Offset) * style.DIMEXE * style.DIMSCALE;
            if (!style.DIMSE1)
                entities.Add(ExtensionLine(Vector2.Polar(ref1, dimexo, refAngle + MathHelper.HalfPI), Vector2.Polar(dimRef1, dimexe, refAngle + MathHelper.HalfPI), style, style.DIMLTEX1));
            if (!style.DIMSE2)
                entities.Add(ExtensionLine(Vector2.Polar(ref2, dimexo, refAngle + MathHelper.HalfPI), Vector2.Polar(dimRef2, dimexe, refAngle + MathHelper.HalfPI), style, style.DIMLTEX2));

            // dimension arrowheads
            if (reversed)
            {
                entities.Add(StartArrowHead(dimRef2, refAngle, style));
                entities.Add(EndArrowHead(dimRef1, refAngle + MathHelper.PI, style));
            }
            else
            {
                entities.Add(StartArrowHead(dimRef1, refAngle + MathHelper.PI, style));
                entities.Add(EndArrowHead(dimRef2, refAngle, style));
            }

            // dimension text
            string text = FormatDimensionText(measure, false, dim.UserText, style, dim.Owner.Record.Layout);

            MText mText = DimensionText(Vector2.Polar(midDim, style.DIMGAP * style.DIMSCALE, refAngle + MathHelper.HalfPI), refAngle, text, style);
            if (mText != null) entities.Add(mText);

            Vector3 defPoint = reversed ? new Vector3(dimRef1.X, dimRef1.Y, elevation) : new Vector3(dimRef2.X, dimRef2.Y, elevation);
            dim.DefinitionPoint = MathHelper.Transform(defPoint, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(midDim.X, midDim.Y, elevation); // this value is in OCS

            // drawing block
            return new Block(name, false, entities, null) { Flags = BlockTypeFlags.AnonymousBlock };
        }

        public static Block Build(Angular2LineDimension dim, string name)
        {
            double offset = dim.Offset;
            double side = Math.Sign(dim.Offset);
            double measure = dim.Measurement;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal
            IList<Vector3> ocsPoints = MathHelper.Transform(new[]{dim.StartFirstLine, dim.EndFirstLine, dim.StartSecondLine, dim.EndSecondLine}, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 refPoint;
            
            refPoint = ocsPoints[0];
            Vector2 ref1Start = new Vector2(refPoint.X, refPoint.Y);
            double elevation = refPoint.Z;

            refPoint = ocsPoints[1];
            Vector2 ref1End = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[2];
            Vector2 ref2Start = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[3];
            Vector2 ref2End = new Vector2(refPoint.X, refPoint.Y);

            Vector2 dirRef1 = ref1End - ref1Start;
            Vector2 dirRef2 = ref2End - ref2Start;
            if (Vector2.AreParallel(dirRef1, dirRef2))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            Vector2 center = FindIntersection(ref1Start, dirRef1, ref2Start, dirRef2);
            double startAngle = Vector2.Angle(ref1Start, ref1End);
            double endAngle = Vector2.Angle(ref2Start, ref2End);
            double cross = Vector2.CrossProduct(dirRef1, dirRef2);
            double aperture = Vector2.AngleBetween(dirRef1, dirRef2);

            if (cross < 0)
            {
                Vector2 tmp1 = ref1Start;
                Vector2 tmp2 = ref1End;
                ref1Start = ref2Start;
                ref1End = ref2End;
                ref2Start = tmp1;
                ref2End = tmp2;
                double tmp = startAngle;
                startAngle = endAngle;
                endAngle = tmp;
            }
            short reversed = 1;
            if (side < 0)
            {
                Vector2 tmp1 = ref1Start;
                Vector2 tmp2 = ref2Start;
                ref1Start = ref1End;
                ref1End = tmp1;
                ref2Start = ref2End;
                ref2End = tmp2;
                reversed = -1;
            }

            Vector2 dimRef1 = Vector2.Polar(center, offset, startAngle);
            Vector2 dimRef2 = Vector2.Polar(center, offset, endAngle);
            Vector2 midDim = Vector2.Polar(center, offset, startAngle + aperture*0.5);

            // reference points
            Layer defPoints = new Layer("Defpoints") {Plot = false};
            entities.Add(new Point(ref1Start) {Layer = defPoints});
            entities.Add(new Point(ref1End) {Layer = defPoints});
            entities.Add(new Point(ref2Start) {Layer = defPoints});
            entities.Add(new Point(ref2End) {Layer = defPoints});

            // dimension lines
            double ext1;
            double ext2;
            entities.Add(DimensionArc(center, dimRef1, dimRef2, startAngle, endAngle, Math.Abs(offset), reversed, style, out ext1, out ext2));

            // dimension arrows
            double angle1 = Math.Asin(ext1*0.5/Math.Abs(offset));
            double angle2 = Math.Asin(ext2*0.5/Math.Abs(offset));
            entities.Add(StartArrowHead(dimRef1, (1 - reversed)*MathHelper.HalfPI + angle1 + startAngle - MathHelper.HalfPI, style));
            entities.Add(EndArrowHead(dimRef2, (1 - reversed)*MathHelper.HalfPI + angle2 + endAngle + MathHelper.HalfPI, style));

            // dimension lines         
            double dimexo = side * style.DIMEXO * style.DIMSCALE;
            double dimexe = side * style.DIMEXE * style.DIMSCALE;
            // the dimension line is only drawn if the end of the extension line is outside the line segment
            int t;
            t = MathHelper.PointInSegment(dimRef1, ref1Start, ref1End);
            if (!style.DIMSE1 && t != 0)
            {
                Vector2 s;
                s = Vector2.Polar(t < 0 ? ref1Start : ref1End, t * dimexo, startAngle);
                entities.Add(ExtensionLine(s, Vector2.Polar(dimRef1, t*dimexe, startAngle), style, style.DIMLTEX1));
            }

            t = MathHelper.PointInSegment(dimRef2, ref2Start, ref2End);
            if (!style.DIMSE2 && t != 0)
            {
                Vector2 s;
                if (t < 0)
                    s = Vector2.Polar(ref2Start, t * dimexo, endAngle);
                else
                    s = Vector2.Polar(ref2End, t * dimexo, endAngle);
                entities.Add(ExtensionLine(s, Vector2.Polar(dimRef2, t*dimexe, endAngle), style, style.DIMLTEX1));
            }

            // dimension text
            string text = FormatDimensionText(measure, true, dim.UserText, style, dim.Owner.Record.Layout);

            double extRot = startAngle + aperture*0.5;
            double textRot = extRot - MathHelper.HalfPI;

            MText mText = DimensionText(Vector2.Polar(midDim, style.DIMGAP * style.DIMSCALE, extRot), textRot, text, style);
            if (mText != null) entities.Add(mText);

            dim.DefinitionPoint = dim.EndSecondLine;
            dim.MidTextPoint = new Vector3(midDim.X, midDim.Y, elevation); // this value is in OCS
            dim.ArcDefinitionPoint = dim.MidTextPoint; // this value is in OCS

            // drawing block
            return new Block(name, false, entities, null) {Flags = BlockTypeFlags.AnonymousBlock};
        }

        public static Block Build(Angular3PointDimension dim, string name)
        {
            double offset = Math.Abs(dim.Offset);
            double side = Math.Sign(dim.Offset);
            double measure = dim.Measurement;
            double aperture = measure*MathHelper.DegToRad;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal
            IList<Vector3> ocsPoints = MathHelper.Transform(new[] { dim.CenterPoint, dim.StartPoint, dim.EndPoint}, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 refPoint;

            refPoint = ocsPoints[0];
            Vector2 refCenter = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[1];
            Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[2];
            Vector2 ref2 = new Vector2(refPoint.X, refPoint.Y);

            double elevation = refPoint.Z;

            if (side < 0)
            {
                Vector2 tmp = ref1;
                ref1 = ref2;
                ref2 = tmp;
            }

            double startAngle = Vector2.Angle(refCenter, ref1);
            double endAngle = Vector2.Angle(refCenter, ref2);
            Vector2 dimRef1 = Vector2.Polar(refCenter, offset, startAngle);
            Vector2 dimRef2 = Vector2.Polar(refCenter, offset, endAngle);
            double midRot = startAngle + aperture*0.5;
            Vector2 midDim = Vector2.Polar(refCenter, offset, midRot);

            // reference points
            Layer defPoints = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPoints });
            entities.Add( new Point(ref2) { Layer = defPoints });
            entities.Add( new Point(refCenter) { Layer = defPoints });

            // dimension lines
            double ext1;
            double ext2;
            entities.Add(DimensionArc(refCenter, dimRef1, dimRef2, startAngle, endAngle, Math.Abs(offset), 1, style, out ext1, out ext2));

            // dimension arrows
            double angle1 = Math.Asin(ext1 * 0.5 / Math.Abs(offset));
            double angle2 = Math.Asin(ext2 * 0.5 / Math.Abs(offset));
            entities.Add( StartArrowHead(dimRef1,angle1 + startAngle - MathHelper.HalfPI, style));
            entities.Add( EndArrowHead(dimRef2, angle2 + endAngle + MathHelper.HalfPI, style));

            // dimension lines
            double dimexo = style.DIMEXO * style.DIMSCALE;
            double dimexe = style.DIMEXE * style.DIMSCALE;
            if (!style.DIMSE1) entities.Add(ExtensionLine(Vector2.Polar(ref1, dimexo, startAngle), Vector2.Polar(dimRef1, dimexe, startAngle), style, style.DIMLTEX1));
            if (!style.DIMSE2) entities.Add(ExtensionLine(Vector2.Polar(ref2, dimexo, endAngle), Vector2.Polar(dimRef2, dimexe, endAngle), style, style.DIMLTEX1));

            // dimension text
            string text = FormatDimensionText(measure, true, dim.UserText, style, dim.Owner.Record.Layout);

            double textRot = midRot - MathHelper.HalfPI;
            double gap = style.DIMGAP*style.DIMSCALE;
            if (textRot < MathHelper.PI)
            {
                textRot += MathHelper.PI;
                gap *= -1;
            }
                
            MText mText = DimensionText(Vector2.Polar(midDim, gap , midRot), textRot, text, style);
            if (mText != null) entities.Add(mText);

            dim.DefinitionPoint = MathHelper.Transform(new Vector3(midDim.X, midDim.Y, elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(midDim.X, midDim.Y, elevation); // this value is in OCS

            // drawing block
            return new Block(name, false, entities, null);
        }

        public static Block Build(DiametricDimension dim, string name)
        {
            double offset = dim.Offset;
            double measure = dim.Measurement;
            double radius = measure * 0.5;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal
            Vector3 refPoint;

            refPoint = MathHelper.Transform(dim.CenterPoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);

            Vector2 centerRef = new Vector2(refPoint.X, refPoint.Y);
            double elev = refPoint.Z;

            refPoint = MathHelper.Transform(dim.ReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);

            double angleRef = Vector2.Angle(centerRef, ref1);
            Vector2 ref2 = Vector2.Polar(ref1, -measure, angleRef);

            short reverse = 1;
            if(angleRef>MathHelper.HalfPI && angleRef<=MathHelper.ThreeHalfPI)
                reverse = -1;

            short side;
            double minOffset = 2 * style.DIMASZ + style.DIMGAP * style.DIMSCALE;
            
            if (offset >= radius && offset <= radius + minOffset)
            {
                offset = radius + minOffset;
                side = -1;
            }
            else if (offset >= radius - minOffset && offset <= radius)
            {
                offset = radius - minOffset;
                side = 1;
            }
            else if (offset > radius)
                side = -1;
            else
                side = 1;

            Vector2 dimRef = Vector2.Polar(centerRef, offset, angleRef);

            // reference points
            Layer defPoints = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPoints });

            // dimension lines
            entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, side, style));

            // center cross
            entities.AddRange(CenterCross(centerRef, radius, style));

            // dimension arrows
            entities.Add(EndArrowHead(ref1, (1 - side) * MathHelper.HalfPI + angleRef, style));

            // dimension text
            string text = "Ø" + FormatDimensionText(measure, false, dim.UserText, style, dim.Owner.Record.Layout);

            double textRot = angleRef;
            if (textRot > MathHelper.HalfPI && textRot <= MathHelper.ThreeHalfPI)
                textRot += MathHelper.PI;

            MText mText = DimensionText(Vector2.Polar(dimRef, -reverse * side * style.DIMGAP * style.DIMSCALE, textRot), textRot, text, style);
            if (mText != null)
            {
                mText.AttachmentPoint = reverse*side<0 ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
                entities.Add(mText);
            }

            dim.DefinitionPoint = MathHelper.Transform(new Vector3(ref2.X, ref2.Y, elev), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(dimRef.X, dimRef.Y, elev); // this value is in OCS

            return new Block(name, false, entities, null);
        }

        public static Block Build(RadialDimension dim, string name)
        {
            double offset = dim.Offset;
            double measure = dim.Measurement;
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            // we will build the dimension block in object coordinates with normal the dimension normal
            Vector3 refPoint;

            refPoint = MathHelper.Transform(dim.CenterPoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);

            Vector2 centerRef = new Vector2(refPoint.X, refPoint.Y);
            double elev = refPoint.Z;

            refPoint = MathHelper.Transform(dim.ReferencePoint, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);

            double angleRef = Vector2.Angle(centerRef, ref1);
            Vector2 ref2 = Vector2.Polar(ref1, -measure, angleRef);

            short reverse = 1;
            if (angleRef > MathHelper.HalfPI && angleRef <= MathHelper.ThreeHalfPI)
                reverse = -1;

            short side;
            double minOffset = 2 * style.DIMASZ + style.DIMGAP * style.DIMSCALE;
            if (offset >= measure && offset <= measure + minOffset)
            {
                offset = measure + minOffset;
                side = -1;
            }
            else if (offset >= measure - minOffset && offset <= measure)
            {
                offset = measure - minOffset;
                side = 1;
            }     
            else if (offset > measure)
                side = -1;
            else
                side = 1;

            Vector2 dimRef = Vector2.Polar(centerRef, offset, angleRef);

            // reference points
            Layer defPoints = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(ref1) { Layer = defPoints });

            // dimension lines
            entities.Add(DimensionRadialLine(dimRef, ref1, angleRef, side, style));

            // center cross
            entities.AddRange(CenterCross(centerRef, measure, style));

            // dimension arrows
            entities.Add(EndArrowHead(ref1, (1 - side) * MathHelper.HalfPI + angleRef, style));

            // dimension text
            string text = "R" + FormatDimensionText(measure, false, dim.UserText, style, dim.Owner.Record.Layout);

            double textRot = angleRef;
            if (textRot > MathHelper.HalfPI && textRot <= MathHelper.ThreeHalfPI)
                textRot += MathHelper.PI;

            MText mText = DimensionText(Vector2.Polar(dimRef, -reverse * side * style.DIMGAP * style.DIMSCALE, textRot), textRot, text, style);
            if (mText != null)
            {
                mText.AttachmentPoint = reverse * side < 0 ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
                entities.Add(mText);
            }

            dim.DefinitionPoint = MathHelper.Transform(new Vector3(ref2.X, ref2.Y, elev), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(dimRef.X, dimRef.Y, elev); // this value is in OCS

            return new Block(name, false, entities, null);
            
        }

        public static Block Build(OrdinateDimension dim, string name)
        {
            DimensionStyle style = dim.Style;
            List<EntityObject> entities = new List<EntityObject>();

            double measure = dim.Measurement;

            dim.DefinitionPoint = dim.Origin;
            double angle = dim.Rotation * MathHelper.DegToRad;

            Vector3 localPoint = MathHelper.Transform(dim.Origin, dim.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 refCenter = new Vector2(localPoint.X, localPoint.Y);

            double elev = localPoint.Z;

            Vector2 startPoint = refCenter + MathHelper.Transform(dim.ReferencePoint, angle, CoordinateSystem.Object, CoordinateSystem.World);
            dim.FirstPoint = MathHelper.Transform(new Vector3(startPoint.X, startPoint.Y, elev), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);

            if (dim.Axis == OrdinateDimensionAxis.X)
                angle += MathHelper.HalfPI;
            Vector2 endPoint = Vector2.Polar(startPoint, dim.Length, angle);
            dim.SecondPoint = MathHelper.Transform(new Vector3(endPoint.X, endPoint.Y, elev), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);

            // reference points
            Layer defPoints = new Layer("Defpoints") { Plot = false };
            entities.Add(new Point(refCenter) { Layer = defPoints });
            entities.Add(new Point(startPoint) { Layer = defPoints });

            short side = 1;
            if (dim.Length < 0) side = -1;

            // dimension lines
            entities.Add(new Line(Vector2.Polar(startPoint, side * style.DIMEXO * style.DIMSCALE, angle), endPoint));

            // dimension text
            Vector2 midText = Vector2.Polar(startPoint, dim.Length + side * style.DIMGAP * style.DIMSCALE, angle);
            dim.MidTextPoint = new Vector3(midText.X, midText.Y, elev); // this value is in OCS

            string text = FormatDimensionText(measure, false, dim.UserText, style, dim.Owner.Record.Layout);

            MText mText = DimensionText(midText, angle, text, style);
            if (mText != null)
            {
                mText.AttachmentPoint = side<0 ? MTextAttachmentPoint.MiddleRight : MTextAttachmentPoint.MiddleLeft;
                entities.Add(mText);
            }

            // drawing block
            return new Block(name, false, entities, null);
        }

        #endregion
    }
}