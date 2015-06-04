// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfHeader : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public int NextAvailableHandle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfHeader(DxfAcadVer version, int id)
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

            Begin();
            Default();
            End(NextAvailableHandle);

            return Build();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Begin()
        {
            Add(0, DxfCodeName.Section);
            Add(2, "HEADER");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void VarName(string name)
        {
            Add(9, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        private void AcadVer(DxfAcadVer version)
        {
            VarName("$ACADVER");
            Add(1, version.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private void Default()
        {
            AcadVer(Version);

            VarName("$DWGCODEPAGE");
            Add(3, "ANSI_" + Encoding.Default.WindowsCodePage);

            /*
            // Angle 0 direction
            VarName("$ANGBASE");
            Add(50, 0.0);

            // 1 = clockwise angles, 0 = counterclockwise
            VarName("$ANGDIR");
            Add(70, 1);

            // Attribute entry dialogs, 1 = on, 0 = off
            VarName("$ATTDIA");
            Add(70, 0);

            // Attribute visibility: 0 = none, 1 = normal, 2 = all
            VarName("$ATTMODE");
            Add(70, 0);

            // Attribute prompting during INSERT, 1 = on, 0 = off
            VarName("$ATTREQ");
            Add(70, 0);

            // Units format for angles
            VarName("$AUNITS");
            70, 

            // Units precision for angles 
            VarName("$AUPREC");
            70, 

            // Axis on if nonzero (not functional in Release 12)
            VarName("$AXISMODE");
            Add(70, 0);

            // Axis X and Y tick spacing (not functional in Release 12)
            VarName("$AXISUNIT");
            Add(10, 0.0);
            Add(20, 0.0);

            // Blip mode on if nonzero
            VarName("$BLIPMODE");
            Add(70, 0);

            // Entity color number; 0 = BYBLOCK, 256 = BYLAYER
            VarName("$CECOLOR");
            Add(62, 0);

            // Entity linetype name, or BYBLOCK or BYLAYER
            VarName("$CELTYPE");
            Add(6, "BYLAYER");

            // First chamfer distance
            VarName("$CHAMFERA");
            Add(40, 0.0);

            // Second chamfer distance
            VarName("$CHAMFERB");
            Add(40, 0.0);

            // Current layer name
            VarName("$CLAYER");
            Add(8, "0");

            // 0 = static coordinate display, 1 = continuous update, 2 = "d<a" format
            VarName("$COORDS");
            Add(70, 0);

            // Alternate unit dimensioning performed if nonzero
            VarName("$DIMALT");
            Add(70, 0);

            // Alternate unit decimal places
            VarName("$DIMALTD");
            Add(70, );

            // Alternate unit scale factor
            VarName("$DIMALTF");
            Add(40, 0.0);

            // Alternate dimensioning suffix
            VarName("$DIMAPOST");
            Add(1, "");

            // 1 = create associative dimensioning, 0 = draw individual entities
            VarName("$DIMASO");
            Add(70, );

            // Dimensioning arrow size
            VarName("$DIMASZ");
            Add(40, );

            // Arrow block name
            VarName("$DIMBLK");
            Add(2, "");

            // First arrow block name
            VarName("$DIMBLK1");
            Add(1, "");

            // Second arrow block name
            VarName("$DIMBLK2");
            Add(1, "");

            // Size of center mark/lines
            VarName("$DIMCEN");
            Add(40, 0.0);

            // 
            VarName("$DIMCLRD");
            Add(70, 0);

            // 
            VarName("$DIMCLRE");
            Add(70, 0);

            // 
            VarName("$DIMCLRT");
            Add(70, 0);

            // 
            VarName("$DIMDLE");
            Add(70, 0);

            // 
            VarName("$DIMDLI");
            Add(70, 0);

            // 
            VarName("$DIMEXE");
            Add(70, 0);

            // 
            VarName("$DIMEXO");
            Add(70, 0);

            // 
            VarName("$DIMGAP");
            Add(70, 0);

            // 
            VarName("$DIMLFAC");
            Add(70, 0);

            // 
            VarName("$DIMLIM");
            Add(70, 0);

            // 
            VarName("$DIMPOST");
            Add(70, 0);

            // 
            VarName("$DIMRND");
            Add(70, 0);

            // 
            VarName("$DIMSAH");
            Add(70, 0);

            // 
            VarName("$DIMSCALE");
            Add(70, 0);

            // 
            VarName("$DIMSE1");
            Add(70, 0);

            // 
            VarName("$DIMSE2");
            Add(70, 0);

            // 
            VarName("$DIMSHO");
            Add(70, 0);

            // 
            VarName("$DIMSOXD");
            Add(70, 0);

            // 
            VarName("$DIMSTYLE");
            Add(70, 0);

            // 
            VarName("$DIMTAD");
            Add(70, 0);

            // 
            VarName("$DIMTFAC");
            Add(70, 0);

            // 
            VarName("$DIMTIH");
            Add(70, 0);

            // 
            VarName("$DIMTIX");
            Add(70, 0);

            // 
            VarName("$DIMTM");
            Add(70, 0);

            // 
            VarName("$DIMTOFL");
            Add(70, 0);

            // 
            VarName("$DIMTOH");
            Add(70, 0);

            // 
            VarName("$DIMTOL");
            Add(70, 0);

            // 
            VarName("$DIMTP");
            Add(70, 0);

            // 
            VarName("$DIMTSZ");
            Add(70, 0);

            // 
            VarName("$DIMTVP");
            Add(70, 0);

            // 
            VarName("$DIMTXT");
            Add(70, 0);

            // 
            VarName("$DIMZIN");
            Add(70, 0);

            // 
            VarName("$DWGCODEPAGE");
            Add(70, 0);

            // 
            VarName("$DRAGMODE");
            Add(70, 0);

            // 
            VarName("$ELEVATION");
            Add(70, 0);
            */

            // X, Y, and Z drawing extents upper-right corner (in WCS)
            VarName("$EXTMAX");
            Add(10, 1260.0);
            Add(20, 891.0);

            // X, Y, and Z drawing extents lower-left corner (in WCS)
            VarName("$EXTMIN");
            Add(10, 0.0);
            Add(20, 0.0);

            /*
            // 
            VarName("$FILLETRAD");
            Add(70, 0);

            // 
            VarName("$FILLMODE");
            Add(70, 0);

            // 
            VarName("$HANDLING");
            Add(70, 0);

            // 
            VarName("$HANDSEED");
            Add(70, 0);

            // 
            VarName("$INSBASE");
            Add(70, 0);

            // 
            VarName("$LIMCHECK");
            Add(70, 0);

            // 
            VarName("$LIMMAX");
            Add(70, 0);

            // 
            VarName("$LIMMIN");
            Add(70, 0);

            // 
            VarName("$LTSCALE");
            Add(70, 0);

            // 
            VarName("$LUNITS");
            Add(70, 0);

            // 
            VarName("$LUPREC");
            Add(70, 0);

            // 
            VarName("$MAXACTVP");
            Add(70, 0);

            // 
            VarName("$MENU");
            Add(70, 0);

            // 
            VarName("$MIRRTEXT");
            Add(70, 0);

            // 
            VarName("$ORTHOMODE");
            Add(70, 0);

            // 
            VarName("$OSMODE");
            Add(70, 0);

            // 
            VarName("$PDMODE");
            Add(70, 0);

            // 
            VarName("$PDSIZE");
            Add(70, 0);

            // 
            VarName("$PELEVATION");
            Add(70, 0);

            // 
            VarName("$PEXTMAX");
            Add(70, 0);

            // 
            VarName("$PEXTMIN");
            Add(70, 0);

            // 
            VarName("$PLIMCHECK");
            Add(70, 0);

            // 
            VarName("$PLIMMAX");
            Add(70, 0);

            // 
            VarName("$PLIMMIN");
            Add(70, 0);

            // 
            VarName("$PLINEGEN");
            Add(70, 0);

            // 
            VarName("$PLINEWID");
            Add(70, 0);

            // 
            VarName("$PSLTSCALE");
            Add(70, 0);

            // 
            VarName("$PUCSNAME");
            Add(70, 0);

            // 
            VarName("$PUCSORG");
            Add(70, 0);

            // 
            VarName("$PUCSXDIR");
            Add(70, 0);

            // 
            VarName("$PUCSYDIR");
            Add(70, 0);

            // 
            VarName("$QTEXTMODE");
            Add(70, 0);

            // 
            VarName("$REGENMODE");
            Add(70, 0);

            // 
            VarName("$SHADEDGE");
            Add(70, 0);

            // 
            VarName("$SHADEDIF");
            Add(70, 0);

            // 
            VarName("$SKETCHINC");
            Add(70, 0);

            // 
            VarName("$SKPOLY");
            Add(70, 0);

            // 
            VarName("$SPLFRAME");
            Add(70, 0);

            // 
            VarName("$SPLINESEGS");
            Add(70, 0);

            // 
            VarName("$SPLINETYPE");
            Add(70, 0);

            // 
            VarName("$SURFTAB1");
            Add(70, 0);

            // 
            VarName("$SURFTAB2");
            Add(70, 0);

            // 
            VarName("$SURFTYPE");
            Add(70, 0);

            // 
            VarName("$SURFU");
            Add(70, 0);

            // 
            VarName("$SURFV");
            Add(70, 0);

            // 
            VarName("$TDCREATE");
            Add(70, 0);

            // 
            VarName("$TDINDWG");
            Add(70, 0);

            // 
            VarName("$TDUPDATE");
            Add(70, 0);

            // 
            VarName("$TDUSRTIMER");
            Add(70, 0);

            // 
            VarName("$TEXTSIZE");
            Add(70, 0);

            // 
            VarName("$TEXTSTYLE");
            Add(70, 0);

            // 
            VarName("$THICKNESS");
            Add(70, 0);

            // 
            VarName("$TILEMODE");
            Add(70, 0);

            // 
            VarName("$TRACEWID");
            Add(70, 0);

            // 
            VarName("$UCSNAME");
            Add(70, 0);

            // 
            VarName("$UCSORG");
            Add(70, 0);

            // 
            VarName("$UCSXDIR");
            Add(70, 0);

            // 
            VarName("$UCSYDIR");
            Add(70, 0);

            // 
            VarName("$UNITMODE");
            Add(70, 0);

            // 
            VarName("$USERI1");
            Add(70, 0);

            // 
            VarName("$USERI2");
            Add(70, 0);

            // 
            VarName("$USERI3");
            Add(70, 0);

            // 
            VarName("$USERI4");
            Add(70, 0);

            // 
            VarName("$USERI5");
            Add(70, 0);

            // 
            VarName("$USERR1");
            Add(40, 0.0);

            // 
            VarName("$USERR2");
            Add(40, 0.0);

            // 
            VarName("$USERR3");
            Add(40, 0.0);

            // 
            VarName("$USERR4");
            Add(40, 0.0);

            // 
            VarName("$USERR5");
            Add(40, 0.0);

            // 0 = timer off, 1 = timer on
            VarName("$USRTIMER");
            Add(70, 0);

            // 
            VarName("$VISRETAIN");
            Add(70, 0);

            // 
            VarName("$WORLDVIEW");
            Add(70, 0);

            */

            // insertion base 
            VarName("$INSBASE");
            Add(10, "0.0");
            Add(20, "0.0");
            Add(30, "0.0");

            // drawing limits upper-right corner 
            VarName("$LIMMAX");
            Add(10, "1260.0");
            Add(20, "891.0");

            // drawing limits lower-left corner 
            VarName("$LIMMIN");
            Add(10, "0.0");
            Add(20, "0.0");

            // default drawing units for AutoCAD DesignCenter blocks
            /* 
            0 = Unitless;
            1 = Inches; 
            2 = Feet; 
            3 = Miles; 
            4 = Millimeters; 
            5 = Centimeters; 
            6 = Meters; 
            7 = Kilometers; 
            8 = Microinches; 
            9 = Mils; 
            10 = Yards; 
            11 = Angstroms; 
            12 = Nanometers; 
            13 = Microns; 
            14 = Decimeters; 
            15 = Decameters; 
            16 = Hectometers; 
            17 = Gigameters; 
            18 = Astronomical units; 
            19 = Light years; 
            20 = Parsecs
            */

            // units format for coordinates and distances
            VarName("$INSUNITS");
            Add(70, (int)4);

            // units format for coordinates and distances
            VarName("$LUNITS");
            Add(70, (int)2);

            // units precision for coordinates and distances
            VarName("$LUPREC");
            Add(70, (int)4);

            // sets drawing units
            VarName("$MEASUREMENT");
            Add(70, (int)1); // 0 = English; 1 = Metric

            // VPORT header variables

            /*
            // Fast zoom enabled if nonzero
            VarName("$FASTZOOM");
            Add(70, 0);

            // Grid mode on if nonzero
            VarName("$GRIDMODE");
            Add(70, 0);

            //  Grid X and Y spacing
            VarName("$GRIDUNIT");
            Add(10, 30.0);
            Add(20, 30.0);

            // Snap grid rotation angle
            VarName("$SNAPANG");
            Add(50, 0.0);

            // Snap/grid base point (in UCS) 
            VarName("$SNAPBASE");
            Add(10, 0.0);
            Add(20, 0.0);

            // Isometric plane: 0 = left, 1 = top, 2 = right
            VarName("$SNAPISOPAIR");
            Add(70, 0);

            // Snap mode on if nonzero
            VarName("$SNAPMODE");
            Add(70, 0);

            // Snap style: 0 = standard, 1 = isometric
            VarName("$SNAPSTYLE");
            Add(70, 0);

            // $SNAPUNIT
            VarName("$SNAPUNIT");
            Add(10, 15.0);
            Add(20, 15.0);

            // XY center of current view on screen
            VarName("$VIEWCTR");
            Add(10, 0.0);
            Add(20, 0.0);

            // Viewing direction (direction from target, in WCS)
            VarName("$VIEWDIR");
            Add(10, 0.0);
            Add(20, 0.0);
            Add(30, 0.0);

            // Height of view
            VarName("$VIEWSIZE");
            Add(40, 0.0);
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextAvailableHandle"></param>
        private void End(int nextAvailableHandle)
        {
            if (Version > DxfAcadVer.AC1009)
            {
                VarName("$HANDSEED");
                Add(5, nextAvailableHandle.ToDxfHandle());
            }

            Add(0, DxfCodeName.EndSec);
        }
    }
}
