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
using System.Globalization;
using System.IO;
using System.Text;
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

namespace netDxf.IO
{
    /// <summary>
    /// Low level dxf reader
    /// </summary>
    internal sealed class DxfReader
    {
        #region private fields

        private bool isBinary;

        private ICodeValueReader chunk;
        private DxfDocument doc;

        // here we will store strings already decoded <string: original, string: decoded>
        private Dictionary<string, string> decodedStrings;

        // blocks records <string: name, string: blockRecord>
        private Dictionary<string, BlockRecord> blockRecords;

        // entities, they will be processed at the end <Entity: entity, string: owner handle>.
        private Dictionary<EntityObject, string> entityList;

        // Viewports, they will be processed at the end <Entity: Viewport, string: clipping boundary handle>.
        private Dictionary<Viewport, string> viewports;

        private Dictionary<Hatch, List<HatchBoundaryPath>> hatchToPaths;
        // HatchBoundaryPaths, they will be processed at the end <Entity: HatchBoundaryPath, string: entity contourn handle>.
        private Dictionary<HatchBoundaryPath, List<string>> hatchContourns;

        // in nested blocks (blocks that contains Insert entities) the block definition might be defined AFTER the block that references them
        // temporarily these variables will store information to post process the nested block list
        private Dictionary<Insert, string> nestedInserts;
        private Dictionary<Dimension, string> nestedDimensions;

        // the named dictionary is the only one we are interested, it is always the first that appears in the section
        // It consists solely of associated pairs of entry names and hard ownership pointer references to the associated object.
        private DictionaryObject namedDictionary;
        // <string: dictionary handle, DictionaryObject: dictionary>
        private Dictionary<string, DictionaryObject> dictionaries;

        private Dictionary<string, ImageDefinitionReactor> imageDefReactors;

        // variables for post-processing
        private Dictionary<Leader, string> leaderAnnotation;
        private Dictionary<Block, List<EntityObject>> blockEntities;
        private Dictionary<Group, List<string>> groupEntities;

        // the order of each table group in the tables section may vary
        private Dictionary<DimensionStyle, string[]> dimStyleToHandles;

        // the MLineStyles are defined, in the objects section, AFTER the MLine that references them,
        // temporarily this variables will store information to post process the MLine list
        private Dictionary<MLine, string> mLineToStyleNames;

        // the ImageDefinition are defined, in the objects section AFTER the Image that references them,
        // temporarily these variables will store information to post process the Image list
        private Dictionary<Image, string> imgToImgDefHandles;
        private Dictionary<string, ImageDefinition> imgDefHandles;

        // the UnderlayDefinitions are defined, in the objects section, AFTER the Underlay entity that references them,
        // temporarily this variables will store information to post process the Underlay list
        private Dictionary<Underlay, string> underlayToDefinitionHandles;
        private Dictionary<string, UnderlayDefinition> underlayDefHandles;

        // temporarily this dictionary will store information to check any possible errors on model and paper space layouts
        private Dictionary<string, BlockRecord> blockRecordPointerToLayout;
        private List<Layout> orphanLayouts;

        #endregion

        #region constructors

        #endregion

        #region public methods

        /// <summary>
        /// Reads the whole stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public DxfDocument Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "The stream cannot be null");

            string dwgcodepage = CheckHeaderVariable(stream, HeaderVariableCode.DwgCodePage, out this.isBinary);
            
            try
            {
                if (this.isBinary)
                {
                    Encoding encoding;
                    DxfVersion version = DxfDocument.CheckDxfFileVersion(stream, out this.isBinary);
                    if (version >= DxfVersion.AutoCad2007)
                        encoding = Encoding.UTF8; // the strings in a dxf binary file seems to be stored as UTF8 even if the file looks like ANSI
                    else
                    {
                        if (string.IsNullOrEmpty(dwgcodepage))
                            encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage); // use the default windows code page, if unable to read the code page header variable.
                        else
                        {
                            int codepage;
                            if (!int.TryParse(dwgcodepage.Split('_')[1], out codepage))
                                encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage); // use the default windows code page, if unable to read the code page header variable.
                            else
                                encoding = Encoding.GetEncoding(codepage);
                        }
                    }
                    this.chunk = new BinaryCodeValueReader(new BinaryReader(stream), encoding);
                }                
                else
                {
                    Encoding encoding;
                    Encoding encodingType = EncodingType.GetType(stream);
                    bool isUnicode = (encodingType.EncodingName == Encoding.UTF8.EncodingName) ||
                                     (encodingType.EncodingName == Encoding.BigEndianUnicode.EncodingName) ||
                                     (encodingType.EncodingName == Encoding.Unicode.EncodingName);

                    if (isUnicode)
                        encoding = Encoding.UTF8;
                    else
                    {
                        // if the file is not UTF-8 use the code page provided by the dxf file
                        if (string.IsNullOrEmpty(dwgcodepage))
                            encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage); // use the default windows code page, if unable to read the code page header variable.
                        else
                        {
                            int codepage;
                            if (!int.TryParse(dwgcodepage.Split('_')[1], out codepage))
                                encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage); // use the default windows code page, if unable to read the code page header variable.
                            else
                                encoding = Encoding.GetEncoding(codepage); // use the default windows code page, if unable to read the code page header variable.
                        }    
                    }
                    this.chunk = new TextCodeValueReader(new StreamReader(stream, encoding, true));
                }
            }
            catch (Exception ex)
            {
                throw new DxfException("Unknown error opening the reader.", ex);
            }

            this.doc = new DxfDocument(new HeaderVariables(), false);

            this.entityList = new Dictionary<EntityObject, string>();
            this.viewports = new Dictionary<Viewport, string>();
            this.hatchToPaths = new Dictionary<Hatch, List<HatchBoundaryPath>>();
            this.hatchContourns = new Dictionary<HatchBoundaryPath, List<string>>();
            this.decodedStrings = new Dictionary<string, string>();
            this.leaderAnnotation = new Dictionary<Leader, string>();

            // blocks
            this.nestedInserts = new Dictionary<Insert, string>();
            this.nestedDimensions = new Dictionary<Dimension, string>();
            this.blockRecords = new Dictionary<string, BlockRecord>(StringComparer.OrdinalIgnoreCase);
            this.blockEntities = new Dictionary<Block, List<EntityObject>>();

            // objects
            this.dictionaries = new Dictionary<string, DictionaryObject>(StringComparer.OrdinalIgnoreCase);
            this.groupEntities = new Dictionary<Group, List<string>>();
            this.dimStyleToHandles = new Dictionary<DimensionStyle, string[]>();
            this.imageDefReactors = new Dictionary<string, ImageDefinitionReactor>(StringComparer.OrdinalIgnoreCase);
            this.imgDefHandles = new Dictionary<string, ImageDefinition>(StringComparer.OrdinalIgnoreCase);
            this.imgToImgDefHandles = new Dictionary<Image, string>();
            this.mLineToStyleNames = new Dictionary<MLine, string>();
            this.underlayToDefinitionHandles = new Dictionary<Underlay, string>();
            this.underlayDefHandles = new Dictionary<string, UnderlayDefinition>();

            // for layouts errors workarounds
            this.blockRecordPointerToLayout = new Dictionary<string, BlockRecord>(StringComparer.OrdinalIgnoreCase);
            this.orphanLayouts = new List<Layout>();

            this.chunk.Next();

            // read the comments at the head of the file, any other comments will be ignored
            // they sometimes hold information about the program that has generated the dxf
            // binary files do not contain any comments
            this.doc.Comments.Clear();
            while (this.chunk.Code == 999)
            {
                this.doc.Comments.Add(this.chunk.ReadString());
                this.chunk.Next();
            }

            while (this.chunk.ReadString() != DxfObjectCode.EndOfFile)
            {
                if (this.chunk.ReadString() == DxfObjectCode.BeginSection)
                {
                    this.chunk.Next();
                    switch (this.chunk.ReadString())
                    {
                        case DxfObjectCode.HeaderSection:
                            this.ReadHeader();
                            break;
                        case DxfObjectCode.ClassesSection:
                            this.ReadClasses();
                            break;
                        case DxfObjectCode.TablesSection:
                            this.ReadTables();
                            break;
                        case DxfObjectCode.BlocksSection:
                            this.ReadBlocks();
                            break;
                        case DxfObjectCode.EntitiesSection:
                            this.ReadEntities();
                            break;
                        case DxfObjectCode.ObjectsSection:
                            this.ReadObjects();
                            break;
                        case DxfObjectCode.ThumbnailImageSection:
                            this.ReadThumbnailImage();
                            break;
                        case DxfObjectCode.AcdsDataSection:
                            this.ReadAcdsData();
                            break;
                        default:
                            throw new InvalidDxfSectionException(this.chunk.ReadString(), string.Format("Unknown section {0}.", this.chunk.ReadString()));
                    }
                }
                this.chunk.Next();
            }
            stream.Position = 0;

            // post process the dimension style list to assign the variables DIMTXSTY, DIMBLK, DIMBLK1, DIMBLK2, DIMLTYPE, DILTEX1, and DIMLTEX2
            foreach (KeyValuePair<DimensionStyle, string[]> pair in this.dimStyleToHandles)
            {
                DimensionStyle defaultDim = DimensionStyle.Default;

                BlockRecord record;

                record = this.doc.GetObjectByHandle(pair.Value[0]) as BlockRecord;
                pair.Key.DIMBLK = record == null ? null : this.doc.Blocks[record.Name];

                record = this.doc.GetObjectByHandle(pair.Value[1]) as BlockRecord;
                pair.Key.DIMBLK1 = record == null ? null : this.doc.Blocks[record.Name];

                record = this.doc.GetObjectByHandle(pair.Value[2]) as BlockRecord;
                pair.Key.DIMBLK2 = record == null ? null : this.doc.Blocks[record.Name];

                record = this.doc.GetObjectByHandle(pair.Value[3]) as BlockRecord;
                pair.Key.DIMLDRBLK = record == null ? null : this.doc.Blocks[record.Name];

                TextStyle txtStyle;

                txtStyle = this.doc.GetObjectByHandle(pair.Value[4]) as TextStyle;
                pair.Key.DIMTXSTY = txtStyle == null ? this.doc.TextStyles[defaultDim.DIMTXSTY.Name] : this.doc.TextStyles[txtStyle.Name];

                LineType ltype;

                ltype = this.doc.GetObjectByHandle(pair.Value[5]) as LineType;
                pair.Key.DIMLTYPE = ltype == null ? this.doc.LineTypes[defaultDim.DIMLTYPE.Name] : this.doc.LineTypes[ltype.Name];

                ltype = this.doc.GetObjectByHandle(pair.Value[6]) as LineType;
                pair.Key.DIMLTEX1 = ltype == null ? this.doc.LineTypes[defaultDim.DIMLTEX1.Name] : this.doc.LineTypes[ltype.Name];

                ltype = this.doc.GetObjectByHandle(pair.Value[7]) as LineType;
                pair.Key.DIMLTEX2 = ltype == null ? this.doc.LineTypes[defaultDim.DIMLTEX2.Name] : this.doc.LineTypes[ltype.Name];
            }

            // post process the image list to assign their image definitions.
            foreach (KeyValuePair<Image, string> pair in this.imgToImgDefHandles)
            {
                Image image = pair.Key;
                image.Definition = this.imgDefHandles[pair.Value];
                image.Definition.Reactors.Add(image.Handle, this.imageDefReactors[image.Handle]);

                // we still need to set the definitive image size, now that we know all units involved
                double factor = UnitHelper.ConversionFactor(this.doc.DrawingVariables.InsUnits, this.doc.RasterVariables.Units);
                image.Width *= factor;
                image.Height *= factor;
            }

            // post process the underlay definition list to assign their image definitions.
            foreach (KeyValuePair<Underlay, string> pair in this.underlayToDefinitionHandles)
            {
                Underlay underlay = pair.Key;
                underlay.Definition = this.underlayDefHandles[pair.Value];
            }

            // post process the MLines to assign their MLineStyle
            foreach (KeyValuePair<MLine, string> pair in this.mLineToStyleNames)
            {
                MLine mline = pair.Key;
                mline.Style = this.GetMLineStyle(pair.Value);
            }

            // post process the entities of the blocks
            foreach (KeyValuePair<Block, List<EntityObject>> pair in this.blockEntities)
            {
                Block block = pair.Key;
                foreach (EntityObject entity in pair.Value)
                {
                    // now that we have all information required by the block entities we can add them to the document
                    // entities like MLine and Image require information that is defined AFTER the block section,
                    // this is the case of the MLineStyle and ImageDefinition that are described in the objects section
                    block.Entities.Add(entity);
                }
            }

            // add the dxf entities to the document
            foreach (KeyValuePair<EntityObject, string> pair in this.entityList)
            {
                Layout layout;
                Block block;
                if (pair.Value == null)
                {
                    // the Model layout is the default in case the entity has not one defined
                    layout = this.doc.Layouts[Layout.ModelSpaceName];
                    block = layout.AssociatedBlock;
                }
                else
                {
                    block = this.GetBlock(((BlockRecord) this.doc.GetObjectByHandle(pair.Value)).Name);
                    layout = block.Record.Layout;
                }

                // the viewport with id 1 is stored directly in the layout since it has no graphical representation
                Viewport viewport = pair.Key as Viewport;
                if (viewport != null)
                {
                    if (viewport.Id == 1)
                    {
                        // the base layout viewport has always id = 1 and we will not add it to the entities list of the document.
                        // this viewport has no graphical representation, it is the view of the paper space layout itself and it does not show the model.
                        layout.Viewport = viewport;
                        layout.Viewport.Owner = block;
                    }
                    else
                    {
                        this.doc.ActiveLayout = layout.Name;
                        this.doc.AddEntity(pair.Key, false, false);
                    }
                }
                else
                {
                    // apply the units scale to the insertion scale (this is for not nested blocks)
                    Insert insert = pair.Key as Insert;
                    if (insert != null)
                    {
                        double scale = UnitHelper.ConversionFactor(this.doc.DrawingVariables.InsUnits, insert.Block.Record.Units);
                        insert.Scale *= scale;
                    }
                    this.doc.ActiveLayout = layout.Name;
                    this.doc.AddEntity(pair.Key, false, false);
                }
            }

            // assign a handle to the default layout viewports
            foreach (Layout layout in this.doc.Layouts)
            {
                if (layout.Viewport == null) continue;

                if (string.IsNullOrEmpty(layout.Viewport.Handle))
                    this.doc.NumHandles = layout.Viewport.AsignHandle(this.doc.NumHandles);
            }

            // post process viewports clipping boundaries
            foreach (KeyValuePair<Viewport, string> pair in this.viewports)
            {
                EntityObject entity = this.doc.GetObjectByHandle(pair.Value) as EntityObject;
                if (entity != null )
                    pair.Key.ClippingBoundary = entity;
            }

            // post process the hatch boundary paths
            foreach (KeyValuePair<Hatch, List<HatchBoundaryPath>> pair in this.hatchToPaths)
            {
                Hatch hatch = pair.Key;
                foreach (HatchBoundaryPath path in pair.Value)
                {
                    List<string> entities = this.hatchContourns[path];
                    foreach (string handle in entities)
                    {
                        EntityObject entity = this.doc.GetObjectByHandle(handle) as EntityObject;
                        if (entity != null)
                            path.AddContour(entity);
                    }
                    hatch.BoundaryPaths.Add(path);
                }
            }

            // post process leader annotations
            foreach (KeyValuePair<Leader, string> pair in this.leaderAnnotation)
            {
                EntityObject entity = this.doc.GetObjectByHandle(pair.Value) as EntityObject;
                if (entity != null)
                {
                    pair.Key.Annotation = entity;
                    pair.Key.Update();
                }
                    
            }

            // post process group entities
            foreach (KeyValuePair<Group, List<string>> pair in this.groupEntities)
            {
                foreach (string handle in pair.Value)
                {
                    EntityObject entity = this.doc.GetObjectByHandle(handle) as EntityObject;
                    if (entity != null)
                        pair.Key.Entities.Add(entity);
                }
            }

            // to play safe we will add the default table objects to the document in case they do not exist,
            // if they already present nothing is overridden
            // add default layer
            this.doc.Layers.Add(Layer.Default);

            // add default line types
            this.doc.LineTypes.Add(LineType.ByLayer);
            this.doc.LineTypes.Add(LineType.ByBlock);
            this.doc.LineTypes.Add(LineType.Continuous);

            // add default text style
            this.doc.TextStyles.Add(TextStyle.Default);

            // add default application registry
            this.doc.ApplicationRegistries.Add(ApplicationRegistry.Default);

            // add default dimension style
            this.doc.DimensionStyles.Add(DimensionStyle.Default);

            // add default MLine style
            this.doc.MlineStyles.Add(MLineStyle.Default);

            this.doc.ActiveLayout = Layout.ModelSpaceName;

            return this.doc;
        }

        public static bool IsBinary(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] sentinel = reader.ReadBytes(22);
            StringBuilder sb = new StringBuilder(18);
            for (int i = 0; i < 18; i++)
                sb.Append((char) sentinel[i]);

            reader.BaseStream.Position = 0;
            return sb.ToString() == "AutoCAD Binary DXF";
        }

        public static string CheckHeaderVariable(Stream stream, string headerVariable, out bool isBinary)
        {
            ICodeValueReader chunk;
            isBinary = IsBinary(stream);

            if (isBinary)
                chunk = new BinaryCodeValueReader(new BinaryReader(stream), Encoding.ASCII);
            else
                chunk = new TextCodeValueReader(new StreamReader(stream));

            chunk.Next();
            while (chunk.ReadString() != DxfObjectCode.EndOfFile)
            {
                chunk.Next();
                if (chunk.ReadString() == DxfObjectCode.HeaderSection)
                {
                    chunk.Next();
                    while (chunk.ReadString() != DxfObjectCode.EndSection)
                    {
                        string varName = chunk.ReadString();
                        chunk.Next();

                        if (varName == headerVariable)
                        {
                            // we found the variable we are looking for
                            stream.Position = 0;
                            return chunk.ReadString();
                        }

                        // some header variables have more than one entry
                        while (chunk.Code != 0 && chunk.Code != 9)
                            chunk.Next();
                    }

                    // we only need to read the header section
                    stream.Position = 0;
                    return null;
                }
            }

            stream.Position = 0;
            return null;
        }

        #endregion

        #region sections methods

        private void ReadHeader()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.HeaderSection);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                string varName = this.chunk.ReadString();
                double julian;
                this.chunk.Next();

                switch (varName)
                {
                    case HeaderVariableCode.AcadVer:
                        DxfVersion acadVer = DxfVersion.Unknown;
                        string version = this.chunk.ReadString();
                        if (StringEnum.IsStringDefined(typeof (DxfVersion), version))
                            acadVer = (DxfVersion) StringEnum.Parse(typeof (DxfVersion), version);
                        if (acadVer < DxfVersion.AutoCad2000)
                            throw new NotSupportedException("Only AutoCad2000 and higher dxf versions are supported.");
                        this.doc.DrawingVariables.AcadVer = acadVer;
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.HandleSeed:
                        string handleSeed = this.chunk.ReadString();
                        this.doc.DrawingVariables.HandleSeed = handleSeed;
                        this.doc.NumHandles = long.Parse(handleSeed, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.Angbase:
                        this.doc.DrawingVariables.Angbase = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.Angdir:
                        this.doc.DrawingVariables.Angdir = (AngleDirection) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.AttMode:
                        this.doc.DrawingVariables.AttMode = (AttMode) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.AUnits:
                        this.doc.DrawingVariables.AUnits = (AngleUnitType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.AUprec:
                        this.doc.DrawingVariables.AUprec = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CeColor:
                        this.doc.DrawingVariables.CeColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CeLtScale:
                        this.doc.DrawingVariables.CeLtScale = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CeLtype:
                        this.doc.DrawingVariables.CeLtype = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CeLweight:
                        this.doc.DrawingVariables.CeLweight = Lineweight.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CLayer:
                        this.doc.DrawingVariables.CLayer = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CMLJust:
                        this.doc.DrawingVariables.CMLJust = (MLineJustification) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CMLScale:
                        this.doc.DrawingVariables.CMLScale = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.CMLStyle:
                        string mLineStyleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!string.IsNullOrEmpty(mLineStyleName))
                            this.doc.DrawingVariables.CMLStyle = mLineStyleName;
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.DimStyle:
                        string dimStyleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!string.IsNullOrEmpty(dimStyleName))
                            this.doc.DrawingVariables.DimStyle = dimStyleName;
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TextSize:
                        double size = this.chunk.ReadDouble();
                        if (size > 0.0)
                            this.doc.DrawingVariables.TextSize = size;
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TextStyle:
                        string textStyleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!string.IsNullOrEmpty(textStyleName))
                            this.doc.DrawingVariables.TextStyle = textStyleName;
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.LastSavedBy:
                        this.doc.DrawingVariables.LastSavedBy = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.LUnits:
                        this.doc.DrawingVariables.LUnits = (LinearUnitType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.LUprec:
                        this.doc.DrawingVariables.LUprec = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.DwgCodePage:
                        this.doc.DrawingVariables.DwgCodePage = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.Extnames:
                        this.doc.DrawingVariables.Extnames = this.chunk.ReadBool();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.InsBase:
                        Vector3 pos = Vector3.Zero;
                        pos.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        pos.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        pos.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        this.doc.DrawingVariables.InsBase = pos;
                        break;
                    case HeaderVariableCode.InsUnits:
                        this.doc.DrawingVariables.InsUnits = (DrawingUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.LtScale:
                        this.doc.DrawingVariables.LtScale = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.LwDisplay:
                        this.doc.DrawingVariables.LwDisplay = this.chunk.ReadBool();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.PdMode:
                        this.doc.DrawingVariables.PdMode = (PointShape) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.PdSize:
                        this.doc.DrawingVariables.PdSize = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.PLineGen:
                        this.doc.DrawingVariables.PLineGen = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.PsLtScale:
                        this.doc.DrawingVariables.PsLtScale = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TdCreate:
                        julian = this.chunk.ReadDouble();
                        if (julian < 1721426 || julian > 5373484)
                            this.doc.DrawingVariables.TdCreate = DateTime.Now;
                        else
                            this.doc.DrawingVariables.TdCreate = DrawingTime.FromJulianCalendar(julian);
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TduCreate:
                        julian = this.chunk.ReadDouble();
                        if (julian < 1721426 || julian > 5373484)
                            this.doc.DrawingVariables.TduCreate = DateTime.Now;
                        else
                            this.doc.DrawingVariables.TduCreate = DrawingTime.FromJulianCalendar(julian);
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TdUpdate:
                        julian = this.chunk.ReadDouble();
                        if (julian < 1721426 || julian > 5373484)
                            this.doc.DrawingVariables.TdUpdate = DateTime.Now;
                        else
                            this.doc.DrawingVariables.TdUpdate = DrawingTime.FromJulianCalendar(julian);
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TduUpdate:
                        julian = this.chunk.ReadDouble();
                        if (julian < 1721426 || julian > 5373484)
                            this.doc.DrawingVariables.TduUpdate = DateTime.Now;
                        else
                            this.doc.DrawingVariables.TduUpdate = DrawingTime.FromJulianCalendar(julian);
                        this.chunk.Next();
                        break;
                    case HeaderVariableCode.TdinDwg:
                        this.doc.DrawingVariables.TdinDwg = DrawingTime.EditingTime(this.chunk.ReadDouble());
                        this.chunk.Next();
                        break;
                    default:
                        // some header variables have more than one entry
                        while (this.chunk.Code != 0 && this.chunk.Code != 9)
                            this.chunk.Next();
                        break;
                }
            }
        }

        private void ReadClasses()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.ClassesSection);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                //read the class
                do
                    this.chunk.Next(); while (this.chunk.Code != 0);
            }
        }

        private void ReadTables()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.TablesSection);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                this.ReadTable();
            }

            // check if all table collections has been created
            if (this.doc.ApplicationRegistries == null) this.doc.ApplicationRegistries = new ApplicationRegistries(this.doc);
            if (this.doc.Blocks == null) this.doc.Blocks = new BlockRecords(this.doc);
            if (this.doc.DimensionStyles == null) this.doc.DimensionStyles = new DimensionStyles(this.doc);
            if (this.doc.Layers == null) this.doc.Layers = new Layers(this.doc);
            if (this.doc.LineTypes == null) this.doc.LineTypes = new LineTypes(this.doc);
            if (this.doc.TextStyles == null) this.doc.TextStyles = new TextStyles(this.doc);
            if (this.doc.UCSs == null) this.doc.UCSs = new UCSs(this.doc);
            if (this.doc.Views == null) this.doc.Views = new Views(this.doc);
            if (this.doc.VPorts == null) this.doc.VPorts = new VPorts(this.doc);
        }

        private void ReadBlocks()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.BlocksSection);

            // the blocks list will be added to the document after reading the blocks section to handle possible nested insert cases.
            Dictionary<string, Block> blocks = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                switch (this.chunk.ReadString())
                {
                    case DxfObjectCode.BeginBlock:
                        Block block = this.ReadBlock();
                        blocks.Add(block.Name, block);
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // post process the possible nested blocks,
            // in nested blocks (blocks that contains Insert entities) the block definition might be defined AFTER the insert that references them
            foreach (KeyValuePair<Insert, string> pair in this.nestedInserts)
            {
                Insert insert = pair.Key;
                insert.Block = blocks[pair.Value];
                foreach (Attribute att in insert.Attributes)
                {
                    // attribute definitions might be null if an INSERT entity attribute has not been defined in the block
                    AttributeDefinition attDef;
                    if (insert.Block.AttributeDefinitions.TryGetValue(att.Tag, out attDef))
                        att.Definition = attDef;
                    att.Owner = insert;
                }
                // in the case the insert belongs to a *PaperSpace# the insert owner has not been assigned yet,
                // in this case the owner units are the document units and will be assigned at the end with the rest of the entities 
                if (insert.Owner != null)
                {
                    // apply the units scale to the insertion scale (this is for nested blocks)
                    double scale = UnitHelper.ConversionFactor(insert.Owner.Record.Units, insert.Block.Record.Units);
                    insert.Scale *= scale;
                }
            }
            foreach (KeyValuePair<Dimension, string> pair in this.nestedDimensions)
            {
                Dimension dim = pair.Key;
                dim.Block = blocks[pair.Value];
            }

            // add the blocks to the document
            // the block entities will not be added to the document at this point
            // entities like MLine and Image require information that is defined AFTER the block section,
            // this is the case of the MLineStyle and ImageDefinition that are described in the objects section
            foreach (Block block in blocks.Values)
                this.doc.Blocks.Add(block, false);
        }

        private void ReadEntities()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.EntitiesSection);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
                this.ReadEntity(false);
        }

        private void ReadObjects()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.ObjectsSection);

            this.chunk.Next();
            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                switch (this.chunk.ReadString())
                {
                    case DxfObjectCode.Dictionary:
                        DictionaryObject dictionary = this.ReadDictionary();
                        this.dictionaries.Add(dictionary.Handle, dictionary);
                        // the named dictionary is always the first in the objects section
                        if (this.namedDictionary == null)
                        {
                            this.CreateObjectCollection(dictionary);
                            this.namedDictionary = dictionary;
                        }
                        break;
                    case DxfObjectCode.RasterVariables:
                        this.doc.RasterVariables = this.ReadRasterVariables();
                        break;
                    case DxfObjectCode.ImageDef:
                        ImageDefinition imageDefinition = this.ReadImageDefinition();
                        this.doc.ImageDefinitions.Add(imageDefinition, false);
                        break;
                    case DxfObjectCode.ImageDefReactor:
                        ImageDefinitionReactor reactor = this.ReadImageDefReactor();
                        if (!this.imageDefReactors.ContainsKey(reactor.ImageHandle))
                            this.imageDefReactors.Add(reactor.ImageHandle, reactor);
                        break;
                    case DxfObjectCode.MLineStyle:
                        MLineStyle style = this.ReadMLineStyle();
                        this.doc.MlineStyles.Add(style, false);
                        break;
                    case DxfObjectCode.Group:
                        Group group = this.ReadGroup();
                        this.doc.Groups.Add(group, false);
                        break;
                    case DxfObjectCode.Layout:
                        Layout layout = this.ReadLayout();
                        if (layout.AssociatedBlock == null)
                            this.orphanLayouts.Add(layout);
                        else
                            this.doc.Layouts.Add(layout, false);
                        break;
                    case DxfObjectCode.UnderlayDgnDefinition:
                        UnderlayDgnDefinition underlayDgnDef = (UnderlayDgnDefinition) this.ReadUnderlayDefinition(UnderlayType.DGN);
                        this.doc.UnderlayDgnDefinitions.Add(underlayDgnDef, false);
                        break;
                    case DxfObjectCode.UnderlayDwfDefinition:
                        UnderlayDwfDefinition underlayDwfDef = (UnderlayDwfDefinition)this.ReadUnderlayDefinition(UnderlayType.DWF);
                        this.doc.UnderlayDwfDefinitions.Add(underlayDwfDef, false);
                        break;
                    case DxfObjectCode.UnderlayPdfDefinition:
                        UnderlayPdfDefinition underlayPdfDef = (UnderlayPdfDefinition)this.ReadUnderlayDefinition(UnderlayType.PDF);
                        this.doc.UnderlayPdfDefinitions.Add(underlayPdfDef, false);
                        break;
                    default:
                        do
                            this.chunk.Next();
                        while (this.chunk.Code != 0);
                        break;
                }
            }

            // this will try to fix problems with layouts and model/paper space blocks
            // nothing of this is be necessary in a well formed dxf
            this.RelinkOrphanLayouts();
            
            // raster variables
            if (this.doc.RasterVariables == null)
                this.doc.RasterVariables = new RasterVariables();
        }

        private void RelinkOrphanLayouts()
        {
            // add any additional layouts corresponding to the PaperSpace block records not referenced by any layout
            // these are fixes for possible errors with layouts and their associated blocks. This should never happen.
            foreach (BlockRecord r in this.blockRecordPointerToLayout.Values)
            {
                Layout layout = null;

                // the *ModelSpace block must be linked to a layout called "Model"
                if (string.Equals(r.Name, Block.DefaultModelSpaceName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Layout l in this.orphanLayouts)
                    {
                        if (string.Equals(l.Name, Layout.ModelSpaceName, StringComparison.OrdinalIgnoreCase))
                        {
                            layout = l;
                            break;
                        }
                    }

                    if (layout == null)
                    {
                        // we will create a "Model" layout since we haven't found any
                        layout = Layout.ModelSpace;
                        this.doc.Layouts.Add(layout);
                        continue;
                    }
                    // we have a suitable layout
                    layout.AssociatedBlock = this.doc.Blocks[r.Name];
                    this.orphanLayouts.Remove(layout);
                    this.doc.Layouts.Add(layout);
                    continue;
                }

                // the *PaperSpace block cannot be linked with a layout called "Model"
                // check if we have any orphan layouts
                if (this.orphanLayouts.Count > 0)
                {
                    foreach (Layout l in this.orphanLayouts)
                    {
                        // find the first occurrence of a layout not named "Model"
                        if (!string.Equals(l.Name, Layout.ModelSpaceName, StringComparison.OrdinalIgnoreCase))
                        {
                            layout = l;
                            break;
                        }
                    }
                    if (layout != null)
                    {
                        // we have a suitable layout
                        layout = this.orphanLayouts[0];
                        layout.AssociatedBlock = this.doc.Blocks[r.Name];
                        this.doc.Layouts.Add(layout);
                        this.orphanLayouts.Remove(layout);
                        continue;
                    }
                }

                // create a new Layout in case there are now more suitable orphans
                short counter = 1;
                string layoutName = "Layout" + 1;

                while (this.doc.Layouts.Contains(layoutName))
                {
                    counter += 1;
                    layoutName = "Layout" + counter;
                }
                layout = new Layout(layoutName)
                {
                    TabOrder = (short)(this.doc.Layouts.Count + 1),
                    AssociatedBlock = this.doc.Blocks[r.Name]
                };

                this.doc.Layouts.Add(layout);
            }

            // if there are still orphan layouts add them to the list, it will create an associate block for them
            foreach (Layout orphan in this.orphanLayouts)
                this.doc.Layouts.Add(orphan, false);

            // add ModelSpace layout if it does not exist
            this.doc.Layouts.Add(Layout.ModelSpace);
        }

        private void ReadThumbnailImage()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.ThumbnailImageSection);

            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                //read the thumbnail image
                do
                    this.chunk.Next();
                while (this.chunk.Code != 0);
            }
        }

        private void ReadAcdsData()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.AcdsDataSection);

            while (this.chunk.ReadString() != DxfObjectCode.EndSection)
            {
                //read the ACDSSCHEMA and ACDSRECORD, multiple entries
                do
                    this.chunk.Next();
                while (this.chunk.Code != 0);
            }
        }

        #endregion

        #region table methods

        private void CreateTableCollection(string name, string handle)
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.Table);

            // number of entries in table, this code is optional
            short numberOfEntries = 0;
            while (this.chunk.Code != 0)
            {
                if (this.chunk.Code == 70) numberOfEntries = this.chunk.ReadShort();
                this.chunk.Next();
            }

            switch (name)
            {
                case DxfObjectCode.ApplicationIDTable:
                    this.doc.ApplicationRegistries = new ApplicationRegistries(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.BlockRecordTable:
                    this.doc.Blocks = new BlockRecords(this.doc, numberOfEntries, handle);
                    return;
                case DxfObjectCode.DimensionStyleTable:
                    this.doc.DimensionStyles = new DimensionStyles(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.LayerTable:
                    this.doc.Layers = new Layers(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.LineTypeTable:
                    this.doc.LineTypes = new LineTypes(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.TextStyleTable:
                    this.doc.TextStyles = new TextStyles(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.UcsTable:
                    this.doc.UCSs = new UCSs(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.ViewTable:
                    this.doc.Views = new Views(this.doc, numberOfEntries, handle);
                    break;
                case DxfObjectCode.VportTable:
                    this.doc.VPorts = new VPorts(this.doc, numberOfEntries, handle);
                    break;
                default:
                    throw new DxfException(string.Format("Unknown Table name {0} at position {1}", name, this.chunk.CurrentPosition));
            }
        }

        private void ReadTable()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.Table);

            string handle = null;
            this.chunk.Next();
            string tableName = this.chunk.ReadString();
            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 330:
                        string owner = this.chunk.ReadString();
                        // owner should be always, 0 handle of the document.
                        Debug.Assert(owner == "0");
                        this.chunk.Next();
                        break;
                    case 102:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;
                    case 100:
                        Debug.Assert(this.chunk.ReadString() == SubclassMarker.Table);
                        this.CreateTableCollection(tableName, handle);
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // read table entries
            while (this.chunk.ReadString() != DxfObjectCode.EndTable)
            {
                this.ReadTableEntry(); 
            }

            this.chunk.Next();
        }

        private void ReadTableEntry()
        {
            string dxfCode = this.chunk.ReadString();
            // only the first *Active VPort is supported, this one describes the current document view.
            VPort active = null;
            string handle = null;

            while (this.chunk.ReadString() != DxfObjectCode.EndTable)
            {
                // table entry common codes
                while (this.chunk.Code != 100)
                {
                    switch (this.chunk.Code)
                    {
                        case 5:
                            handle = this.chunk.ReadString();
                            this.chunk.Next();
                            break;
                        case 105:
                            // this handle code is specific of dimension styles
                            handle = this.chunk.ReadString();
                            this.chunk.Next();
                            break;
                        case 330:
                            //string owner = this.chunk.ReadString(); // owner should be always, the handle of the list to which the entry belongs.
                            this.chunk.Next();
                            break;
                        case 102:
                            this.ReadExtensionDictionaryGroup();
                            this.chunk.Next();
                            break;
                        default:
                            this.chunk.Next();
                            break;
                    }
                }

                this.chunk.Next();

                switch (dxfCode)
                {
                    case DxfObjectCode.ApplicationIDTable:
                        ApplicationRegistry appReg = this.ReadApplicationId();
                        if (appReg != null)
                        {
                            appReg.Handle = handle;
                            this.doc.ApplicationRegistries.Add(appReg, false);
                        }
                        break;
                    case DxfObjectCode.BlockRecordTable:
                        BlockRecord record = this.ReadBlockRecord();
                        if (record != null)
                        {
                            record.Handle = handle;
                            this.blockRecords.Add(record.Name, record);
                        }
                        break;
                    case DxfObjectCode.DimensionStyleTable:
                        DimensionStyle dimStyle = this.ReadDimensionStyle();
                        if (dimStyle != null)
                        {
                            dimStyle.Handle = handle;
                            this.doc.DimensionStyles.Add(dimStyle, false);
                        }
                        break;
                    case DxfObjectCode.LayerTable:
                        Layer layer = this.ReadLayer();
                        if (layer != null)
                        {
                            layer.Handle = handle;
                            this.doc.Layers.Add(layer, false);
                        }
                        break;
                    case DxfObjectCode.LineTypeTable:
                        LineType lineType = this.ReadLineType();
                        if (lineType != null)
                        {
                            lineType.Handle = handle;
                            this.doc.LineTypes.Add(lineType, false);
                        }
                        break;
                    case DxfObjectCode.TextStyleTable:
                        TextStyle style = this.ReadTextStyle();
                        if (style != null)
                        {
                            style.Handle = handle;
                            this.doc.TextStyles.Add(style, false);
                        }
                        break;
                    case DxfObjectCode.UcsTable:
                        UCS ucs = this.ReadUCS();
                        if (ucs != null)
                        {
                            ucs.Handle = handle;
                            this.doc.UCSs.Add(ucs, false);
                        }
                        break;
                    case DxfObjectCode.ViewTable:
                        this.ReadView();
                        //this.doc.Views.Add((View) entry);
                        break;
                    case DxfObjectCode.VportTable:
                        VPort vport = this.ReadVPort();
                        // only the first *Active VPort will be read, no multiview support.
                        if (vport != null && active == null)
                        {
                            active = this.doc.Viewport;
                            active.Handle = handle;
                            active.ViewCenter = vport.ViewCenter;
                            active.SnapBasePoint = vport.SnapBasePoint;
                            active.SnapSpacing = vport.SnapSpacing;
                            active.GridSpacing = vport.GridSpacing;
                            active.ViewDirection = vport.ViewDirection;
                            active.ViewTarget = vport.ViewTarget;
                            active.ViewHeight = vport.ViewHeight;
                            active.ViewAspectRatio = vport.ViewAspectRatio;
                            active.ShowGrid = vport.ShowGrid;
                            active.SnapMode = vport.SnapMode;
                        }
                        break;
                    default:
                        this.ReadUnkownTableEntry();
                        return;
                }
            }
        }

        private ApplicationRegistry ReadApplicationId()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.ApplicationId);

            string appId = string.Empty;
            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                }
                this.chunk.Next();
            }

            if (string.IsNullOrEmpty(appId) || !TableObject.IsValidName(appId)) return null;

            return new ApplicationRegistry(appId, false);
        }

        private BlockRecord ReadBlockRecord()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.BlockRecord);

            string name = null;
            DrawingUnits units = DrawingUnits.Unitless;
            bool allowExploding = true;
            bool scaleUniformly = false;
            List<XData> xData = new List<XData>();
            XData designCenterData = null;
            string pointerToLayout = null;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        units = (DrawingUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 280:
                        allowExploding = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 281:
                        scaleUniformly = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 340:
                        pointerToLayout = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        if (data.ApplicationRegistry.Name.Equals(ApplicationRegistry.DefaultName, StringComparison.OrdinalIgnoreCase))
                            designCenterData = data;
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            if (string.IsNullOrEmpty(name)) return null;

            // we need to check for generated blocks by dimensions, even if the dimension was deleted the block might persist in the drawing.
            this.CheckDimBlockName(name);

            // here is where dxf versions prior to AutoCad2007 stores the block units
            if (designCenterData != null)
            {
                foreach (XDataRecord data in designCenterData.XDataRecord)
                {
                    // the second 1070 code is the one that stores the block units,
                    // it will override the first 1070 that stores the Autodesk Design Center version number
                    if (data.Code == XDataCode.Int16)
                        units = (DrawingUnits)(short)data.Value;
                }
            }

            BlockRecord record = new BlockRecord(name)
            {
                Units = units,
                AllowExploding = allowExploding,
                ScaleUniformly = scaleUniformly
            };

            record.XData.AddRange(xData);

            if (!string.IsNullOrEmpty(pointerToLayout) && pointerToLayout != "0")
                this.blockRecordPointerToLayout.Add(pointerToLayout, record);

            return record;
        }

        private DimensionStyle ReadDimensionStyle()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.DimensionStyle);

            DimensionStyle defaultDim = DimensionStyle.Default;
            string name = null;

            // dimension lines
            AciColor dimclrd = defaultDim.DIMCLRD;
            string dimltype = string.Empty; // handle for post processing
            Lineweight dimlwd = defaultDim.DIMLWD;
            double dimdle = defaultDim.DIMDLE;
            double dimdli = defaultDim.DIMDLI;

            // extension lines
            AciColor dimclre = defaultDim.DIMCLRE;
            string dimltex1 = string.Empty; // handle for post processing
            string dimltex2 = string.Empty; // handle for post processing
            Lineweight dimlwe = defaultDim.DIMLWE;
            bool dimse1 = false;
            bool dimse2 = false;
            double dimexo = defaultDim.DIMEXO;
            double dimexe = defaultDim.DIMEXE;

            // symbols and arrows
            double dimasz = defaultDim.DIMASZ;
            double dimcen = defaultDim.DIMCEN;
            bool dimsah = defaultDim.DIMSAH;
            string dimblk = string.Empty; // handle for post processing
            string dimblk1 = string.Empty; // handle for post processing
            string dimblk2 = string.Empty; // handle for post processing
            string dimldrblk = string.Empty; // handle for post processing

            // fit
            double dimscale = defaultDim.DIMSCALE;
            short dimtih = defaultDim.DIMTIH;
            short dimtoh = defaultDim.DIMTOH;

            // text
            string dimtxsty = string.Empty; // handle for post processing
            AciColor dimclrt = defaultDim.DIMCLRT;
            double dimtxt = defaultDim.DIMTXT;
            short dimjust = defaultDim.DIMJUST;
            short dimtad = defaultDim.DIMTAD;
            double dimgap = defaultDim.DIMGAP;

            // primary units
            double dimlfac = defaultDim.DIMLFAC;
            short dimadec = defaultDim.DIMADEC;
            short dimdec = defaultDim.DIMDEC;
            string dimpost = defaultDim.DIMPOST;
            char dimdsep = defaultDim.DIMDSEP;
            AngleUnitType dimaunit = defaultDim.DIMAUNIT;
            LinearUnitType dimlunit = defaultDim.DIMLUNIT;
            FractionFormatType dimfrac = defaultDim.DIMFRAC;
            double dimrnd = 0.0;
            short dimzin = 0;
            short dimazin = 3;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case 3:
                        dimpost = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case 40:
                        dimscale = this.chunk.ReadDouble();
                        if (dimscale <= 0.0) dimscale = defaultDim.DIMSCALE;
                        break;
                    case 41:
                        dimasz = this.chunk.ReadDouble();
                        if (dimasz < 0.0) dimasz = defaultDim.DIMASZ;
                        break;
                    case 42:
                        dimexo = this.chunk.ReadDouble();
                        if (dimexo < 0.0) dimexo = defaultDim.DIMEXO;
                        break;
                    case 43:
                        dimdli = this.chunk.ReadDouble();
                        if (dimdli < 0.0) dimdli = defaultDim.DIMDLI;
                        break;
                    case 44:
                        dimexe = this.chunk.ReadDouble();
                        if (dimexe < 0.0) dimexe = defaultDim.DIMEXE;
                        break;
                    case 45:
                        dimrnd = this.chunk.ReadDouble();
                        if (dimrnd < 0.0) dimrnd = defaultDim.DIMEXE;
                        break;
                    case 46:
                        dimdle = this.chunk.ReadDouble();
                        if (dimdle < 0.0) dimdle = defaultDim.DIMDLE;
                        break;
                    case 73:
                        dimtih = this.chunk.ReadShort();
                        break;
                    case 74:
                        dimtoh = this.chunk.ReadShort();
                        break;
                    case 75:
                        dimse1 = this.chunk.ReadShort() != 0;
                        break;
                    case 76:
                        dimse2 = this.chunk.ReadShort() != 0;
                        break;
                    case 77:
                        dimtad = this.chunk.ReadShort();
                        break;
                    case 78:
                        dimzin = this.chunk.ReadShort();
                        break;
                    case 79:
                        dimazin = this.chunk.ReadShort();
                        break;
                    case 140:
                        dimtxt = this.chunk.ReadDouble();
                        if (dimtxt <= 0.0) dimtxt = defaultDim.DIMTXT;
                        break;
                    case 141:
                        dimcen = this.chunk.ReadDouble();
                        break;
                    case 144:
                        dimlfac = this.chunk.ReadDouble();
                        if (MathHelper.IsZero(dimlfac)) dimlfac = 1.0;
                        break;
                    case 147:
                        dimgap = this.chunk.ReadDouble();
                        if (dimgap < 0.0) dimgap = defaultDim.DIMGAP;
                        break;
                    case 173:
                        dimsah = this.chunk.ReadShort() != 0;
                        break;
                    case 176:
                        dimclrd = AciColor.FromCadIndex(this.chunk.ReadShort());
                        break;
                    case 177:
                        dimclre = AciColor.FromCadIndex(this.chunk.ReadShort());
                        break;
                    case 178:
                        dimclrt = AciColor.FromCadIndex(this.chunk.ReadShort());
                        break;
                    case 179:
                        dimadec = this.chunk.ReadShort();
                        if (dimadec < 0) dimadec = defaultDim.DIMADEC;
                        break;
                    case 271:
                        dimdec = this.chunk.ReadShort();
                        if (dimdec < 0) dimdec = defaultDim.DIMDEC;
                        break;
                    case 275:
                        dimaunit = (AngleUnitType) this.chunk.ReadShort();
                        break;
                    case 276:
                        dimfrac = (FractionFormatType) this.chunk.ReadShort();
                        break;
                    case 277:
                        dimlunit = (LinearUnitType) this.chunk.ReadShort();
                        break;
                    case 278:
                        dimdsep = (char) this.chunk.ReadShort();
                        break;
                    case 280:
                        dimjust = this.chunk.ReadShort();
                        break;
                    case 340:
                        dimtxsty = this.chunk.ReadString();
                        break;
                    case 341:
                        dimldrblk = this.chunk.ReadString();
                        break;
                    case 342:
                        dimblk = this.chunk.ReadString();
                        break;
                    case 343:
                        dimblk1 = this.chunk.ReadString();
                        break;
                    case 344:
                        dimblk2 = this.chunk.ReadString();
                        break;
                    case 345:
                        dimltype = this.chunk.ReadString();
                        break;
                    case 346:
                        dimltex1 = this.chunk.ReadString();
                        break;
                    case 347:
                        dimltex2 = this.chunk.ReadString();
                        break;
                    case 371:
                        dimlwd = Lineweight.FromCadIndex(this.chunk.ReadShort());
                        break;
                    case 372:
                        dimlwe = Lineweight.FromCadIndex(this.chunk.ReadShort());
                        break;
                }
                this.chunk.Next();
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;        

            DimensionStyle style = new DimensionStyle(name, false)
            {
                // dimension lines
                DIMCLRD = dimclrd,
                DIMLWD = dimlwd,
                DIMDLI = dimdli,
                DIMDLE = dimdle,

                // extension lines
                DIMCLRE = dimclre,
                DIMLWE = dimlwe,
                DIMSE1 = dimse1,
                DIMSE2 = dimse2,
                DIMEXO = dimexo,
                DIMEXE = dimexe,

                // symbols and arrows
                DIMASZ = dimasz,
                DIMCEN = dimcen,
                DIMSAH = dimsah,

                // fit
                DIMSCALE = dimscale,
                DIMTIH = dimtih,
                DIMTOH = dimtoh,

                // text
                DIMTXT = dimtxt,
                DIMCLRT = dimclrt,
                DIMJUST = dimjust,
                DIMTAD = dimtad,
                DIMGAP = dimgap,

                //primary units
                DIMADEC = dimadec,
                DIMDEC = dimdec,
                DIMLFAC = dimlfac,
                DIMPOST = dimpost,
                DIMDSEP = dimdsep,
                DIMAUNIT = dimaunit,
                DIMLUNIT = dimlunit,
                DIMFRAC = dimfrac,
                DIMRND = dimrnd
            };

            // store information for post processing. The blocks, text styles, and line types definitions might appear after the dimension style
            string[] handles = {dimblk, dimblk1, dimblk2, dimldrblk, dimtxsty, dimltype, dimltex1, dimltex2};
            this.dimStyleToHandles.Add(style, handles);

            // suppress feet and/or inches
            if (dimlunit == LinearUnitType.Architectural || dimlunit == LinearUnitType.Engineering)
            {
                if (dimzin == 1)
                {
                    style.SuppressZeroFeet = false;
                    style.SuppressZeroInches = false;
                }
                else if (dimzin == 2)
                {
                    style.SuppressZeroFeet = false;
                    style.SuppressZeroInches = true;
                }
                else if (dimzin == 3)
                {
                    style.SuppressZeroFeet = true;
                    style.SuppressZeroInches = false;
                }
                else
                {
                    style.SuppressZeroFeet = true;
                    style.SuppressZeroInches = true;
                }
            }

            // suppress leading and/or trailing zeros
            if (12 - dimzin <= 0)
            {
                style.SuppressLinearLeadingZeros = true;
                style.SuppressLinearTrailingZeros = true;
            }
            else if (8 - dimzin <= 0)
            {
                style.SuppressLinearLeadingZeros = false;
                style.SuppressLinearTrailingZeros = true;
            }
            else if (4 - dimzin <= 0)
            {
                style.SuppressLinearLeadingZeros = true;
                style.SuppressLinearTrailingZeros = false;
            }
            else
            {
                style.SuppressLinearLeadingZeros = false;
                style.SuppressLinearTrailingZeros = false;
            }
            
            // suppress angular leading and/or trailing zeros
            if (dimazin == 0)
            {
                style.SuppressAngularLeadingZeros = false;
                style.SuppressAngularTrailingZeros = false;
            }
            else if (dimazin == 1)
            {
                style.SuppressAngularLeadingZeros = true;
                style.SuppressAngularTrailingZeros = false;
            }
            else if (dimazin == 2)
            {
                style.SuppressAngularLeadingZeros = false;
                style.SuppressAngularTrailingZeros = true;
            }
            else if (dimazin == 3)
            {
                style.SuppressAngularLeadingZeros = true;
                style.SuppressAngularTrailingZeros = true;
            }

            return style;
        }

        private Layer ReadLayer()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.Layer);

            string name = null;
            bool isVisible = true;
            bool plot = true;
            AciColor color = AciColor.Default;
            LineType lineType = LineType.ByLayer;
            Lineweight lineweight = Lineweight.Default;
            LayerFlags flags = LayerFlags.None;
            Transparency transparency = new Transparency(0);

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (LayerFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 62:
                        short index = this.chunk.ReadShort();
                        if (index < 0)
                        {
                            isVisible = false;
                            index = Math.Abs(index);
                        }
                        if (!color.UseTrueColor)
                            color = AciColor.FromCadIndex(index);

                        this.chunk.Next();
                        break;
                    case 420: // the layer uses true color
                        color = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 6:
                        // the line type names ByLayer or ByBlock are case insensitive
                        string lineTypeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.Equals(lineTypeName, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByLayerName;
                        else if (string.Equals(lineTypeName, LineType.ByBlockName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByBlockName;
                        lineType = this.GetLineType(lineTypeName);

                        this.chunk.Next();
                        break;
                    case 290:
                        plot = this.chunk.ReadBool();
                        this.chunk.Next();
                        break;
                    case 370:
                        lineweight = Lineweight.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 1001:
                        // layer transparency is stored in XData
                        // the ApplicationRegistries might be defined after the objects that requires them,
                        // same old story this time we don't need to store the information in the object XData since it is only supported by entities.
                        if (this.chunk.ReadString() == "AcCmTransparency")
                        {
                            this.chunk.Next();
                            transparency = Transparency.FromAlphaValue(this.chunk.ReadInt());
                        }
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;

            return new Layer(name, false)
            {
                Color = color,
                LineType = lineType,
                IsVisible = isVisible,
                IsFrozen = (flags & LayerFlags.Frozen) == LayerFlags.Frozen,
                IsLocked = (flags & LayerFlags.Locked) == LayerFlags.Locked,
                Plot = plot,
                Lineweight = lineweight,
                Transparency = transparency
            };
        }

        private LineType ReadLineType()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.LineType);

            string name = null;
            string description = null;
            List<double> segments = new List<double>();

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2: // line type name is case insensitive
                        name = this.chunk.ReadString();
                        if (string.Equals(name, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                            name = LineType.ByLayerName;
                        else if (string.Equals(name, LineType.ByBlock.Name, StringComparison.OrdinalIgnoreCase))
                            name = LineType.ByBlockName;
                        name = this.DecodeEncodedNonAsciiCharacters(name);
                        break;
                    case 3: // line type description
                        description = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case 73:
                        //number of segments (not needed)
                        break;
                    case 40:
                        //length of the line type segments (not needed)
                        break;
                    case 49:
                        segments.Add(this.chunk.ReadDouble());
                        break;
                }
                this.chunk.Next();
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;

            return new LineType(name, description, false)
            {
                Segments = segments,
            };
        }

        private TextStyle ReadTextStyle()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.TextStyle);

            string name = null;
            string font = null;
            bool isVertical = false;
            bool isBackward = false;
            bool isUpsideDown = false;
            double height = 0.0f;
            double widthFactor = 0.0f;
            double obliqueAngle = 0.0f;
            string familyName = string.Empty;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 3:
                        font = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        int vertical = this.chunk.ReadShort();
                        if (vertical == 4)
                            isVertical = true;
                        this.chunk.Next();
                        break;
                    case 71:
                        int upDownBack = this.chunk.ReadShort();
                        if (upDownBack == 6)
                        {
                            isBackward = true;
                            isUpsideDown = true;
                        }
                        else if (upDownBack == 2)
                            isBackward = true;
                        else if (upDownBack == 4)
                            isUpsideDown = true;
                        this.chunk.Next();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        if (height < 0.0)
                            height = 0.0;
                        this.chunk.Next();
                        break;
                    case 41:
                        widthFactor = this.chunk.ReadDouble();
                        if (widthFactor < 0.01 || widthFactor > 100.0)
                            widthFactor = 1.0;
                        this.chunk.Next();
                        break;
                    case 42:
                        //last text height used (not applicable)
                        this.chunk.Next();
                        break;
                    case 50:
                        obliqueAngle = this.chunk.ReadDouble();
                        if (obliqueAngle < -85.0 || obliqueAngle > 85.0)
                            obliqueAngle = 0.0;
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (appId.Equals(ApplicationRegistry.DefaultName, StringComparison.OrdinalIgnoreCase))
                        {
                            this.chunk.Next();
                            if (this.chunk.Code == 1000)
                            {
                                familyName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                            }
                        }
                        else
                            this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }               
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;

            // For some true type fonts AutoCad only writes the family in the extended data and not the file name.
            // This is a workaround to get that file, and it will only works on windows it tries to find that file in the registry.
            // It only recognizes regular fonts, not bold or italic. This information is written in the extended data 1071 code (undocumented)
            if (string.IsNullOrEmpty(font))
                font = TextStyle.FontFileFromFamilyName(familyName);

            // if we still cannot find the font file from its family name discard the style
            if (string.IsNullOrEmpty(font)) return null;

            return new TextStyle(name, font, false)
            {
                Height = height,
                IsBackward = isBackward,
                IsUpsideDown = isUpsideDown,
                IsVertical = isVertical,
                ObliqueAngle = obliqueAngle,
                WidthFactor = widthFactor,
            };
        }

        private UCS ReadUCS()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.Ucs);

            string name = null;
            Vector3 origin = Vector3.Zero;
            Vector3 xDir = Vector3.UnitX;
            Vector3 yDir = Vector3.UnitY;
            double elevation = 0.0;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case 10:
                        origin.X = this.chunk.ReadDouble();
                        break;
                    case 20:
                        origin.Y = this.chunk.ReadDouble();
                        break;
                    case 30:
                        origin.Z = this.chunk.ReadDouble();
                        break;
                    case 11:
                        xDir.X = this.chunk.ReadDouble();
                        break;
                    case 21:
                        xDir.Y = this.chunk.ReadDouble();
                        break;
                    case 31:
                        xDir.Z = this.chunk.ReadDouble();
                        break;
                    case 12:
                        yDir.X = this.chunk.ReadDouble();
                        break;
                    case 22:
                        yDir.Y = this.chunk.ReadDouble();
                        break;
                    case 32:
                        yDir.Z = this.chunk.ReadDouble();
                        break;
                    case 146:
                        elevation = this.chunk.ReadDouble();
                        break;
                }

                this.chunk.Next();
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;

            return new UCS(name, origin, xDir, yDir, false)
            {
                Elevation = elevation
            };
        }

        private View ReadView()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.View);

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                this.chunk.Next();
            }

            return null;
        }

        private VPort ReadVPort()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.VPort);

            string name = null;
            Vector2 center = Vector2.Zero;
            Vector2 snapBasePoint = Vector2.Zero;
            Vector2 snapSpacing = new Vector2(0.5);
            Vector2 gridSpacing = new Vector2(10.0);
            Vector3 target = Vector3.Zero;
            Vector3 direction = Vector3.UnitZ;
            double height = 10.0;
            double ratio = 1.0;
            bool showGrid = true;
            bool snapMode = false;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case 12:
                        center.X = this.chunk.ReadDouble();
                        break;
                    case 22:
                        center.Y = this.chunk.ReadDouble();
                        break;
                    case 13:
                        snapBasePoint.X = this.chunk.ReadDouble();
                        break;
                    case 23:
                        snapBasePoint.Y = this.chunk.ReadDouble();
                        break;
                    case 14:
                        snapSpacing.X = this.chunk.ReadDouble();
                        break;
                    case 24:
                        snapSpacing.Y = this.chunk.ReadDouble();
                        break;
                    case 15:
                        gridSpacing.X = this.chunk.ReadDouble();
                        break;
                    case 25:
                        gridSpacing.Y = this.chunk.ReadDouble();
                        break;
                    case 16:
                        direction.X = this.chunk.ReadDouble();
                        break;
                    case 26:
                        direction.Y = this.chunk.ReadDouble();
                        break;
                    case 36:
                        direction.Z = this.chunk.ReadDouble();
                        break;
                    case 17:
                        target.X = this.chunk.ReadDouble();
                        break;
                    case 27:
                        target.Y = this.chunk.ReadDouble();
                        break;
                    case 37:
                        target.Z = this.chunk.ReadDouble();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        break;
                    case 41:
                        ratio = this.chunk.ReadDouble();
                        if (ratio <= 0) ratio = 1.0;
                        break;
                    case 75:
                        snapMode = this.chunk.ReadShort() != 0;
                        break;
                    case 76:
                        showGrid = this.chunk.ReadShort() != 0;
                        break;
                }

                this.chunk.Next();
            }

            if (string.IsNullOrEmpty(name) || !TableObject.IsValidName(name)) return null;

            return new VPort(name, false)
            {
                ViewCenter = center,
                SnapBasePoint = snapBasePoint,
                SnapSpacing = snapSpacing,
                GridSpacing = gridSpacing,
                ViewTarget = target,
                ViewDirection = direction,
                ViewHeight = height,
                ViewAspectRatio = ratio,
                ShowGrid = showGrid,
                SnapMode = snapMode
            };
        }

        private void ReadUnkownTableEntry()
        {
            do
                this.chunk.Next();
            while (this.chunk.Code != 0);
        }

        #endregion

        #region block methods

        private Block ReadBlock()
        {
            Debug.Assert(this.chunk.ReadString() == DxfObjectCode.BeginBlock);

            BlockRecord blockRecord;
            Layer layer = Layer.Default;
            string name = string.Empty;
            string handle = string.Empty;
            string xrefFile = string.Empty;
            BlockTypeFlags type = BlockTypeFlags.None;
            Vector3 basePoint = Vector3.Zero;
            List<EntityObject> entities = new List<EntityObject>();
            List<AttributeDefinition> attDefs = new List<AttributeDefinition>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                        xrefFile = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 8:
                        string layerName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(layerName);
                        this.chunk.Next();
                        break;
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        type = (BlockTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 10:
                        basePoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        basePoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        basePoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 3:
                        //I don't know the reason of these duplicity since code 2 also contains the block name
                        //The program EASE exports code 3 with an empty string (use it or don't use it but do NOT mix information)
                        //name = dxfPairInfo.Value;
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // read block entities
            while (this.chunk.ReadString() != DxfObjectCode.EndBlock)
            {
                EntityObject entity = this.ReadEntity(true);
                if (entity != null)
                {
                    AttributeDefinition attDef = entity as AttributeDefinition;
                    if (attDef != null)
                        attDefs.Add(attDef);
                    else
                        entities.Add(entity);
                }
            }

            // read the end block object until a new element is found
            this.chunk.Next();
            string endBlockHandle = string.Empty;
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        endBlockHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 8:
                        // the EndBlock layer and the Block layer are the same
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (!this.blockRecords.TryGetValue(name, out blockRecord))
                throw new DxfTableException(DxfObjectCode.BlockRecordTable, "The block record " + name + " is not defined");

            EndBlock end = new EndBlock
            {
                Handle = endBlockHandle,
                Owner = blockRecord,
            };

            Block block;

            if ((type & BlockTypeFlags.XRef) == BlockTypeFlags.XRef)
            {
                block = new Block(name, xrefFile)
                {
                    Handle = handle,
                    Owner = blockRecord,
                    Origin = basePoint,
                    Layer = layer,
                    Flags = type,
                    End = end
                };
            }
            else
            {
                block = new Block(name, false, null, null)
                {
                    Handle = handle,
                    Owner = blockRecord,
                    Origin = basePoint,
                    Layer = layer,
                    Flags = type,
                    End = end
                };
            }

            if (name.StartsWith(Block.DefaultPaperSpaceName, StringComparison.OrdinalIgnoreCase))
            {
                // the dxf is not consistent with the way they handle entities that belong to different paper spaces.
                // While the entities of *Paper_Space block are stored in the ENTITIES section as the *Model_Space,
                // the list of entities in *Paper_Space# are stored in the block definition itself.
                // As all this entities do not need an insert entity to have a visual representation,
                // they will be stored in the global entities lists together with the rest of the entities of *Model_Space and *Paper_Space
                foreach (EntityObject entity in entities)
                    this.entityList.Add(entity, blockRecord.Handle);

                // this kind of blocks do not store attribute definitions
            }
            else
            {             
                // add attribute definitions
                foreach (AttributeDefinition attDef in attDefs)
                {
                    // AutoCAD allows duplicate tags in attribute definitions, but this library does not having duplicate tags is not recommended in any way,
                    // since there will be no way to know which is the definition associated to the insert attribute
                    if(!block.AttributeDefinitions.ContainsTag(attDef.Tag))
                        block.AttributeDefinitions.Add(attDef);
                }
                // block entities for post processing (MLines and Images references other objects (MLineStyle and ImageDefinition) that will be defined later
                this.blockEntities.Add(block, entities);
            }

            return block;
        }

        private AttributeDefinition ReadAttributeDefinition()
        {
            string id = string.Empty;
            string text = string.Empty;
            object value = null;
            AttributeFlags flags = AttributeFlags.Visible;
            Vector3 firstAlignmentPoint = Vector3.Zero;
            Vector3 secondAlignmentPoint = Vector3.Zero;
            TextStyle style = TextStyle.Default;
            double height = 0.0;
            double widthFactor = 0.0;
            short horizontalAlignment = 0;
            short verticalAlignment = 0;
            double rotation = 0.0;
            double obliqueAngle = 0.0;
            Vector3 normal = Vector3.UnitZ;

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        id = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 3:
                        text = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 1:
                        value = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (AttributeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 10:
                        firstAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        firstAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        firstAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        secondAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        secondAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        secondAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 7:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        style = this.GetTextStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        widthFactor = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 51:
                        obliqueAngle = MathHelper.NormalizeAngle(this.chunk.ReadDouble());
                        if (obliqueAngle > 180) obliqueAngle -= 360;
                        if (obliqueAngle < -85.0 || obliqueAngle > 85.0) obliqueAngle = 0.0;
                        this.chunk.Next();
                        break;
                    case 72:
                        horizontalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 74:
                        verticalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            TextAlignment alignment = ObtainAlignment(horizontalAlignment, verticalAlignment);
            Vector3 ocsBasePoint = alignment == TextAlignment.BaselineLeft ? firstAlignmentPoint : secondAlignmentPoint;
            Vector3 wcsBasePoint = MathHelper.Transform(ocsBasePoint, normal, CoordinateSystem.Object, CoordinateSystem.World);

            AttributeDefinition attDef = new AttributeDefinition(id)
            {
                Position = wcsBasePoint,
                Normal = normal,
                Alignment = alignment,
                Prompt = text,
                Value = value,
                Flags = flags,
                Style = style,
                Height = height,
                WidthFactor = MathHelper.IsZero(widthFactor) ? style.WidthFactor : widthFactor,
                ObliqueAngle = obliqueAngle,
                Rotation = rotation
            };

            return attDef;
        }

        private Attribute ReadAttribute(Block block, bool isBlockEntity = false)
        {
            string handle = null;
            Layer layer = Layer.Default;
            AciColor color = AciColor.ByLayer;
            LineType lineType = LineType.ByLayer;
            Lineweight lineweight = Lineweight.ByLayer;
            double linetypeScale = 1.0;
            bool isVisible = true;
            Transparency transparency = Transparency.ByLayer;

            AttributeFlags flags = AttributeFlags.Visible;
            Vector3 firstAlignmentPoint = Vector3.Zero;
            Vector3 secondAlignmentPoint = Vector3.Zero;
            TextStyle style = TextStyle.Default;
            double height = 0.0;
            double widthFactor = 0.0;
            double obliqueAngle = 0.0;
            short horizontalAlignment = 0;
            short verticalAlignment = 0;
            double rotation = 0.0;
            Vector3 normal = Vector3.UnitZ;

            // DxfObject codes
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new DxfObjectException(DxfObjectCode.Attribute, "Premature end of entity definition.");
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // AcDbEntity common codes
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new DxfObjectException(DxfObjectCode.Attribute, "Premature end of entity definition.");
                    case 8: // layer code
                        string layerName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(layerName);
                        this.chunk.Next();
                        break;
                    case 62: // ACI color code
                        if (!color.UseTrueColor)
                            color = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 440: //transparency
                        transparency = Transparency.FromAlphaValue(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 420: // the entity uses true color
                        color = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 6: // type line code
                        // the line type names ByLayer or ByBlock are case insensitive
                        string lineTypeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.Equals(lineTypeName, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByLayerName;
                        if (string.Equals(lineTypeName, LineType.ByBlockName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByBlockName;
                        lineType = this.GetLineType(lineTypeName);
                        this.chunk.Next();
                        break;
                    case 370: // line weight code
                        lineweight = Lineweight.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 48: // line type scale
                        linetypeScale = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 60: //object visibility
                        isVisible = this.chunk.ReadShort() == 0;
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                    
                }
            }

            string atttag = null;
            AttributeDefinition attdef = null;
            object value = null;

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        atttag = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        // seems that some programs might export insert entities with attributes which definitions are not defined in the block
                        // if it is not present the insert attribute will have a null definition
                        if (!isBlockEntity)
                            block.AttributeDefinitions.TryGetValue(atttag, out attdef);
                        this.chunk.Next();
                        break;
                    case 1:
                        value = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (AttributeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 10:
                        firstAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        firstAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        firstAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        secondAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        secondAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        secondAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 7:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        style = this.GetTextStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        widthFactor = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 51:
                        obliqueAngle = MathHelper.NormalizeAngle(this.chunk.ReadDouble());
                        if (obliqueAngle > 180) obliqueAngle -= 360;
                        if (obliqueAngle < -85.0 || obliqueAngle > 85.0)
                            obliqueAngle = 0.0;
                        this.chunk.Next();
                        break;
                    case 72:
                        horizontalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 74:
                        verticalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            TextAlignment alignment = ObtainAlignment(horizontalAlignment, verticalAlignment);
            Vector3 ocsBasePoint = alignment == TextAlignment.BaselineLeft ? firstAlignmentPoint : secondAlignmentPoint;
            Vector3 wcsBasePoint = MathHelper.Transform(ocsBasePoint, normal, CoordinateSystem.Object, CoordinateSystem.World);

            return new Attribute
            {
                Handle = handle,
                Color = color,
                Layer = layer,
                LineType = lineType,
                Lineweight = lineweight,
                LineTypeScale = linetypeScale,
                Transparency = transparency,
                IsVisible = isVisible,
                Definition = attdef,
                Tag = atttag,
                Position = wcsBasePoint,
                Normal = normal,
                Alignment = alignment,
                Value = value,
                Flags = flags,
                Style = style,
                Height = height,
                WidthFactor = MathHelper.IsZero(widthFactor) ? style.WidthFactor : widthFactor,
                ObliqueAngle = obliqueAngle,
                Rotation = rotation
            };
        }

        #endregion

        #region entity methods

        private EntityObject ReadEntity(bool isBlockEntity)
        {
            string handle = null;
            string owner = null;
            Layer layer = Layer.Default;
            AciColor color = AciColor.ByLayer;
            LineType lineType = LineType.ByLayer;
            Lineweight lineweight = Lineweight.ByLayer;
            double linetypeScale = 1.0;
            bool isVisible = true;
            Transparency transparency = Transparency.ByLayer;

            EntityObject entityObject;

            string dxfCode = this.chunk.ReadString();
            this.chunk.Next();

            // DxfObject common codes
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new DxfObjectException(dxfCode, "Premature end of entity definition.");
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 102:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;
                    case 330:
                        owner = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // AcDbEntity common codes
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.Entity);
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new DxfObjectException(dxfCode, "Premature end of entity definition.");
                    case 8: //layer code
                        string layerName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(layerName);
                        this.chunk.Next();
                        break;
                    case 62: //ACI color code
                        if (!color.UseTrueColor)
                            color = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 420: //the entity uses true color
                        color = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 440: //transparency
                        transparency = Transparency.FromAlphaValue(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 6: //type line code
                        // the line type names ByLayer or ByBlock are case insensitive
                        string lineTypeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.Equals(lineTypeName, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByLayerName;
                        else if (string.Equals(lineTypeName, LineType.ByBlockName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByBlockName;
                        lineType = this.GetLineType(lineTypeName);
                        this.chunk.Next();
                        break;
                    case 370: //line weight code
                        short index = this.chunk.ReadShort();
                        if (index < -3 || index > 200)
                            index = -1;
                        lineweight = Lineweight.FromCadIndex(index);
                        this.chunk.Next();
                        break;
                    case 48: //line type scale
                        linetypeScale = this.chunk.ReadDouble();
                        if (linetypeScale <= 0.0)
                            linetypeScale = 1.0;
                        this.chunk.Next();
                        break;
                    case 60: //object visibility
                        isVisible = this.chunk.ReadShort() == 0;
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            switch (dxfCode)
            {
                case DxfObjectCode.Arc:
                    entityObject = this.ReadArc();
                    break;
                case DxfObjectCode.AttributeDefinition:
                    entityObject = this.ReadAttributeDefinition();
                    break;
                case DxfObjectCode.Circle:
                    entityObject = this.ReadCircle();
                    break;
                case DxfObjectCode.Dimension:
                    entityObject = this.ReadDimension(isBlockEntity);
                    break;
                case DxfObjectCode.Ellipse:
                    entityObject = this.ReadEllipse();
                    break;
                case DxfObjectCode.Face3d:
                    entityObject = this.ReadFace3d();
                    break;
                case DxfObjectCode.Hatch:
                    entityObject = this.ReadHatch();
                    break;
                case DxfObjectCode.Image:
                    entityObject = this.ReadImage();
                    break;
                case DxfObjectCode.Insert:
                    entityObject = this.ReadInsert(isBlockEntity);
                    break;
                case DxfObjectCode.Leader:
                    entityObject = this.ReadLeader();
                    break;
                case DxfObjectCode.Line:
                    entityObject = this.ReadLine();
                    break;
                case DxfObjectCode.LightWeightPolyline:
                    entityObject = this.ReadLwPolyline();
                    break;
                case DxfObjectCode.Mesh:
                    entityObject = this.ReadMesh();
                    break;
                case DxfObjectCode.MLine:
                    entityObject = this.ReadMLine();
                    break;
                case DxfObjectCode.MText:
                    entityObject = this.ReadMText();
                    break;
                case DxfObjectCode.Point:
                    entityObject = this.ReadPoint();
                    break;
                case DxfObjectCode.Polyline:
                    entityObject = this.ReadPolyline();
                    break;
                case DxfObjectCode.Ray:
                    entityObject = this.ReadRay();
                    break;
                case DxfObjectCode.Solid:
                    entityObject = this.ReadSolid();
                    break;
                case DxfObjectCode.Text:
                    entityObject = this.ReadText();
                    break;
                case DxfObjectCode.Tolerance:
                    entityObject = this.ReadTolerance();
                    break;
                case DxfObjectCode.Trace:
                    entityObject = this.ReadTrace();
                    break;
                case DxfObjectCode.Spline:
                    entityObject = this.ReadSpline();
                    break;
                case DxfObjectCode.UnderlayDgn:
                    entityObject = this.ReadUnderlay();
                    break;
                case DxfObjectCode.UnderlayDwf:
                    entityObject = this.ReadUnderlay();
                    break;
                case DxfObjectCode.UnderlayPdf:
                    entityObject = this.ReadUnderlay();
                    break;
                case DxfObjectCode.Viewport:
                    entityObject = this.ReadViewport();
                    break;
                case DxfObjectCode.XLine:
                    entityObject = this.ReadXLine();
                    break;
                case DxfObjectCode.Wipeout:
                    entityObject = this.ReadWipeout();
                    break;
                default:
                    this.ReadUnknowEntity();
                    return null;
            }

            if (entityObject == null || string.IsNullOrEmpty(handle)) return null;

            entityObject.Handle = handle;
            entityObject.Layer = layer;
            entityObject.Color = color;
            entityObject.LineType = lineType;
            entityObject.Lineweight = lineweight;
            entityObject.LineTypeScale = linetypeScale;
            entityObject.IsVisible = isVisible;
            entityObject.Transparency = transparency;

            // the entities list will be processed at the end
            if (!isBlockEntity) this.entityList.Add(entityObject, owner);

            return entityObject;
        }

        private Wipeout ReadWipeout()
        {
            Vector3 position = Vector3.Zero;
            Vector3 u = Vector3.UnitX;
            Vector3 v = Vector3.UnitY;
            ClippingBoundaryType boundaryType = ClippingBoundaryType.Rectangular;
            double x = 0.0;
            double y = 0.0;
            List<Vector2> vertexes = new List<Vector2>();
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        position.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        position.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        position.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        u.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        u.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        u.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        v.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        v.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        v.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;              
                    case 71:
                        boundaryType = (ClippingBoundaryType)this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 91:
                        // we cannot rely in this information it might or might not appear
                        this.chunk.Next();
                        break;
                    case 14:
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        vertexes.Add(new Vector2(x, y));
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            // for polygonal boundaries the last vertex is equal to the first, we will remove it
            if (boundaryType == ClippingBoundaryType.Polygonal)
                vertexes.RemoveAt(vertexes.Count-1);

            Vector3 normal = Vector3.Normalize(Vector3.CrossProduct(u, v));
            IList<Vector3> ocsPoints = MathHelper.Transform(new List<Vector3> {position, u, v}, normal, CoordinateSystem.World, CoordinateSystem.Object);
            double bx = ocsPoints[0].X;
            double by = ocsPoints[0].Y;
            double elevation = ocsPoints[0].Z;
            double max = ocsPoints[1].X;

            for (int i = 0; i < vertexes.Count; i++)
            {
                double vx = bx + max * (vertexes[i].X + 0.5);
                double vy = by + max * (0.5 - vertexes[i].Y);
                vertexes[i] = new Vector2(vx, vy);
            }

            ClippingBoundary clippingBoundary = boundaryType == ClippingBoundaryType.Rectangular ? new ClippingBoundary(vertexes[0], vertexes[1]) : new ClippingBoundary(vertexes);
            Wipeout entity = new Wipeout(clippingBoundary)
            {
                Normal = normal,
                Elevation = elevation
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Underlay ReadUnderlay()
        {
            string underlayDefHandle = null;
            Vector3 position = Vector3.Zero;
            Vector3 scale = new Vector3(1.0);
            double rotation = 0.0;
            Vector3 normal = Vector3.UnitZ;
            UnderlayDisplayFlags displayOptions = UnderlayDisplayFlags.ShowUnderlay;
            short contrast = 100;
            short fade = 0;
            Vector2 clippingVertex = Vector2.Zero;
            List<Vector2> clippingVertexes = new List<Vector2>();
            ClippingBoundary clippingBoundary;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        position.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        position.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        position.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        scale.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 42:
                        scale.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        scale.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 340:
                        underlayDefHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 280:
                        displayOptions = (UnderlayDisplayFlags)this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 281:
                        contrast = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 282:
                        fade = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 11:
                        clippingVertex = new Vector2();
                        clippingVertex.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        clippingVertex.Y = this.chunk.ReadDouble();
                        clippingVertexes.Add(clippingVertex);
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Vector3 wcsPosition = MathHelper.Transform(position, normal, CoordinateSystem.Object, CoordinateSystem.World);
            if (clippingVertexes.Count < 2)
                clippingBoundary = null;
            else if (clippingVertexes.Count == 2)
                clippingBoundary = new ClippingBoundary(clippingVertexes[0], clippingVertexes[1]);
            else
                clippingBoundary = new ClippingBoundary(clippingVertexes);

            Underlay underlay = new Underlay
            {
                Position = wcsPosition,
                Scale = scale,
                Normal = normal,
                Rotation = rotation,
                DisplayOptions = displayOptions,
                Contrast = contrast,
                Fade = fade,
                ClippingBoundary = clippingBoundary
            };

            underlay.XData.AddRange(xData);

            if (string.IsNullOrEmpty(underlayDefHandle) || underlayDefHandle == "0") return null;

            this.underlayToDefinitionHandles.Add(underlay, underlayDefHandle);

            return underlay;
        }

        private Tolerance ReadTolerance()
        {
            DimensionStyle style = DimensionStyle.Default;
            Vector3 position = Vector3.Zero;
            string value = string.Empty;
            Vector3 normal = Vector3.UnitZ;
            Vector3 xAxis = Vector3.UnitX;

            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 3:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.DimStyle;
                        style = this.GetDimensionStyle(styleName);
                        this.chunk.Next();
                        break;

                    case 10:
                        position.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        position.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        position.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1:
                        value = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        xAxis.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        xAxis.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        xAxis.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            xAxis = MathHelper.Transform(xAxis, normal, CoordinateSystem.World, CoordinateSystem.Object);
            double rotation = Vector2.Angle(new Vector2(xAxis.X, xAxis.Y));

            Tolerance entity = Tolerance.ParseRepresentation(value);
            entity.Style = style;
            entity.Position = position;
            entity.Rotation = rotation * MathHelper.RadToDeg;
            entity.Normal = normal;

            entity.XData.AddRange(xData);

            return entity;
        }

        private Leader ReadLeader()
        {
            DimensionStyle style = DimensionStyle.Default;
            bool showArrowhead = true;
            LeaderPathType path = LeaderPathType.StraightLineSegements;
            bool hasHookline = false;
            List<Vector3> wcsVertexes = null;
            AciColor lineColor = AciColor.ByLayer;
            string annotation = string.Empty;
            Vector3 normal = Vector3.UnitZ;
            double elevation = 0.0;
            Vector3 offset = Vector3.Zero;

            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 3:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.DimStyle;
                        style = this.GetDimensionStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 71:
                        showArrowhead = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 72:
                        path = (LeaderPathType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 73:
                        this.chunk.Next();
                        break;
                    case 74:
                        this.chunk.Next();
                        break;
                    case 75:
                        hasHookline = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 76:
                        this.chunk.Next();
                        break;
                    case 77:
                        lineColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 10:
                        wcsVertexes = this.ReadLeaderVertexes();
                        break;
                    case 340:
                        annotation = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 213:
                        offset.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 223:
                        offset.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 233:
                        offset.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            if (hasHookline) wcsVertexes.RemoveAt(wcsVertexes.Count - 2);
            IList<Vector3> ocsVertexes = MathHelper.Transform(wcsVertexes, normal, CoordinateSystem.World, CoordinateSystem.Object);
            List<Vector2> vertexes = new List<Vector2>();
            foreach (Vector3 v in ocsVertexes)
            {
                vertexes.Add(new Vector2(v.X, v.Y));
                elevation = v.Z;
            }

            // The text vertical position is stored in the Leader extended data
            Vector3 ocsOffset = MathHelper.Transform(offset, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Leader leader = new Leader(vertexes)
            {
                Style = style,
                ShowArrowhead = showArrowhead,
                PathType = path,
                LineColor = lineColor,
                Elevation = elevation,
                Normal = normal,
                Offset = new Vector2(ocsOffset.X, ocsOffset.Y)
            };

            leader.XData.AddRange(xData);

            // this is for post-processing, the annotation entity might appear after the leader
            this.leaderAnnotation.Add(leader, annotation);

            LeaderTextVerticalPosition txtPosition = LeaderTextVerticalPosition.Above;
            XData txtPosData;
            if (leader.XData.TryGetValue(ApplicationRegistry.Default.Name, out txtPosData))
            {
                bool dstyleInfo = false;
                List<XDataRecord>.Enumerator entries = txtPosData.XDataRecord.GetEnumerator();
                while (entries.MoveNext())
                {
                    XDataRecord data = entries.Current;
                    // in this XDataRecord is where the leader text vertical position is stored
                    if (data.Code == XDataCode.String)
                    {
                        if (string.Equals((string)data.Value, "DSTYLE", StringComparison.OrdinalIgnoreCase)) dstyleInfo = true;
                    }

                    if (data.Code == XDataCode.Int16)
                    {
                        // the leader text vertical position is stored in value 77
                        if ((short)data.Value == 77 && dstyleInfo)
                        {
                            if (entries.MoveNext())
                                    data = entries.Current;
                                else // unexpected end of the DSTYLE information         
                                    break;
                            txtPosition = (LeaderTextVerticalPosition)(short)data.Value;
                            break;
                        }
                    }
                }
            }

            leader.TextVerticalPosition = txtPosition;

            return leader;
        }

        private List<Vector3> ReadLeaderVertexes()
        {
            List<Vector3> vertexes = new List<Vector3>();
            while (this.chunk.Code == 10)
            {
                Vector3 vertex = Vector3.Zero;
                vertex.X = this.chunk.ReadDouble();
                this.chunk.Next();
                vertex.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                if (this.chunk.Code == 30)
                {
                    vertex.Z = this.chunk.ReadDouble();
                    this.chunk.Next();
                }
                vertexes.Add(vertex);
            }
            return vertexes;
        }

        private Mesh ReadMesh()
        {
            int subdivisionLevel = 0;
            List<Vector3> vertexes = null;
            List<int[]> faces = null;
            List<MeshEdge> edges = null;
            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 91:
                        subdivisionLevel = this.chunk.ReadInt();
                        if (subdivisionLevel < 0 || subdivisionLevel > 255) subdivisionLevel = 0;
                        this.chunk.Next();
                        break;
                    case 92:
                        int numVertexes = this.chunk.ReadInt();
                        this.chunk.Next();
                        vertexes = this.ReadMeshVertexes(numVertexes);
                        break;
                    case 93:
                        int sizeFaceList = this.chunk.ReadInt();
                        this.chunk.Next();
                        faces = this.ReadMeshFaces(sizeFaceList);
                        break;
                    case 94:
                        int numEdges = this.chunk.ReadInt();
                        this.chunk.Next();
                        edges = this.ReadMeshEdges(numEdges);
                        break;
                    case 95:
                        int numCrease  = this.chunk.ReadInt();
                        this.chunk.Next();
                        if (edges == null)
                            throw new NullReferenceException("The edges list is not initialized.");
                        if (numCrease != edges.Count)
                            throw new Exception("The number of edge creases must be the same as the number of edges.");
                        this.ReadMeshEdgeCreases(edges);
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(), "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }
            Mesh entity = new Mesh(vertexes, faces, edges)
                        {
                            SubdivisionLevel = (byte)subdivisionLevel,
                        };

            entity.XData.AddRange(xData);

            return entity;

        }

        private List<Vector3> ReadMeshVertexes(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "The number of vertexes must be greater than zero.");

            List<Vector3> vertexes = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
            {
                double x = this.chunk.ReadDouble();
                this.chunk.Next();
                double y = this.chunk.ReadDouble();
                this.chunk.Next();
                double z = this.chunk.ReadDouble();
                this.chunk.Next();

                vertexes.Add(new Vector3(x, y, z));
            }

            return vertexes;
        }

        private List<int[]> ReadMeshFaces(int size)
        {
            Debug.Assert(size > 0, "The size of face list must be greater than zero.");

            List<int[]> faces = new List<int[]>();

            for (int i = 0; i < size; i++)
            {
                int indexes = this.chunk.ReadInt();
                this.chunk.Next();
                int[] face = new int[indexes];
                for (int j = 0; j < indexes; j++)
                {
                    face[j] = this.chunk.ReadInt();
                    this.chunk.Next();
                }
                faces.Add(face);
                i += indexes;
            }

            return faces;
        }

        private List<MeshEdge> ReadMeshEdges(int count)
        {
            List<MeshEdge> vertexes = new List<MeshEdge>(count);

            for (int i = 0; i < count; i++)
            {
                int start = this.chunk.ReadInt();
                this.chunk.Next();
                int end = this.chunk.ReadInt();
                this.chunk.Next();

                vertexes.Add(new MeshEdge(start, end));
            }
            return vertexes;
        }

        private void ReadMeshEdgeCreases(IList<MeshEdge> edges)
        {
            foreach (MeshEdge edge in edges)
            {
                edge.Crease = this.chunk.ReadDouble();
                this.chunk.Next();
            }
        }

        private Viewport ReadViewport()
        {
            Debug.Assert(this.chunk.ReadString() == SubclassMarker.Viewport);

            Viewport viewport = new Viewport();
            Vector3 center = viewport.Center;
            Vector2 viewCenter = viewport.ViewCenter;
            Vector2 snapBase = viewport.SnapBase;
            Vector2 snapSpacing = viewport.SnapSpacing;
            Vector2 gridSpacing = viewport.GridSpacing;
            Vector3 viewDirection = viewport.ViewDirection;
            Vector3 viewTarget = viewport.ViewTarget;
            Vector3 ucsOrigin = Vector3.Zero;
            Vector3 ucsXAxis = Vector3.UnitX;
            Vector3 ucsYAxis = Vector3.UnitY;

            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        viewport.Width = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        viewport.Height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 68:
                        viewport.Stacking = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 69:
                        viewport.Id = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 12:
                        viewCenter.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        viewCenter.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        snapBase.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        snapBase.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        snapSpacing.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        snapSpacing.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 15:
                        gridSpacing.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        gridSpacing.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 16:
                        viewDirection.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 26:
                        viewDirection.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 36:
                        viewDirection.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 17:
                        viewTarget.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 27:
                        viewTarget.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 37:
                        viewTarget.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 42:
                        viewport.LensLength = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        viewport.FrontClipPlane = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 44:
                        viewport.BackClipPlane = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 45:
                        viewport.ViewHeight = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        viewport.SnapAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 51:
                        viewport.TwistAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 72:
                        viewport.CircleZoomPercent = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 331:
                        Layer layer = (Layer) this.doc.GetObjectByHandle(this.chunk.ReadString());
                        viewport.FrozenLayers.Add(layer);
                        this.chunk.Next();
                        break;
                    case 90:
                        viewport.Status = (ViewportStatusFlags) this.chunk.ReadInt();
                        this.chunk.Next();
                        break;
                    case 340:
                        // we will post process the clipping boundary in case it has been defined before the viewport
                        this.viewports.Add(viewport, this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 110:
                        ucsOrigin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 120:
                        ucsOrigin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 130:
                        ucsOrigin.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 111:
                        ucsXAxis.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 121:
                        ucsXAxis.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 131:
                        ucsXAxis.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 112:
                        ucsYAxis.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 122:
                        ucsYAxis.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 132:
                        ucsYAxis.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            viewport.Center = center;
            viewport.ViewCenter = viewCenter;
            viewport.SnapBase = snapBase;
            viewport.SnapSpacing = snapSpacing;
            viewport.GridSpacing = gridSpacing;
            viewport.ViewDirection = viewDirection;
            viewport.ViewTarget = viewTarget;
            viewport.UcsOrigin = ucsOrigin;
            viewport.UcsXAxis = ucsXAxis;
            viewport.UcsYAxis = ucsYAxis;

            viewport.XData.AddRange(xData);

            return viewport;
        }

        private Image ReadImage()
        {
            Vector3 position = Vector3.Zero;
            Vector3 u = Vector3.Zero;
            Vector3 v = Vector3.Zero;
            double width = 0.0, height = 0.0;
            string imageDefHandle = null;
            ImageDisplayFlags displayOptions = ImageDisplayFlags.ShowImage | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.UseClippingBoundary;
            bool clipping = false;
            short brightness = 50;
            short contrast = 50;
            short fade = 0;
            double x = 0.0;
            double y = 0.0;
            List<Vector2> vertexes = new List<Vector2>();
            ClippingBoundaryType boundaryType = ClippingBoundaryType.Rectangular;

            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        position.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        position.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        position.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        u.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        u.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        u.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        v.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        v.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        v.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        width = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 340:
                        imageDefHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 70:
                        displayOptions = (ImageDisplayFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 280:
                        clipping = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 281:
                        brightness = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 282:
                        contrast = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 283:
                        fade = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        boundaryType = (ClippingBoundaryType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 91:
                        // we cannot rely in this information it might or might not appear
                        this.chunk.Next();
                        break;
                    case 14:
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        vertexes.Add(new Vector2(x, y));
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Vector3 normal = Vector3.CrossProduct(u, v);
            Vector3 uOCS = MathHelper.Transform(u, normal, CoordinateSystem.World, CoordinateSystem.Object);
            double rotation = Vector2.Angle(new Vector2(uOCS.X, uOCS.Y))*MathHelper.RadToDeg;
            double uLength = u.Modulus();
            double vLength = v.Modulus();

            for (int i = 0; i < vertexes.Count; i++)
            {
                double vx = vertexes[i].X * uLength;
                double vy = vertexes[i].Y * vLength;
                vertexes[i] = new Vector2(vx, vy);
            }

            ClippingBoundary clippingBoundary = boundaryType == ClippingBoundaryType.Rectangular ? new ClippingBoundary(vertexes[0], vertexes[1]) : new ClippingBoundary(vertexes);

            Image image = new Image
            {
                Width = width*uLength,
                Height = height*vLength,
                Position = position,
                Normal = normal,
                Rotation = rotation,
                DisplayOptions = displayOptions,
                Clipping = clipping,
                Brightness = brightness,
                Contrast = contrast,
                Fade = fade,
                ClippingBoundary = clippingBoundary
            };

            image.XData.AddRange(xData);

            if (string.IsNullOrEmpty(imageDefHandle) || imageDefHandle == "0") return null;

            this.imgToImgDefHandles.Add(image, imageDefHandle);

            return image;
        }

        private Arc ReadArc()
        {
            Vector3 center = Vector3.Zero;
            double radius = 1.0;
            double startAngle = 0.0;
            double endAngle = 180.0;
            double thickness = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<XData> xData = new List<XData>();

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        radius = this.chunk.ReadDouble();
                        if (radius <= 0) radius = 1.0;
                        this.chunk.Next();
                        break;
                    case 50:
                        startAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 51:
                        endAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            // this is just an example of the stupid autodesk dxf way of doing things, while an ellipse the center is given in world coordinates,
            // the center of an arc is given in object coordinates (different rules for the same concept).
            // It is a lot more intuitive to give the center in world coordinates and then define the orientation with the normal..
            Vector3 wcsCenter = MathHelper.Transform(center, normal, CoordinateSystem.Object, CoordinateSystem.World);
            Arc entity = new Arc
            {
                Center = wcsCenter,
                Radius = radius,
                StartAngle = startAngle,
                EndAngle = endAngle,
                Thickness = thickness,
                Normal = normal
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Circle ReadCircle()
        {
            Vector3 center = Vector3.Zero;
            double radius = 1.0;
            double thickness = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        radius = this.chunk.ReadDouble();
                        if (radius <= 0) radius = 1.0;
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            // this is just an example of the stupid autodesk dxf way of doing things, while an ellipse the center is given in world coordinates,
            // the center of a circle is given in object coordinates (different rules for the same concept).
            // It is a lot more intuitive to give the center in world coordinates and then define the orientation with the normal..
            Vector3 wcsCenter = MathHelper.Transform(center, normal, CoordinateSystem.Object, CoordinateSystem.World);

            Circle entity = new Circle
            {
                Center = wcsCenter,
                Radius = radius,
                Thickness = thickness,
                Normal = normal
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Dimension ReadDimension(bool isBlockEntity = false)
        {
            string drawingBlockName = null;
            Block drawingBlock = null;
            Vector3 defPoint = Vector3.Zero;
            Vector3 midtxtPoint = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            DimensionTypeFlags dimType = DimensionTypeFlags.Linear;
            MTextAttachmentPoint attachmentPoint = MTextAttachmentPoint.BottomCenter;
            MTextLineSpacingStyle lineSpacingStyle = MTextLineSpacingStyle.AtLeast;
            DimensionStyle style = DimensionStyle.Default;
            double dimRot = 0.0;
            double lineSpacingFactor = 1.0;
            bool dimInfo = false;
            string userText = null;

            this.chunk.Next();
            while (!dimInfo)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                        userText = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if(string.IsNullOrEmpty(userText.Trim(' ', '\t'))) userText = " ";
                        this.chunk.Next();
                        break;
                    case 2:
                        drawingBlockName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity) drawingBlock = this.GetBlock(drawingBlockName);
                        this.chunk.Next();
                        break;
                    case 3:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.DimStyle;
                        style = this.GetDimensionStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 10:
                        defPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        defPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        defPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        midtxtPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        midtxtPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        midtxtPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        dimType = (DimensionTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        attachmentPoint = (MTextAttachmentPoint) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 72:
                        lineSpacingStyle = (MTextLineSpacingStyle) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 41:
                        lineSpacingFactor = this.chunk.ReadDouble();
                        if (lineSpacingFactor < 0.25 || lineSpacingFactor > 4.0)
                            lineSpacingFactor = 1.0;
                        this.chunk.Next();
                        break;
                    case 51:
                        // even if the documentation says that code 51 is optional, rotated ordinate dimensions will not work correctly is this value is not provided
                        dimRot = 360 - this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 100:
                        string marker = this.chunk.ReadString();
                        if (marker == SubclassMarker.AlignedDimension ||
                            marker == SubclassMarker.RadialDimension ||
                            marker == SubclassMarker.DiametricDimension ||
                            marker == SubclassMarker.Angular3PointDimension ||
                            marker == SubclassMarker.Angular2LineDimension ||
                            marker == SubclassMarker.OrdinateDimension)
                            dimInfo = true; // we have finished reading the basic dimension info
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // this is the result of the way the dxf use the DimensionTypeFlag enumeration, it is a mixture of a regular enumeration with flags
            DimensionTypeFlags type = dimType;
            OrdinateDimensionAxis axis = OrdinateDimensionAxis.Y;
            if ((type & DimensionTypeFlags.BlockReference) == DimensionTypeFlags.BlockReference)
                type -= DimensionTypeFlags.BlockReference;
            if ((type & DimensionTypeFlags.OrdinteType) == DimensionTypeFlags.OrdinteType)
            {
                axis = OrdinateDimensionAxis.X;
                type -= DimensionTypeFlags.OrdinteType;
            }
            if ((type & DimensionTypeFlags.UserTextPosition) == DimensionTypeFlags.UserTextPosition)
                type -= DimensionTypeFlags.UserTextPosition;

            Dimension dim;
            switch (type)
            {
                case DimensionTypeFlags.Aligned:
                    dim = this.ReadAlignedDimension(defPoint);
                    break;
                case DimensionTypeFlags.Linear:
                    dim = this.ReadLinearDimension(defPoint);
                    break;
                case DimensionTypeFlags.Radius:
                    dim = this.ReadRadialDimension(defPoint, midtxtPoint);
                    break;
                case DimensionTypeFlags.Diameter:
                    dim = this.ReadDiametricDimension(defPoint, midtxtPoint);
                    break;
                case DimensionTypeFlags.Angular3Point:
                    dim = this.ReadAngular3PointDimension(defPoint, normal);
                    break;
                case DimensionTypeFlags.Angular:
                    dim = this.ReadAngular2LineDimension(defPoint);
                    break;
                case DimensionTypeFlags.Ordinate:
                    dim = this.ReadOrdinateDimension(defPoint, axis, normal, dimRot);
                    break;
                default:
                    throw new ArgumentException("The dimension type: " + type + " is not implemented or unknown.");
            }

            if (dim == null) return null;
            if (drawingBlockName == null) return null;

            dim.Style = style;
            dim.Block = drawingBlock;
            dim.DefinitionPoint = defPoint;
            dim.MidTextPoint = midtxtPoint;
            dim.AttachmentPoint = attachmentPoint;
            dim.LineSpacingStyle = lineSpacingStyle;
            dim.LineSpacingFactor = lineSpacingFactor;
            dim.Normal = normal;
            dim.UserText = userText;

            if (isBlockEntity)
                this.nestedDimensions.Add(dim, drawingBlockName);

            return dim;
        }

        private AlignedDimension ReadAlignedDimension(Vector3 defPoint)
        {
            Vector3 firstRef = Vector3.Zero;
            Vector3 secondRef = Vector3.Zero;
            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        firstRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        firstRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        firstRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        secondRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        secondRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        secondRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }
            double offset = Vector3.Distance(secondRef, defPoint);
            
            AlignedDimension entity = new AlignedDimension
            {
                FirstReferencePoint = firstRef,
                SecondReferencePoint = secondRef,
                Offset = offset
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private LinearDimension ReadLinearDimension(Vector3 defPoint)
        {
            Vector3 firtRef = Vector3.Zero;
            Vector3 secondRef = Vector3.Zero;
            double rot = 0.0;
            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        firtRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        firtRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        firtRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        secondRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        secondRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        secondRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rot = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 52:
                        // AutoCAD is unable to recognized code 52 for oblique dimension line even though it appears as valid in the dxf documentation
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Vector3 midPoint = Vector3.MidPoint(firtRef, secondRef);
            Vector3 origin = defPoint;
            Vector3 dir = new Vector3(Math.Cos(rot*MathHelper.DegToRad), Math.Sin(rot*MathHelper.DegToRad), 0.0);
            dir.Normalize();
            double offset = MathHelper.PointLineDistance(midPoint, origin, dir);

            LinearDimension entity = new LinearDimension
            {
                FirstReferencePoint = firtRef,
                SecondReferencePoint = secondRef,
                Offset = offset,
                Rotation = rot
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private RadialDimension ReadRadialDimension(Vector3 defPoint, Vector3 midtxtPoint)
        {
            Vector3 circunferenceRef = Vector3.Zero;
            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 15:
                        circunferenceRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        circunferenceRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 35:
                        circunferenceRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            double offset = Vector3.Distance(defPoint, midtxtPoint);
            RadialDimension entity = new RadialDimension
            {
                CenterPoint = Vector3.MidPoint(defPoint, circunferenceRef),
                ReferencePoint = circunferenceRef,
                Offset = offset
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private DiametricDimension ReadDiametricDimension(Vector3 defPoint, Vector3 midtxtPoint)
        {
            Vector3 circunferenceRef = Vector3.Zero;
            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 15:
                        circunferenceRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        circunferenceRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 35:
                        circunferenceRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Vector3 center = Vector3.MidPoint(defPoint, circunferenceRef);
            double offset = Vector3.Distance(center, midtxtPoint);
            DiametricDimension entity = new DiametricDimension
            {
                CenterPoint = center,
                ReferencePoint = circunferenceRef,
                Offset = offset
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Angular3PointDimension ReadAngular3PointDimension(Vector3 defPoint, Vector3 normal)
        {
            Vector3 center = Vector3.Zero;
            Vector3 firstRef = Vector3.Zero;
            Vector3 secondRef = Vector3.Zero;

            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        firstRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        firstRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        firstRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        secondRef.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        secondRef.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        secondRef.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 15:
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 35:
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            IList<Vector3> ocsPoints = MathHelper.Transform(new[] {center, firstRef, secondRef, defPoint}, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 refPoint;

            refPoint = ocsPoints[0];
            Vector2 refCenter = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[1];
            Vector2 ref1 = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[2];
            Vector2 ref2 = new Vector2(refPoint.X, refPoint.Y);

            refPoint = ocsPoints[3];
            Vector2 midTxt = new Vector2(refPoint.X, refPoint.Y);

            double aperture = (Vector2.Angle(refCenter, ref2) - Vector2.Angle(refCenter, ref1)) * MathHelper.RadToDeg;
            aperture = MathHelper.NormalizeAngle(aperture);
            double angle = (Vector2.Angle(refCenter, ref2) - Vector2.Angle(refCenter, midTxt)) * MathHelper.RadToDeg;
            angle = MathHelper.NormalizeAngle(angle);

            double offset = Vector3.Distance(center, defPoint);
            if (angle > aperture)
                offset *= -1;

            Angular3PointDimension entity = new Angular3PointDimension
            {
                CenterPoint = center,
                FirstPoint = firstRef,
                SecondPoint = secondRef,
                Offset = offset
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Angular2LineDimension ReadAngular2LineDimension(Vector3 defPoint)
        {
            Vector3 startFirstLine = Vector3.Zero;
            Vector3 endFirstLine = Vector3.Zero;
            Vector3 startSecondLine = Vector3.Zero;
            Vector3 arcDefinitionPoint = Vector3.Zero;

            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        startFirstLine.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        startFirstLine.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        startFirstLine.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        endFirstLine.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        endFirstLine.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        endFirstLine.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 15:
                        startSecondLine.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        startSecondLine.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 35:
                        startSecondLine.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 16:
                        arcDefinitionPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 26:
                        arcDefinitionPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 36:
                        arcDefinitionPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Angular2LineDimension entity = new Angular2LineDimension
            {
                StartFirstLine = startFirstLine,
                EndFirstLine = endFirstLine,
                StartSecondLine = startSecondLine,
                EndSecondLine = defPoint,
                Offset = Vector3.Distance(startSecondLine, defPoint),
                ArcDefinitionPoint = arcDefinitionPoint
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private OrdinateDimension ReadOrdinateDimension(Vector3 defPoint, OrdinateDimensionAxis axis, Vector3 normal, double rotation)
        {
            Vector3 firstPoint = Vector3.Zero;
            Vector3 secondPoint = Vector3.Zero;

            List<XData> xData = new List<XData>();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        firstPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        firstPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        firstPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        secondPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        secondPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        secondPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 localPoint = MathHelper.Transform(defPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 refCenter = new Vector2(localPoint.X, localPoint.Y);

            localPoint = MathHelper.Transform(firstPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 firstRef = MathHelper.Transform(new Vector2(localPoint.X, localPoint.Y) - refCenter, rotation*MathHelper.DegToRad, CoordinateSystem.World, CoordinateSystem.Object);

            localPoint = MathHelper.Transform(secondPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 secondRef = MathHelper.Transform(new Vector2(localPoint.X, localPoint.Y) - refCenter, rotation*MathHelper.DegToRad, CoordinateSystem.World, CoordinateSystem.Object);

            double length = axis == OrdinateDimensionAxis.X ? secondRef.Y - firstRef.Y : secondRef.X - firstRef.X;

            OrdinateDimension entity = new OrdinateDimension
            {
                Origin = defPoint,
                ReferencePoint = firstRef,
                Length = length,
                Rotation = rotation,
                Axis = axis
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Ellipse ReadEllipse()
        {
            Vector3 center = Vector3.Zero;
            Vector3 axisPoint = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            double[] param = new double[2];
            double ratio = 0.0;

            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        axisPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        axisPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        axisPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        ratio = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        param[0] = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 42:
                        param[1] = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Vector3 ocsAxisPoint = MathHelper.Transform(axisPoint,
                normal,
                CoordinateSystem.World,
                CoordinateSystem.Object);
            double rotation = Vector2.Angle(new Vector2(ocsAxisPoint.X, ocsAxisPoint.Y));

            double majorAxis = 2*axisPoint.Modulus();
            double minorAxis = majorAxis*ratio;

            Ellipse ellipse = new Ellipse
            {
                MajorAxis = majorAxis,
                MinorAxis = minorAxis,
                Rotation = rotation*MathHelper.RadToDeg,
                Center = center,
                Normal = normal
            };

            ellipse.XData.AddRange(xData);

            this.SetEllipseParameters(ellipse, param);
            return ellipse;
        }

        private Point ReadPoint()
        {
            Vector3 location = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            double thickness = 0.0;
            double rotation = 0.0;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        location.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        location.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        location.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = 360.0 - this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            Point entity = new Point
            {
                Position = location,
                Thickness = thickness,
                Rotation = rotation,
                Normal = normal
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Face3d ReadFace3d()
        {
            Vector3 v0 = Vector3.Zero;
            Vector3 v1 = Vector3.Zero;
            Vector3 v2 = Vector3.Zero;
            Vector3 v3 = Vector3.Zero;
            Face3dEdgeFlags flags = Face3dEdgeFlags.Visibles;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        v0.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        v0.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        v0.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        v1.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        v1.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        v1.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        v2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        v2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        v2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        v3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        v3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        v3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (Face3dEdgeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }
            
            Face3d entity = new Face3d
            {
                FirstVertex = v0,
                SecondVertex = v1,
                ThirdVertex = v2,
                FourthVertex = v3,
                EdgeFlags = flags
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Solid ReadSolid()
        {
            Vector3 v0 = Vector3.Zero;
            Vector3 v1 = Vector3.Zero;
            Vector3 v2 = Vector3.Zero;
            Vector3 v3 = Vector3.Zero;
            double thickness = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        v0.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        v0.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        v0.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        v1.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        v1.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        v1.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        v2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        v2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        v2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        v3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        v3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        v3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            Solid entity = new Solid
            {
                FirstVertex = new Vector2(v0.X, v0.Y),
                SecondVertex = new Vector2(v1.X, v1.Y),
                ThirdVertex = new Vector2(v2.X, v2.Y),
                FourthVertex = new Vector2(v3.X, v3.Y),
                Elevation = v0.Z,
                Thickness = thickness,
                Normal = normal
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Trace ReadTrace()
        {
            Vector3 v0 = Vector3.Zero;
            Vector3 v1 = Vector3.Zero;
            Vector3 v2 = Vector3.Zero;
            Vector3 v3 = Vector3.Zero;
            double thickness = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        v0.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        v0.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        v0.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        v1.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        v1.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        v1.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        v2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        v2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        v2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        v3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        v3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        v3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            Trace entity = new Trace
            {
                FirstVertex = new Vector2(v0.X, v0.Y),
                SecondVertex = new Vector2(v1.X, v1.Y),
                ThirdVertex = new Vector2(v2.X, v2.Y),
                FourthVertex = new Vector2(v3.X, v3.Y),
                Elevation = v0.Z,
                Thickness = thickness,
                Normal = normal
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Spline ReadSpline()
        {
            SplineTypeFlags flags = SplineTypeFlags.None;
            Vector3 normal = Vector3.UnitZ;
            short degree = 3;
            int ctrlPointIndex = -1;

            List<double> knots = new List<double>();
            List<SplineVertex> ctrlPoints = new List<SplineVertex>();
            double ctrlX = 0;
            double ctrlY = 0;
            double ctrlZ;
            double ctrlWeigth = -1;

            // tolerances (not used)
            double knotTolerance = 0.0000001;
            double ctrlPointTolerance = 0.0000001;
            double fitTolerance = 0.0000000001;

            // start and end tangents
            double stX = 0;
            double stY = 0;
            double stZ;
            Vector3? startTangent = null;
            double etX = 0;
            double etY = 0;
            double etZ;
            Vector3? endTangent = null;

            // fit points variable
            List<Vector3> fitPoints = new List<Vector3>();
            double fitX = 0;
            double fitY = 0;
            double fitZ;

            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (SplineTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        degree = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 72:
                        // the spline entity can actually hold a larger number of knots, we cannot use this information it might be wrong.
                        //short numKnots = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 73:
                        // the spline entity can actually hold a larger number of control points
                        //short numCtrlPoints = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 74:
                        // the spline entity can actually hold a larger number of fit points, we cannot use this information it might be wrong.
                        //numFitPoints = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 42:
                        knotTolerance = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        ctrlPointTolerance = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 44:
                        fitTolerance = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        stX = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        stY = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        stZ = this.chunk.ReadDouble();
                        startTangent = new Vector3(stX, stY, stZ);
                        this.chunk.Next();
                        break;
                    case 13:
                        etX = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        etY = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        etZ = this.chunk.ReadDouble();
                        endTangent = new Vector3(etX, etY, etZ);
                        this.chunk.Next();
                        break;
                    case 40:
                        // multiple code 40 entries, one per knot value
                        knots.Add(this.chunk.ReadDouble());
                        this.chunk.Next();
                        break;
                    case 10:
                        ctrlX = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        ctrlY = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        ctrlZ = this.chunk.ReadDouble();
                        if (ctrlWeigth <= 0)
                        {
                            ctrlPoints.Add(new SplineVertex(ctrlX, ctrlY, ctrlZ));
                            ctrlPointIndex = ctrlPoints.Count - 1;
                        }
                        else
                        {
                            ctrlPoints.Add(new SplineVertex(ctrlX, ctrlY, ctrlZ, ctrlWeigth));
                            ctrlPointIndex = -1;
                        }
                        this.chunk.Next();
                        break;
                    case 41:
                        // code 41 might appear before or after the control point coordinates.
                        double weigth = this.chunk.ReadDouble();
                        if (weigth <= 0.0) weigth = 1.0;

                        if (ctrlPointIndex == -1)
                        {
                            ctrlWeigth = weigth;
                        }
                        else
                        {
                            ctrlPoints[ctrlPointIndex].Weigth = weigth;
                            ctrlWeigth = -1;
                        }
                        this.chunk.Next();
                        break;
                    case 11:
                        fitX = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        fitY = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        fitZ = this.chunk.ReadDouble();
                        fitPoints.Add(new Vector3(fitX, fitY, fitZ));
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            SplineCreationMethod method = (flags & SplineTypeFlags.FitPointCreationMethod) == SplineTypeFlags.FitPointCreationMethod ? SplineCreationMethod.FitPoints : SplineCreationMethod.ControlPoints;
            bool isPeriodic = ((flags & SplineTypeFlags.ClosedPeriodicSpline) == SplineTypeFlags.ClosedPeriodicSpline) || 
                              ((flags & SplineTypeFlags.Periodic) == SplineTypeFlags.Periodic);
            Spline entity = new Spline(ctrlPoints, knots, degree, fitPoints, method, isPeriodic)
            {
                KnotTolerance = knotTolerance,
                CtrlPointTolerance = ctrlPointTolerance,
                FitTolerance = fitTolerance,
                StartTangent = startTangent,
                EndTangent = endTangent
            };


            if ((flags & SplineTypeFlags.FitChord) == SplineTypeFlags.FitChord)
                entity.KnotParameterization = SplineKnotParameterization.FitChord;
            else if ((flags & SplineTypeFlags.FitSqrtChord) == SplineTypeFlags.FitSqrtChord)
                entity.KnotParameterization = SplineKnotParameterization.FitSqrtChord;
            else if ((flags & SplineTypeFlags.FitUniform) == SplineTypeFlags.FitUniform)
                entity.KnotParameterization = SplineKnotParameterization.FitUniform;
            else if ((flags & SplineTypeFlags.FitCustom) == SplineTypeFlags.FitCustom)
                entity.KnotParameterization = SplineKnotParameterization.FitCustom;

            entity.XData.AddRange(xData);

            return entity;
        }

        private Insert ReadInsert(bool isBlockEntity = false)
        {
            Vector3 basePoint = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            Vector3 scale = new Vector3(1.0, 1.0, 1.0);
            double rotation = 0.0;
            string blockName = null;
            Block block = null;
            List<Attribute> attributes = new List<Attribute>();
            List<XData> xData = new List<XData>();

            Insert insert = new Insert();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        blockName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity) block = this.GetBlock(blockName);
                        this.chunk.Next();
                        break;
                    case 10:
                        basePoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        basePoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        basePoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        scale.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 42:
                        scale.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        scale.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            if (this.chunk.ReadString() == DxfObjectCode.Attribute)
            {
                while (this.chunk.ReadString() != DxfObjectCode.EndSequence)
                {
                    Attribute attribute = this.ReadAttribute(block, isBlockEntity);
                    if (attribute != null)
                    {
                        attribute.Owner = insert;
                        attributes.Add(attribute);
                    }
                }
            }

            string endSequenceHandle = string.Empty;
            if (this.chunk.ReadString() == DxfObjectCode.EndSequence)
            {
                // read the end sequence object until a new element is found
                this.chunk.Next();
                while (this.chunk.Code != 0)
                {
                    switch (this.chunk.Code)
                    {
                        case 5:
                            endSequenceHandle = this.chunk.ReadString();
                            this.chunk.Next();
                            break;
                        case 8:
                            // the EndSquence layer and the Insert layer are the same
                            this.chunk.Next();
                            break;
                        default:
                            this.chunk.Next();
                            break;
                    }
                }
            }

            // It is a lot more intuitive to give the position in world coordinates and then define the orientation with the normal.
            Vector3 wcsBasePoint = MathHelper.Transform(basePoint, normal, CoordinateSystem.Object, CoordinateSystem.World);

            insert.Block = block;
            insert.Position = wcsBasePoint;
            insert.Rotation = rotation;
            insert.Scale = scale;
            insert.Normal = normal;
            insert.Attributes = new AttributeCollection(attributes);
            insert.EndSequence.Handle = endSequenceHandle;
            insert.XData.AddRange(xData);

            // post process nested inserts
            if (isBlockEntity)
                this.nestedInserts.Add(insert, blockName);

            return insert;
        }

        private Line ReadLine()
        {
            Vector3 start = Vector3.Zero;
            Vector3 end = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            double thickness = 0.0;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        start.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        start.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        start.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        end.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        end.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        end.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }
            
            Line entity = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Normal = normal,
                Thickness = thickness
            };

            entity.XData.AddRange(xData);

            return entity;

        }

        private Ray ReadRay()
        {
            Vector3 origin = Vector3.Zero;
            Vector3 direction = Vector3.UnitX;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        origin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        origin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        origin.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        direction.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        direction.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        direction.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            Ray entity = new Ray
            {
                Origin = origin,
                Direction = direction
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private XLine ReadXLine()
        {
            Vector3 origin = Vector3.Zero;
            Vector3 direction = Vector3.UnitX;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        origin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        origin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        origin.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        direction.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        direction.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        direction.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            XLine entity = new XLine
            {
                Origin = origin,
                Direction = direction
            };

            entity.XData.AddRange(xData);

            return entity;

        }

        private MLine ReadMLine()
        {
            string styleName = null;
            double scale = 1.0;
            MLineJustification justification = MLineJustification.Zero;
            MLineFlags flags = MLineFlags.Has;
            int numVertexes = 0;
            int numStyleElements = 0;
            double elevation = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<MLineVertex> segments = new List<MLineVertex>();
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        // the MLineStyle is defined in the objects sections after the definition of the entity, something similar happens with the image entity
                        // the MLineStyle will be applied to the MLine after parsing the whole file
                        styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.CMLStyle;
                        this.chunk.Next();
                        break;
                    case 40:
                        scale = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        justification = (MLineJustification) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        flags = (MLineFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 72:
                        numVertexes = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 73:
                        numStyleElements = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 10:
                        // this info is not needed it is repeated in the vertexes list
                        this.chunk.Next();
                        break;
                    case 20:
                        // this info is not needed it is repeated in the vertexes list
                        this.chunk.Next();
                        break;
                    case 30:
                        // this info is not needed it is repeated in the vertexes list
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        // the info that follows contains the information on the vertexes of the MLine
                        segments = this.ReadMLineSegments(numVertexes, numStyleElements, normal, out elevation);
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");
                        this.chunk.Next();
                        break;
                }
            }

            MLine mline = new MLine
            {
                Elevation = elevation,
                Scale = scale,
                Justification = justification,
                Normal = normal,
                Flags = flags
            };
            mline.Vertexes.AddRange(segments);
            mline.XData.AddRange(xData);

            // save the referenced style name for post processing
            this.mLineToStyleNames.Add(mline, styleName);

            return mline;
        }

        private List<MLineVertex> ReadMLineSegments(int numVertexes, int numStyleElements, Vector3 normal, out double elevation)
        {
            elevation = 0.0;

            List<MLineVertex> segments = new List<MLineVertex>();
            Matrix3 trans = MathHelper.ArbitraryAxis(normal).Traspose();
            for (int i = 0; i < numVertexes; i++)
            {
                Vector3 vertex = new Vector3();
                vertex.X = this.chunk.ReadDouble(); // code 11
                this.chunk.Next();
                vertex.Y = this.chunk.ReadDouble(); // code 21
                this.chunk.Next();
                vertex.Z = this.chunk.ReadDouble(); // code 31
                this.chunk.Next();

                Vector3 dir = new Vector3();
                dir.X = this.chunk.ReadDouble(); // code 12
                this.chunk.Next();
                dir.Y = this.chunk.ReadDouble(); // code 22
                this.chunk.Next();
                dir.Z = this.chunk.ReadDouble(); // code 32
                this.chunk.Next();

                Vector3 mitter = new Vector3();
                mitter.X = this.chunk.ReadDouble(); // code 13
                this.chunk.Next();
                mitter.Y = this.chunk.ReadDouble(); // code 23
                this.chunk.Next();
                mitter.Z = this.chunk.ReadDouble(); // code 33
                this.chunk.Next();

                List<double>[] distances = new List<double>[numStyleElements];
                for (int j = 0; j < numStyleElements; j++)
                {
                    distances[j] = new List<double>();
                    short numDistances = this.chunk.ReadShort(); // code 74
                    this.chunk.Next();
                    for (short k = 0; k < numDistances; k++)
                    {
                        distances[j].Add(this.chunk.ReadDouble()); // code 41
                        this.chunk.Next();
                    }

                    // no more info is needed, fill parameters are not supported
                    short numFillParams = this.chunk.ReadShort(); // code 75
                    this.chunk.Next();
                    for (short k = 0; k < numFillParams; k++)
                    {
                        //double param = this.chunk.ReadDouble(); // code 42
                        this.chunk.Next();
                    }
                }

                // we need to convert WCS coordinates to OCS coordinates
                if (!normal.Equals(Vector3.UnitZ))
                {
                    vertex = trans*vertex;
                    dir = trans*dir;
                    mitter = trans*mitter;
                }

                MLineVertex segment = new MLineVertex(new Vector2(vertex.X, vertex.Y),
                    new Vector2(dir.X, dir.Y),
                    new Vector2(mitter.X, mitter.Y),
                    distances);
                elevation = vertex.Z;
                segments.Add(segment);
            }

            return segments;
        }

        private LwPolyline ReadLwPolyline()
        {
            double elevation = 0.0;
            double thickness = 0.0;
            PolylineTypeFlags flags = PolylineTypeFlags.OpenPolyline;
            double constantWidth = -1.0;
            List<LwPolylineVertex> polVertexes = new List<LwPolylineVertex>();
            LwPolylineVertex v = null;
            double vX = 0.0;
            Vector3 normal = Vector3.UnitZ;

            List<XData> xData = new List<XData>();

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 38:
                        elevation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        // constant width (optional; default = 0). If present it will override any vertex width (codes 40 and/or 41)
                        constantWidth = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (PolylineTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 90:
                        //numVertexes = int.Parse(code.Value);
                        this.chunk.Next();
                        break;
                    case 10:
                        vX = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        double vY = this.chunk.ReadDouble();
                        v = new LwPolylineVertex(vX, vY);     
                        polVertexes.Add(v);
                        this.chunk.Next();
                        break;
                    case 40:
                        double startWidth = this.chunk.ReadDouble();
                        if(v!=null)
                            if (startWidth >= 0.0) v.StartWidth = startWidth;
                        this.chunk.Next();
                        break;
                    case 41:
                        double endWidth = this.chunk.ReadDouble();
                        if (v != null)
                            if (endWidth >= 0.0) v.EndWidth = endWidth;
                        this.chunk.Next();
                        break;
                    case 42:
                        if (v != null)
                            v.Bulge = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            LwPolyline entity = new LwPolyline
            {
                Elevation = elevation,
                Thickness = thickness,
                Flags = flags,
                Normal = normal
            };

            entity.Vertexes.AddRange(polVertexes);

            if (constantWidth >= 0.0)
                entity.SetConstantWidth(constantWidth);

            entity.XData.AddRange(xData);

            return entity;

        }

        private EntityObject ReadPolyline()
        {
            // the entity Polyline in dxf can actually hold three kinds of entities
            // 3d polyline is the generic polyline
            // polyface mesh
            // polylines 2d is the old way of writing polylines the AutoCAD2000 and newer always use LwPolylines to define a 2d polyline
            // this way of reading 2d polylines is here for compatibility reasons with older dxf versions.
            PolylineTypeFlags flags = PolylineTypeFlags.OpenPolyline;
            PolylineSmoothType smoothType = PolylineSmoothType.NoSmooth; 
            double elevation = 0.0;
            double thickness = 0.0;
            Vector3 normal = Vector3.UnitZ;
            List<Vertex> vertexes = new List<Vertex>();
            List<XData> xData = new List<XData>();

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                        elevation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 39:
                        thickness = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (PolylineTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 75:
                        smoothType = (PolylineSmoothType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        //this field might not exist for polyface meshes, we cannot depend on it
                        //numVertexes = int.Parse(code.Value); code = this.ReadCodePair();
                        this.chunk.Next();
                        break;
                    case 72:
                        //this field might not exist for polyface meshes, we cannot depend on it
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            //begin to read the vertex list (although it is not recommended the vertex list might have 0 entries)
            while (this.chunk.ReadString() != DxfObjectCode.EndSequence)
            {
                if (this.chunk.ReadString() == DxfObjectCode.Vertex)
                {
                    Vertex vertex = this.ReadVertex();
                    vertexes.Add(vertex);
                }
            }

            // read the end sequence object until a new element is found
            this.chunk.Next();
            string endSequenceHandle = null;
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        endSequenceHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 8:
                        // the polyline EndSequence layer should be the same as the polyline layer
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            EntityObject pol;
            bool isClosed = (flags & PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM) == PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;

            //to avoid possible error between the vertex type and the polyline type
            //the polyline type will decide which information to use from the read vertex
            if ((flags & PolylineTypeFlags.Polyline3D) == PolylineTypeFlags.Polyline3D)
            {
                List<PolylineVertex> polyline3dVertexes = new List<PolylineVertex>();
                foreach (Vertex v in vertexes)
                {
                    PolylineVertex vertex = new PolylineVertex
                    {
                        Flags = v.Flags,
                        Position = v.Position,
                        Handle = v.Handle,
                    };
                    polyline3dVertexes.Add(vertex);
                }

                pol = new Polyline(polyline3dVertexes, isClosed)
                {
                    Flags = flags,
                    SmoothType = smoothType,
                    Normal = normal
                };
                ((Polyline) pol).EndSequence.Handle = endSequenceHandle;
            }
            else if ((flags & PolylineTypeFlags.PolyfaceMesh) == PolylineTypeFlags.PolyfaceMesh)
            {
                //the vertex list created contains vertex and face information
                List<PolyfaceMeshVertex> polyfaceVertexes = new List<PolyfaceMeshVertex>();
                List<PolyfaceMeshFace> polyfaceFaces = new List<PolyfaceMeshFace>();
                foreach (Vertex v in vertexes)
                {
                    if ((v.Flags & (VertexTypeFlags.PolyfaceMeshVertex | VertexTypeFlags.Polygon3dMesh)) == (VertexTypeFlags.PolyfaceMeshVertex | VertexTypeFlags.Polygon3dMesh))
                    {
                        PolyfaceMeshVertex vertex = new PolyfaceMeshVertex
                        {
                            Location = v.Position,
                            Handle = v.Handle,
                        };
                        polyfaceVertexes.Add(vertex);
                    }
                    else if ((v.Flags & VertexTypeFlags.PolyfaceMeshVertex) == VertexTypeFlags.PolyfaceMeshVertex)
                    {
                        PolyfaceMeshFace vertex = new PolyfaceMeshFace
                        {
                            VertexIndexes = v.VertexIndexes,
                            Handle = v.Handle,
                        };
                        polyfaceFaces.Add(vertex);
                    }
                }
                pol = new PolyfaceMesh(polyfaceVertexes, polyfaceFaces)
                {
                    Normal = normal
                };
                ((PolyfaceMesh) pol).EndSequence.Handle = endSequenceHandle;
            }
            else
            {
                List<LwPolylineVertex> polylineVertexes = new List<LwPolylineVertex>();
                foreach (Vertex v in vertexes)
                {
                    LwPolylineVertex vertex = new LwPolylineVertex
                    {
                        Position = new Vector2(v.Position.X, v.Position.Y),
                        StartWidth = v.StartWidth,
                        Bulge = v.Bulge,
                        EndWidth = v.EndWidth,
                    };

                    polylineVertexes.Add(vertex);
                }

                pol = new LwPolyline(polylineVertexes, isClosed)
                {
                    Flags = flags,
                    Thickness = thickness,
                    Elevation = elevation,
                    Normal = normal,
                };
            }

            pol.XData.AddRange(xData);

            return pol;
        }

        private Vertex ReadVertex()
        {
            string handle = string.Empty;
            Layer layer = Layer.Default;
            AciColor color = AciColor.ByLayer;
            LineType lineType = LineType.ByLayer;
            Vector3 position = new Vector3();
            double startWidth = 0.0;
            double endWidth = 0.0;
            double bulge = 0.0;
            List<short> vertexIndexes = new List<short>();
            VertexTypeFlags flags = VertexTypeFlags.PolylineVertex;

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 8:
                        string layerName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(layerName);
                        this.chunk.Next();
                        break;
                    case 62: //ACI color code
                        if (!color.UseTrueColor)
                            color = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 420: //the entity uses true color
                        color = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 6:
                        // the line type names ByLayer or ByBlock are case insensitive
                        string lineTypeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.Equals(lineTypeName, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByLayerName;
                        if (string.Equals(lineTypeName, LineType.ByBlockName, StringComparison.OrdinalIgnoreCase))
                            lineTypeName = LineType.ByBlockName;
                        lineType = this.GetLineType(lineTypeName);
                        this.chunk.Next();
                        break;
                    case 10:
                        position.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        position.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        position.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        startWidth = this.chunk.ReadDouble();
                        if (startWidth < 0.0) startWidth = 0.0;
                        this.chunk.Next();
                        break;
                    case 41:
                        endWidth = this.chunk.ReadDouble();
                        if (endWidth < 0.0) endWidth = 0.0;
                        this.chunk.Next();
                        break;
                    case 42:
                        bulge = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (VertexTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 71:
                        vertexIndexes.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 72:
                        vertexIndexes.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 73:
                        vertexIndexes.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 74:
                        vertexIndexes.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            return new Vertex
            {
                Flags = flags,
                Position = position,
                StartWidth = startWidth,
                Bulge = bulge,
                Color = color,
                EndWidth = endWidth,
                Layer = layer,
                LineType = lineType,
                VertexIndexes = vertexIndexes.ToArray(),
                Handle = handle
            };
        }

        private Text ReadText()
        {
            string textString = string.Empty;
            double height = 0.0;
            double widthFactor = 1.0;
            double rotation = 0.0;
            double obliqueAngle = 0.0;
            TextStyle style = TextStyle.Default;
            Vector3 firstAlignmentPoint = Vector3.Zero;
            Vector3 secondAlignmentPoint = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            short horizontalAlignment = 0;
            short verticalAlignment = 0;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                        textString = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 10:
                        firstAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        firstAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        firstAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        secondAlignmentPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        secondAlignmentPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        secondAlignmentPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        if (height <= 0.0)
                            height = this.doc.DrawingVariables.TextSize;
                        this.chunk.Next();
                        break;
                    case 41:
                        widthFactor = this.chunk.ReadDouble();
                        if (widthFactor < 0.01 || widthFactor > 100.0)
                            widthFactor = this.doc.DrawingVariables.TextSize;
                        this.chunk.Next();
                        break;
                    case 50:
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 51:
                        obliqueAngle = this.chunk.ReadDouble();
                        if (obliqueAngle < -85.0 || obliqueAngle > 85.0)
                            obliqueAngle = 0.0;
                        this.chunk.Next();
                        break;
                    case 7:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.TextStyle;
                        style = this.GetTextStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 72:
                        horizontalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 73:
                        verticalAlignment = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            TextAlignment alignment = ObtainAlignment(horizontalAlignment, verticalAlignment);
            Vector3 ocsBasePoint = alignment == TextAlignment.BaselineLeft ? firstAlignmentPoint : secondAlignmentPoint;

            // another example of this OCS vs WCS non sense.
            // while the MText position is written in WCS the position of the Text is written in OCS (different rules for the same concept).
            textString = this.DecodeEncodedNonAsciiCharacters(textString);
            Text text = new Text
            {
                Value = textString,
                Height = height,
                WidthFactor = widthFactor,
                Rotation = rotation,
                ObliqueAngle = obliqueAngle,
                Style = style,
                Position = MathHelper.Transform(ocsBasePoint, normal, CoordinateSystem.Object, CoordinateSystem.World),
                Normal = normal,
                Alignment = alignment
            };

            text.XData.AddRange(xData);

            return text;
        }

        private MText ReadMText()
        {
            Vector3 insertionPoint = Vector3.Zero;
            Vector2 direction = Vector2.UnitX;
            Vector3 normal = Vector3.UnitZ;
            double height = 0.0;
            double rectangleWidth = 0.0;
            double lineSpacing = 1.0;
            double rotation = 0.0;
            bool isRotationDefined = false;
            MTextAttachmentPoint attachmentPoint = MTextAttachmentPoint.TopLeft;
            TextStyle style = TextStyle.Default;
            string textString = string.Empty;
            List<XData> xData = new List<XData>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                        textString = string.Concat(textString, this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 3:
                        textString = string.Concat(textString, this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 10:
                        insertionPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        insertionPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 30:
                        insertionPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        direction.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        direction.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 31:
                        // Z direction value (direction.Z = double.Parse(dxfPairInfo.Value);)
                        // the angle of the text is defined on the plane where it belongs so Z value will be zero.
                        this.chunk.Next();
                        break;
                    case 40:
                        height = this.chunk.ReadDouble();
                        if (height <= 0.0)
                            height = this.doc.DrawingVariables.TextSize;
                        this.chunk.Next();
                        break;
                    case 41:
                        rectangleWidth = this.chunk.ReadDouble();
                        if (rectangleWidth < 0.0)
                            rectangleWidth = 0.0;
                        this.chunk.Next();
                        break;
                    case 44:
                        lineSpacing = this.chunk.ReadDouble();
                        if (lineSpacing < 0.25 || lineSpacing > 4.0)
                            lineSpacing = 1.0;
                        this.chunk.Next();
                        break;
                    case 50: // even if the AutoCAD dxf documentation says that the rotation is in radians, this is wrong this value is in degrees
                        isRotationDefined = true;
                        rotation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 7:
                        string styleName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(styleName))
                            styleName = this.doc.DrawingVariables.TextStyle;
                        style = this.GetTextStyle(styleName);
                        this.chunk.Next();
                        break;
                    case 71:
                        attachmentPoint = (MTextAttachmentPoint) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            textString = this.DecodeEncodedNonAsciiCharacters(textString);
            // text dxf files stores the tabs as ^I in the MText texts, they will be replaced by the standard tab character
            if(!this.isBinary) textString = textString.Replace("^I", "\t");

            MText entity = new MText
            {
                Value = textString,
                Position = insertionPoint,
                Height = height,
                RectangleWidth = rectangleWidth,
                Style = style,
                LineSpacingFactor = lineSpacing,
                AttachmentPoint = attachmentPoint,
                Rotation = isRotationDefined ? rotation : Vector2.Angle(direction)*MathHelper.RadToDeg,
                Normal = normal,
            };

            entity.XData.AddRange(xData);

            return entity;
        }

        private Hatch ReadHatch()
        {
            string name = string.Empty;
            HatchFillType fill = HatchFillType.SolidFill;
            double elevation = 0.0;
            Vector3 normal = Vector3.UnitZ;
            HatchPattern pattern = HatchPattern.Line;
            bool associative = false;
            List<HatchBoundaryPath> paths = new List<HatchBoundaryPath>();
            List<XData> xData = new List<XData>();

            this.chunk.Next();

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 30:
                        elevation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 210:
                        normal.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 220:
                        normal.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 230:
                        normal.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 91:
                        // the next lines hold the information about the hatch boundary paths
                        int numPaths = this.chunk.ReadInt();
                        paths = this.ReadHatchBoundaryPaths(numPaths);
                        break;
                    case 70:
                        // Solid fill flag
                        fill = (HatchFillType) this.chunk.ReadShort();
                        //if (fill == HatchFillType.SolidFill) name = PredefinedHatchPatternName.Solid;
                        this.chunk.Next();
                        break;
                    case 71:
                        // Associativity flag (associative = 1; non-associative = 0); for MPolygon, solid-fill flag (has solid fill = 1; lacks solid fill = 0)
                        if (this.chunk.ReadShort() != 0) associative = true;
                        this.chunk.Next();
                        break;
                    case 75:
                        // the next lines hold the information about the hatch pattern
                        pattern = this.ReadHatchPattern(name);
                        pattern.Fill = fill;
                        break;
                    case 1001:
                        string appId = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(appId);
                        xData.Add(data);
                        break;
                    default:
                        if (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
                            throw new DxfInvalidCodeValueEntityException(this.chunk.Code, this.chunk.ReadString(),
                                "The extended data of an entity must start with the application registry code.");

                        this.chunk.Next();
                        break;
                }
            }

            if (paths.Count == 0) return null;

            Hatch entity = new Hatch(pattern, new List<HatchBoundaryPath>(), associative)
            {
                Elevation = elevation,
                Normal = normal
            };

            this.hatchToPaths.Add(entity, paths);

            entity.XData.AddRange(xData);

            // here is where dxf stores the pattern origin
            XData patternOrigin;
            Vector2 origin = Vector2.Zero;
            if (entity.XData.TryGetValue(ApplicationRegistry.Default.Name, out patternOrigin))
            {
                foreach (XDataRecord record in patternOrigin.XDataRecord)
                {
                    if (record.Code == XDataCode.RealX)
                        origin.X = (double)record.Value;
                    else if (record.Code == XDataCode.RealY)
                        origin.Y = (double)record.Value;
                    // record.Code == XDataCode.RealZ is always 0
                }
            }
            pattern.Origin = origin;

            return entity;
        }

        private List<HatchBoundaryPath> ReadHatchBoundaryPaths(int numPaths)
        {
            List<HatchBoundaryPath> paths = new List<HatchBoundaryPath>();
            HatchBoundaryPathTypeFlags pathTypeFlag = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;

            this.chunk.Next();
            while (paths.Count < numPaths)
            {
                HatchBoundaryPath path;
                switch (this.chunk.Code)
                {
                    case 92:
                        pathTypeFlag = (HatchBoundaryPathTypeFlags) this.chunk.ReadInt();
                        // adding External and Derived to all path type flags solves an strange problem with code 98 not found,
                        // it seems related to the code 47 that appears before, only some combinations of flags are affected
                        // this is what the documentation says about code 47:
                        // Pixel size used to determine the density to perform various intersection and ray casting operations
                        // in hatch pattern computation for associative hatches and hatches created with the Flood method of hatching
                        pathTypeFlag = pathTypeFlag | HatchBoundaryPathTypeFlags.External | HatchBoundaryPathTypeFlags.Derived;

                        if ((pathTypeFlag & HatchBoundaryPathTypeFlags.Polyline) == HatchBoundaryPathTypeFlags.Polyline)
                        {
                            path = this.ReadEdgePolylineBoundaryPath();
                            path.PathTypeFlag = pathTypeFlag;
                            paths.Add(path);
                        }
                        else
                            this.chunk.Next();
                        break;
                    case 93:
                        int numEdges = this.chunk.ReadInt();
                        path = this.ReadEdgeBoundaryPath(numEdges);
                        path.PathTypeFlag = pathTypeFlag;
                        paths.Add(path);
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }
            return paths;
        }

        private HatchBoundaryPath ReadEdgePolylineBoundaryPath()
        {
            HatchBoundaryPath.Polyline poly = new HatchBoundaryPath.Polyline();

            this.chunk.Next();

            bool hasBulge = this.chunk.ReadShort() != 0; // code 72
            this.chunk.Next();

            // is polyline closed
            poly.IsClosed = this.chunk.ReadShort() != 0; // code 73
            this.chunk.Next();

            int numVertexes = this.chunk.ReadInt(); // code 93
            poly.Vertexes = new Vector3[numVertexes];
            this.chunk.Next();

            for (int i = 0; i < numVertexes; i++)
            {
                double bulge = 0.0;
                double x = this.chunk.ReadDouble(); // code 10
                this.chunk.Next();
                double y = this.chunk.ReadDouble(); // code 20
                this.chunk.Next();
                if (hasBulge)
                {
                    bulge = this.chunk.ReadDouble(); // code 42
                    this.chunk.Next();
                }
                poly.Vertexes[i] = new Vector3(x, y, bulge);
            }
            HatchBoundaryPath path = new HatchBoundaryPath(new List<HatchBoundaryPath.Edge> { poly });

            // read all referenced entities
            Debug.Assert(this.chunk.Code == 97, "The reference count code 97 was expected.");
            int numBoundaryObjects = this.chunk.ReadInt();
            this.hatchContourns.Add(path, new List<string>(numBoundaryObjects));
            this.chunk.Next();
            for (int i = 0; i < numBoundaryObjects; i++)
            {
                Debug.Assert(this.chunk.Code == 330, "The reference handle code 330 was expected.");
                this.hatchContourns[path].Add(this.chunk.ReadString());
                this.chunk.Next();
            }

            return path;
        }

        private HatchBoundaryPath ReadEdgeBoundaryPath(int numEdges)
        {
            // the information of the boundary path data always appear exactly as it is read
            List<HatchBoundaryPath.Edge> entities = new List<HatchBoundaryPath.Edge>();
            this.chunk.Next();

            while (entities.Count < numEdges)
            {
                // Edge type (only if boundary is not a polyline): 1 = Line; 2 = Circular arc; 3 = Elliptic arc; 4 = Spline
                if (this.chunk.Code != 72) throw new ArgumentException();
                HatchBoundaryPath.EdgeType type = (HatchBoundaryPath.EdgeType) this.chunk.ReadShort();
                switch (type)
                {
                    case HatchBoundaryPath.EdgeType.Line:
                        this.chunk.Next();
                        // line
                        double lX1 = this.chunk.ReadDouble(); // code 10
                        this.chunk.Next();
                        double lY1 = this.chunk.ReadDouble(); // code 20
                        this.chunk.Next();
                        double lX2 = this.chunk.ReadDouble(); // code 11
                        this.chunk.Next();
                        double lY2 = this.chunk.ReadDouble(); // code 21
                        this.chunk.Next();

                        HatchBoundaryPath.Line line = new HatchBoundaryPath.Line
                        {
                            Start = new Vector2(lX1, lY1),
                            End = new Vector2(lX2, lY2)
                        };
                        entities.Add(line);
                        break;
                    case HatchBoundaryPath.EdgeType.Arc:
                        this.chunk.Next();
                        // circular arc
                        double aX = this.chunk.ReadDouble(); // code 10
                        this.chunk.Next();
                        double aY = this.chunk.ReadDouble(); // code 40
                        this.chunk.Next();
                        double aR = this.chunk.ReadDouble(); // code 40
                        this.chunk.Next();
                        double aStart = this.chunk.ReadDouble(); // code 50
                        this.chunk.Next();
                        double aEnd = this.chunk.ReadDouble(); // code 51
                        this.chunk.Next();
                        bool aCCW = this.chunk.ReadShort() != 0; // code 73
                        this.chunk.Next();

                        HatchBoundaryPath.Arc arc = new HatchBoundaryPath.Arc
                        {
                            Center = new Vector2(aX, aY),
                            Radius = aR,
                            StartAngle = aStart,
                            EndAngle = aEnd,
                            IsCounterclockwise = aCCW
                        };
                        entities.Add(arc);
                        break;
                    case HatchBoundaryPath.EdgeType.Ellipse:
                        this.chunk.Next();
                        // elliptic arc
                        double eX = this.chunk.ReadDouble(); // code 10
                        this.chunk.Next();
                        double eY = this.chunk.ReadDouble(); // code 20
                        this.chunk.Next();
                        double eAxisX = this.chunk.ReadDouble(); // code 11
                        this.chunk.Next();
                        double eAxisY = this.chunk.ReadDouble(); // code 21
                        this.chunk.Next();
                        double eAxisRatio = this.chunk.ReadDouble(); // code 40
                        this.chunk.Next();
                        double eStart = this.chunk.ReadDouble(); // code 50
                        this.chunk.Next();
                        double eEnd = this.chunk.ReadDouble(); // code 51
                        this.chunk.Next();
                        bool eCCW = this.chunk.ReadShort() != 0; // code 73
                        this.chunk.Next();

                        HatchBoundaryPath.Ellipse ellipse = new HatchBoundaryPath.Ellipse
                        {
                            Center = new Vector2(eX, eY),
                            EndMajorAxis = new Vector2(eAxisX, eAxisY),
                            MinorRatio = eAxisRatio,
                            StartAngle = eStart,
                            EndAngle = eEnd,
                            IsCounterclockwise = eCCW
                        };

                        entities.Add(ellipse);
                        break;
                    case HatchBoundaryPath.EdgeType.Spline:
                        this.chunk.Next();
                        // spline

                        short degree = (short) this.chunk.ReadInt(); // code 94
                        this.chunk.Next();

                        bool isRational = this.chunk.ReadShort() != 0; // code 73
                        this.chunk.Next();

                        bool isPeriodic = this.chunk.ReadShort() != 0; // code 74
                        this.chunk.Next();

                        int numKnots = this.chunk.ReadInt(); // code 95
                        double[] knots = new double[numKnots];
                        this.chunk.Next();

                        int numControlPoints = this.chunk.ReadInt(); // code 96
                        Vector3[] controlPoints = new Vector3[numControlPoints];
                        this.chunk.Next();

                        for (int i = 0; i < numKnots; i++)
                        {
                            knots[i] = this.chunk.ReadDouble(); // code 40
                            this.chunk.Next();
                        }

                        for (int i = 0; i < numControlPoints; i++)
                        {
                            double x = this.chunk.ReadDouble(); // code 10
                            this.chunk.Next();

                            double y = this.chunk.ReadDouble(); // code 20
                            this.chunk.Next();

                            // control point weight might not be present
                            double w = 1.0;
                            if (this.chunk.Code == 42)
                            {
                                w = this.chunk.ReadDouble(); // code 42
                                this.chunk.Next();
                            }

                            controlPoints[i] = new Vector3(x, y, w);
                        }

                        // this information is only required for AutoCAD version 2010 and newer
                        // stores information about spline fit point (the spline entity does not make use of this information)
                        if (this.doc.DrawingVariables.AcadVer >= DxfVersion.AutoCad2010)
                        {
                            int numFitData = this.chunk.ReadInt(); // code 97
                            this.chunk.Next();
                            for (int i = 0; i < numFitData; i++)
                            {
                                double fitX = this.chunk.ReadDouble(); // code 11
                                this.chunk.Next();
                                double fitY = this.chunk.ReadDouble(); // code 21
                                this.chunk.Next();
                            }

                            // the info on start tangent might not appear
                            if (this.chunk.Code == 12)
                            {
                                double startTanX = this.chunk.ReadDouble(); // code 12
                                this.chunk.Next();
                                double startTanY = this.chunk.ReadDouble(); // code 22
                                this.chunk.Next();
                            }
                            // the info on end tangent might not appear
                            if (this.chunk.Code == 13)
                            {
                                double endTanX = this.chunk.ReadDouble(); // code 13
                                this.chunk.Next();
                                double endTanY = this.chunk.ReadDouble(); // code 23
                                this.chunk.Next();
                            }
                        }

                        HatchBoundaryPath.Spline spline = new HatchBoundaryPath.Spline
                        {
                            Degree = degree,
                            IsPeriodic = isPeriodic,
                            IsRational = isRational,
                            ControlPoints = controlPoints,
                            Knots = knots
                        };

                        entities.Add(spline);
                        break;
                }
            }

            HatchBoundaryPath path = new HatchBoundaryPath(entities);

            // read all referenced entities
            Debug.Assert(this.chunk.Code == 97, "The reference count code 97 was expected.");
            int numBoundaryObjects = this.chunk.ReadInt();
            this.hatchContourns.Add(path, new List<string>(numBoundaryObjects));
            this.chunk.Next();
            for (int i = 0; i < numBoundaryObjects; i++)
            {
                Debug.Assert(this.chunk.Code == 330, "The reference handle code 330 was expected.");
                this.hatchContourns[path].Add(this.chunk.ReadString());
                this.chunk.Next();
            }

            return path;
        }

        private HatchPattern ReadHatchPattern(string name)
        {
            HatchPattern hatch = null;
            double angle = 0.0;
            double scale = 1.0;
            bool isGradient = false;
            List<HatchPatternLineDefinition> lineDefinitions = new List<HatchPatternLineDefinition>();
            HatchType type = HatchType.UserDefined;
            HatchStyle style = HatchStyle.Normal;

            while (this.chunk.Code != 0 && this.chunk.Code != 1001)
            {
                switch (this.chunk.Code)
                {
                    case 52:
                        angle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        scale = this.chunk.ReadDouble();
                        if (scale <= 0)
                            scale = 1.0;
                        this.chunk.Next();
                        break;
                    case 47:
                        this.chunk.Next();
                        break;
                    case 98:
                        this.chunk.Next();
                        break;
                    case 10:
                        this.chunk.Next();
                        break;
                    case 20:
                        this.chunk.Next();
                        break;
                    case 75:
                        style = (HatchStyle) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 76:
                        type = (HatchType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 77:
                        // hatch pattern double flag (not used)
                        this.chunk.Next();
                        break;
                    case 78:
                        // number of pattern definition lines
                        short numLines = this.chunk.ReadShort();
                        lineDefinitions = this.ReadHatchPatternDefinitionLine(scale, angle, numLines);
                        break;
                    case 450:
                        if (this.chunk.ReadInt() == 1)
                        {
                            isGradient = true; // gradient pattern
                            hatch = this.ReadHatchGradientPattern();
                        }
                        else
                            this.chunk.Next(); // solid hatch, we do not need to read anything else
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (!isGradient)
                hatch = new HatchPattern(name);

            hatch.Angle = angle;
            hatch.Style = style;
            hatch.Scale = scale;
            hatch.Type = type;
            hatch.LineDefinitions.AddRange(lineDefinitions);
            return hatch;
        }

        private HatchGradientPattern ReadHatchGradientPattern()
        {
            // the information for gradient pattern must follow an strict order
            //dxfPairInfo = this.ReadCodePair();  // code 450 not needed
            this.chunk.Next(); // code 451 not needed
            this.chunk.Next();
            double angle = this.chunk.ReadDouble(); // code 460
            this.chunk.Next();
            bool centered = (int) this.chunk.ReadDouble() == 0; // code 461
            this.chunk.Next();
            bool singleColor = this.chunk.ReadInt() != 0; // code 452
            this.chunk.Next();
            double tint = this.chunk.ReadDouble(); // code 462
            this.chunk.Next(); // code 453 not needed

            this.chunk.Next(); // code 463 not needed (0.0)
            this.chunk.Next(); // code 63
            this.chunk.Next(); // code 421
            AciColor color1 = AciColor.FromTrueColor(this.chunk.ReadInt());

            this.chunk.Next(); // code 463 not needed (1.0)
            this.chunk.Next(); // code 63
            this.chunk.Next(); // code 421
            AciColor color2 = AciColor.FromTrueColor(this.chunk.ReadInt());

            this.chunk.Next(); // code 470
            string typeName = this.chunk.ReadString();
            if (!StringEnum.IsStringDefined(typeof (HatchGradientPatternType), typeName))
                throw new DxfObjectException("HatchPatternGradient", "Unknown hatch gradient type: " + typeName);
            HatchGradientPatternType type = (HatchGradientPatternType) StringEnum.Parse(typeof (HatchGradientPatternType), typeName);

            if (singleColor)
                return new HatchGradientPattern(color1, tint, type)
                {
                    Centered = centered,
                    Angle = angle*MathHelper.RadToDeg
                };

            return new HatchGradientPattern(color1, color2, type)
            {
                Centered = centered,
                Angle = angle*MathHelper.RadToDeg
            };
        }

        private List<HatchPatternLineDefinition> ReadHatchPatternDefinitionLine(double patternScale, double patternAngle, short numLines)
        {
            List<HatchPatternLineDefinition> lineDefinitions = new List<HatchPatternLineDefinition>();

            this.chunk.Next();
            for (int i = 0; i < numLines; i++)
            {
                Vector2 origin = Vector2.Zero;
                Vector2 delta = Vector2.Zero;

                double angle = this.chunk.ReadDouble(); // code 53
                this.chunk.Next();

                origin.X = this.chunk.ReadDouble(); // code 43
                this.chunk.Next();

                origin.Y = this.chunk.ReadDouble(); // code 44
                this.chunk.Next();

                delta.X = this.chunk.ReadDouble(); // code 45
                this.chunk.Next();

                delta.Y = this.chunk.ReadDouble(); // code 46
                this.chunk.Next();

                short numSegments = this.chunk.ReadShort(); // code 79
                this.chunk.Next();

                // Pattern fill data. In theory this should hold the same information as the pat file but for unknown reason the dxf requires global data instead of local.
                // this means we have to convert the global data into local, since we are storing the pattern line definition as it appears in the acad.pat file.
                double sinOrigin = Math.Sin(patternAngle*MathHelper.DegToRad);
                double cosOrigin = Math.Cos(patternAngle*MathHelper.DegToRad);
                origin = new Vector2(cosOrigin*origin.X/patternScale + sinOrigin*origin.Y/patternScale, -sinOrigin*origin.X/patternScale + cosOrigin*origin.Y/patternScale);

                double sinDelta = Math.Sin(angle*MathHelper.DegToRad);
                double cosDelta = Math.Cos(angle*MathHelper.DegToRad);
                delta = new Vector2(cosDelta*delta.X/patternScale + sinDelta*delta.Y/patternScale, -sinDelta*delta.X/patternScale + cosDelta*delta.Y/patternScale);

                HatchPatternLineDefinition lineDefiniton = new HatchPatternLineDefinition
                {
                    Angle = angle - patternAngle,
                    Origin = origin,
                    Delta = delta,
                };

                for (int j = 0; j < numSegments; j++)
                {
                    // positive values means solid segments and negative values means spaces (one entry per element)
                    lineDefiniton.DashPattern.Add(this.chunk.ReadDouble() / patternScale); // code 49
                    this.chunk.Next();
                }

                lineDefinitions.Add(lineDefiniton);
            }

            return lineDefinitions;
        }

        private void ReadUnknowEntity()
        {
            // if the entity is unknown keep reading until an end of section or a new entity is found
            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                this.chunk.Next();
            }
        }

        #endregion

        #region object methods

        private void CreateObjectCollection(DictionaryObject namedDict)
        {
            string groupsHandle = null;
            string layoutsHandle = null;
            string mlineStylesHandle = null;
            string imageDefsHandle = null;
            string underlayDgnDefsHandle = null;
            string underlayDwfDefsHandle = null;
            string underlayPdfDefsHandle = null;
            foreach (KeyValuePair<string, string> entry in namedDict.Entries)
            {
                switch (entry.Value)
                {
                    case DxfObjectCode.GroupDictionary:
                        groupsHandle = entry.Key;
                        break;
                    case DxfObjectCode.LayoutDictionary:
                        layoutsHandle = entry.Key;
                        break;
                    case DxfObjectCode.MLineStyleDictionary:
                        mlineStylesHandle = entry.Key;
                        break;
                    case DxfObjectCode.ImageDefDictionary:
                        imageDefsHandle = entry.Key;
                        break;
                    case DxfObjectCode.UnderlayDgnDefinitionDictionary:
                        underlayDgnDefsHandle = entry.Key;
                        break;
                    case DxfObjectCode.UnderlayDwfDefinitionDictionary:
                        underlayDwfDefsHandle = entry.Key;
                        break;
                    case DxfObjectCode.UnderlayPdfDefinitionDictionary:
                        underlayPdfDefsHandle = entry.Key;
                        break;
                }
            }

            // create the collections with the provided handles
            this.doc.Groups = new Groups(this.doc, groupsHandle);
            this.doc.Layouts = new Layouts(this.doc, layoutsHandle);
            this.doc.MlineStyles = new MLineStyles(this.doc, mlineStylesHandle);
            this.doc.ImageDefinitions = new ImageDefinitions(this.doc, imageDefsHandle);
            this.doc.UnderlayDgnDefinitions = new UnderlayDgnDefinitions(this.doc, underlayDgnDefsHandle);
            this.doc.UnderlayDwfDefinitions = new UnderlayDwfDefinitions(this.doc, underlayDwfDefsHandle);
            this.doc.UnderlayPdfDefinitions = new UnderlayPdfDefinitions(this.doc, underlayPdfDefsHandle);
        }

        private DictionaryObject ReadDictionary()
        {
            string handle = null;
            //string handleOwner = null;
            DictionaryCloningFlags clonning = DictionaryCloningFlags.KeepExisting;
            bool isHardOwner = false;
            int numEntries = 0;
            List<string> names = new List<string>();
            List<string> handlesToOwner = new List<string>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 330:
                        //handleOwner = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 280:
                        isHardOwner = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 281:
                        clonning = (DictionaryCloningFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 3:
                        numEntries += 1;
                        names.Add(this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString()));
                        this.chunk.Next();
                        break;
                    case 350: // Soft-owner ID/handle to entry object 
                        handlesToOwner.Add(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 360:
                        // Hard-owner ID/handle to entry object
                        handlesToOwner.Add(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            //DxfObject owner = null;
            //if (handleOwner != null)
            //    owner = this.doc.AddedObjects[handleOwner];

            DictionaryObject dictionary = new DictionaryObject(null)
            {
                Handle = handle,
                IsHardOwner = isHardOwner,
                Cloning = clonning
            };

            for (int i = 0; i < numEntries; i++)
            {
                string id = handlesToOwner[i];
                if (id == null)
                    throw new NullReferenceException("Null handle in dictionary.");
                dictionary.Entries.Add(id, names[i]);
            }

            return dictionary;
        }

        private RasterVariables ReadRasterVariables()
        {
            RasterVariables variables = new RasterVariables();
            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        variables.Handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 70:
                        variables.DisplayFrame = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 71:
                        variables.DisplayQuality = (ImageDisplayQuality) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 72:
                        variables.Units = (ImageUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            return variables;
        }

        private ImageDefinition ReadImageDefinition()
        {
            string handle = null;
            string ownerHandle = null;
            string fileName = null;
            string name = null;
            double width = 0, height = 0;
            double wPixel = 0.0;
            double hPixel = 0.0;
            ImageResolutionUnits units = ImageResolutionUnits.Unitless;

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 1:
                        fileName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 10:
                        width = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        wPixel = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        hPixel = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 281:
                        units = (ImageResolutionUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 330:
                        ownerHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (ownerHandle != null)
            {
                DictionaryObject imageDefDict = this.dictionaries[ownerHandle];
                if (handle == null)
                    throw new NullReferenceException("Null handle in ImageDefinition dictionary.");
                name = imageDefDict.Entries[handle];
            }

            // The documentation says that this is the size of one pixel in AutoCAD units, but it seems that this is always the size of one pixel in millimeters
            // this value is used to calculate the image resolution in PPI or PPC, and the default image size.
            // The documentation in this regard and its relation with the final image size in drawing units is a complete nonsense
            double factor = UnitHelper.ConversionFactor((ImageUnits) units, DrawingUnits.Millimeters);
            ImageDefinition imageDefinition = new ImageDefinition(fileName, name, (int) width, factor/wPixel, (int) height, factor/hPixel, units)
            {
                Handle = handle
            };

            this.imgDefHandles.Add(imageDefinition.Handle, imageDefinition);
            return imageDefinition;
        }

        private ImageDefinitionReactor ReadImageDefReactor()
        {
            string handle = null;
            string imgOwner = null;

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 330:
                        imgOwner = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            return new ImageDefinitionReactor(imgOwner)
            {
                Handle = handle
            };
        }

        private MLineStyle ReadMLineStyle()
        {
            string handle = null;
            string name = null;
            string description = null;
            AciColor fillColor = AciColor.ByLayer;
            double startAngle = 90.0;
            double endAngle = 90.0;
            MLineStyleFlags flags = MLineStyleFlags.None;
            List<MLineStyleElement> elements = new List<MLineStyleElement>();

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 3:
                        description = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 62:
                        if (!fillColor.UseTrueColor)
                            fillColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;
                    case 420:
                        fillColor = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;
                    case 70:
                        flags = (MLineStyleFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 51:
                        startAngle = this.chunk.ReadDouble();
                        if (startAngle < 10.0 || startAngle > 170.0)
                            startAngle = 90.0;
                        this.chunk.Next();
                        break;
                    case 52:
                        endAngle = this.chunk.ReadDouble();
                        if (endAngle < 10.0 || endAngle > 170.0)
                            endAngle = 90.0;
                        this.chunk.Next();
                        break;
                    case 71:
                        short numElements = this.chunk.ReadShort();
                        elements = this.ReadMLineStyleElements(numElements);
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (string.IsNullOrEmpty(name)) return null;

            MLineStyle style = new MLineStyle(name, elements, description)
            {
                Handle = handle,
                FillColor = fillColor,
                Flags = flags,
                StartAngle = startAngle,
                EndAngle = endAngle
            };

            return style;
        }

        private List<MLineStyleElement> ReadMLineStyleElements(short numElements)
        {
            List<MLineStyleElement> elements = new List<MLineStyleElement>();

            this.chunk.Next();

            for (short i = 0; i < numElements; i++)
            {
                double offset = this.chunk.ReadDouble(); // code 49
                this.chunk.Next();

                AciColor color = AciColor.FromCadIndex(this.chunk.ReadShort());
                this.chunk.Next();

                if (this.chunk.Code == 420)
                {
                    color = AciColor.FromTrueColor(this.chunk.ReadInt()); // code 420
                    this.chunk.Next();
                }

                // the line type names ByLayer or ByBlock are case insensitive
                string lineTypeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString()); // code 6
                if (string.Equals(lineTypeName, LineType.ByLayerName, StringComparison.OrdinalIgnoreCase))
                    lineTypeName = LineType.ByLayerName;
                if (string.Equals(lineTypeName, LineType.ByBlockName, StringComparison.OrdinalIgnoreCase))
                    lineTypeName = LineType.ByBlockName;
                LineType lineType = this.GetLineType(lineTypeName);
                this.chunk.Next();

                MLineStyleElement element = new MLineStyleElement(offset)
                {
                    Color = color,
                    LineType = lineType
                };

                elements.Add(element);
            }

            return elements;
        }

        private Group ReadGroup()
        {
            string handle = null;
            string description = null;
            string name = null;
            bool isUnnamed = true;
            bool isSelectable = true;
            List<string> entities = new List<string>();
            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 330:
                        string handleOwner = this.chunk.ReadString();
                        DictionaryObject dict = this.dictionaries[handleOwner];
                        if (handle == null)
                            throw new NullReferenceException("Null handle in Group dictionary.");
                        name = dict.Entries[handle];
                        this.chunk.Next();
                        break;
                    case 300:
                        description = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 70:
                        isUnnamed = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 71:
                        isSelectable = this.chunk.ReadShort() != 0;
                        this.chunk.Next();
                        break;
                    case 340:
                        string entity = this.chunk.ReadString();
                        entities.Add(entity);
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            // we need to keep track of the group names generated
            if (isUnnamed) this.CheckGroupName(name);

            Group group = new Group(name, false)
            {
                Handle = handle,
                Description = description,
                IsUnnamed = isUnnamed,
                IsSelectable = isSelectable
            };

            // the group entities will be processed later
            this.groupEntities.Add(group, entities);

            return group;
        }

        private Layout ReadLayout()
        {
            PlotSettings plot = new PlotSettings();
            string handle = null;
            string owner = null;
            string name = null;
            short tabOrder = 1;
            Vector2 minLimit = new Vector2(-20.0, -7.5);
            Vector2 maxLimit = new Vector2(277.0, 202.5);
            Vector3 basePoint = Vector3.Zero;
            Vector3 minExtents = new Vector3(25.7, 19.5, 0.0);
            Vector3 maxExtents = new Vector3(231.3, 175.5, 0.0);
            double elevation = 0;
            Vector3 ucsOrigin = Vector3.Zero;
            Vector3 ucsXAxis = Vector3.UnitX;
            Vector3 ucsYAxis = Vector3.UnitY;
            BlockRecord ownerRecord = null;
            string ownerRecordHandle = null;

            string dxfCode = this.chunk.ReadString();
            this.chunk.Next();

            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new DxfObjectException(dxfCode, "Premature end of object definition.");
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 102:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;
                    case 330:
                        owner = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 100:
                        if (this.chunk.ReadString() == SubclassMarker.PlotSettings)
                            plot = this.ReadPlotSettings();
                        this.chunk.Next();
                        break;
                    case 1:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 71:
                        tabOrder = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 10:
                        minLimit.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 20:
                        minLimit.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 11:
                        maxLimit.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 21:
                        maxLimit.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 12:
                        basePoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 22:
                        basePoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 32:
                        basePoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 14:
                        minExtents.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 24:
                        minExtents.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 34:
                        minExtents.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 15:
                        maxExtents.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 25:
                        maxExtents.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 35:
                        maxExtents.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 146:
                        elevation = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 13:
                        ucsOrigin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 23:
                        ucsOrigin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 33:
                        ucsOrigin.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 16:
                        ucsXAxis.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 26:
                        ucsXAxis.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 36:
                        ucsXAxis.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 17:
                        ucsYAxis.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 27:
                        ucsYAxis.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 37:
                        ucsYAxis.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 330:
                        ownerRecordHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (handle != null)
            {
                if (this.blockRecordPointerToLayout.TryGetValue(handle, out ownerRecord))
                    this.blockRecordPointerToLayout.Remove(handle);
                else
                    ownerRecord = this.doc.GetObjectByHandle(ownerRecordHandle) as BlockRecord;
            }
            else
                ownerRecord = this.doc.GetObjectByHandle(ownerRecordHandle) as BlockRecord;
            
            if(ownerRecord != null)
                if (this.doc.Blocks.GetReferences(ownerRecord.Name).Count > 0) 
                    ownerRecord = null; // the block is already in use

            Layout layout = new Layout(name)
            {
                PlotSettings = plot,
                Handle = handle,
                MinLimit = minLimit,
                MaxLimit = maxLimit,
                BasePoint = basePoint,
                MinExtents = minExtents,
                MaxExtents = maxExtents,
                Elevation = elevation,
                UcsOrigin = ucsOrigin,
                UcsXAxis = ucsXAxis,
                UcsYAxis = ucsYAxis,
                TabOrder = tabOrder > 0 ? tabOrder : (short) (this.doc.Layouts.Count + 1),
                AssociatedBlock = ownerRecord == null ? null : this.doc.Blocks[ownerRecord.Name]          
            };

            return layout;
        }

        private PlotSettings ReadPlotSettings()
        {
            PlotSettings plot = new PlotSettings();
            Vector2 paperSize = plot.PaperSize;
            Vector2 windowBottomLeft = plot.WindowBottomLeft;
            Vector2 windowUpRight = plot.WindowUpRight;
            Vector2 paperImageOrigin = plot.PaperImageOrigin;

            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                        plot.PageSetupName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 2:
                        plot.PlotterName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 4:
                        plot.PaperSizeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 6:
                        plot.ViewName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 7:
                        plot.CurrentStyleSheet = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 40:
                        plot.LeftMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 41:
                        plot.BottomMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 42:
                        plot.RightMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 43:
                        plot.TopMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 44:
                        paperSize.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 45:
                        paperSize.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 48:
                        windowBottomLeft.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 49:
                        windowUpRight.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 140:
                        windowBottomLeft.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 141:
                        windowUpRight.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 142:
                        plot.PrintScaleNumerator = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 143:
                        plot.PrintScaleDenominator = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 70:
                        plot.Flags = (PlotFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 72:
                        plot.PaperUnits = (PlotPaperUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 73:
                        plot.PaperRotation = (PlotRotation) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;
                    case 148:
                        paperImageOrigin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    case 149:
                        paperImageOrigin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            plot.PaperSize = paperSize;
            plot.WindowBottomLeft = windowBottomLeft;
            plot.WindowUpRight = windowUpRight;
            plot.PaperImageOrigin = paperImageOrigin;

            return plot;
        }

        private UnderlayDefinition ReadUnderlayDefinition(UnderlayType type)
        {
            string handle = null;
            string page = string.Empty;
            string ownerHandle = null;
            string fileName = null;
            string name = null;

            this.chunk.Next();
            while (this.chunk.Code != 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    case 1:
                        fileName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 2:
                        page = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;
                    case 330:
                        ownerHandle = this.chunk.ReadString();
                        this.chunk.Next();
                        break;
                    default:
                        this.chunk.Next();
                        break;
                }
            }

            if (ownerHandle != null)
            {
                DictionaryObject underlayDefDict = this.dictionaries[ownerHandle];
                if (handle == null) throw new NullReferenceException("Null handle in underlay definition dictionary.");
                name = underlayDefDict.Entries[handle];
            }

            UnderlayDefinition underlayDef = null;
            switch (type)
            {
                case UnderlayType.DGN:
                    underlayDef = new UnderlayDgnDefinition(fileName, name)
                    {
                        Handle = handle,
                        Layout = page
                    };
                    break;
                case UnderlayType.DWF:
                    underlayDef = new UnderlayDwfDefinition(fileName, name)
                    {
                        Handle = handle
                    };     
                    break;
                case UnderlayType.PDF:
                    underlayDef = new UnderlayPdfDefinition(fileName, name)
                    {
                        Handle = handle,
                        Page = page
                    };     
                    break;
            }

            if (underlayDef == null) throw new NullReferenceException("Underlay reference definition.");
            this.underlayDefHandles.Add(underlayDef.Handle, underlayDef);
            return underlayDef;
        }

        #endregion

        #region private methods

        private void ReadExtensionDictionaryGroup()
        {
            //string dictionaryGroup = this.chunk.ReadString().Remove(0, 1);
            this.chunk.Next();

            while (this.chunk.Code != 102)
            {
                switch (this.chunk.Code)
                {
                    case 330:
                        break;
                    case 360:
                        break;
                }
                this.chunk.Next();
            }
        }

        private XData ReadXDataRecord(string appId)
        {
            ApplicationRegistry appReg = this.GetApplicationRegistry(appId);

            XData xData = new XData(appReg);
            this.chunk.Next();

            while (this.chunk.Code >= 1000 && this.chunk.Code <= 1071)
            {
                if (this.chunk.Code == XDataCode.AppReg)
                    break;

                short code = this.chunk.Code;
                object value = null;
                switch (code)
                {
                    case XDataCode.String:
                        value = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case XDataCode.AppReg:
                        // Application name cannot appear inside the extended data, AutoCAD assumes it is the beginning of a new application extended data group
                        break;
                    case XDataCode.ControlString:
                        value = this.chunk.ReadString();
                        break;
                    case XDataCode.LayerName:
                        value = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;
                    case XDataCode.BinaryData:
                        value = this.chunk.ReadBytes();
                        break;
                    case XDataCode.DatabaseHandle:
                        value = this.chunk.ReadString();
                        break;
                    case XDataCode.RealX:
                    case XDataCode.RealY:
                    case XDataCode.RealZ:
                    case XDataCode.WorldSpacePositionX:
                    case XDataCode.WorldSpacePositionY:
                    case XDataCode.WorldSpacePositionZ:
                    case XDataCode.WorldSpaceDisplacementX:
                    case XDataCode.WorldSpaceDisplacementY:
                    case XDataCode.WorldSpaceDisplacementZ:
                    case XDataCode.WorldDirectionX:
                    case XDataCode.WorldDirectionY:
                    case XDataCode.WorldDirectionZ:
                    case XDataCode.Real:
                    case XDataCode.Distance:
                    case XDataCode.ScaleFactor:
                        value = this.chunk.ReadDouble();
                        break;
                    case XDataCode.Int16:
                        value = this.chunk.ReadShort();
                        break;
                    case XDataCode.Int32:
                        value = this.chunk.ReadInt();
                        break;
                }

                XDataRecord xDataRecord = new XDataRecord(code, value);
                xData.XDataRecord.Add(xDataRecord);
                this.chunk.Next();
            }

            return xData;
        }

        private string DecodeEncodedNonAsciiCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string decoded;
            if (this.decodedStrings.TryGetValue(text, out decoded)) return decoded;

            int length = text.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                char c = text[i];
                if (c == '\\')
                {
                    if (i + 6 < length)
                    {
                        // \U+#### where #### is a four digits hexadecimal number
                        if ((text[i + 1] == 'U' || text[i + 1] == 'u') && text[i + 2] == '+')
                        {
                            int value;
                            string hex = text.Substring(i + 3, 4);
                            if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
                            {
                                c = (char) value;
                                i += 6;
                            }
                        }
                    }
                }
                sb.Append(c);
            }
            decoded = sb.ToString();

            // decoding of encoded non ASCII characters using regular expressions,
            // this code is slower depends on the number of non ASCII characters in the string
            //decoded = Regex.Replace(
            //    text,
            //    @"\\U\+(?<hex>[a-zA-Z0-9]{4})",
            //    m => ((char)int.Parse(m.Groups["hex"].Value, NumberStyles.HexNumber)).ToString(CultureInfo.InvariantCulture), RegexOptions.IgnoreCase);

            this.decodedStrings.Add(text, decoded);
            return decoded;
        }

        private void SetEllipseParameters(Ellipse ellipse, double[] param)
        {
            double a = ellipse.MajorAxis*0.5;
            double b = ellipse.MinorAxis*0.5;

            Vector2 startPoint = new Vector2(a * Math.Cos(param[0]), b * Math.Sin(param[0]));
            Vector2 endPoint = new Vector2(a * Math.Cos(param[1]), b * Math.Sin(param[1]));

            // trigonometry functions are very prone to round off errors
            if (startPoint.Equals(endPoint))
            {
                ellipse.StartAngle = 0.0;
                ellipse.EndAngle = 360.0;
            }
            else
            {
                ellipse.StartAngle = Vector2.Angle(startPoint) * MathHelper.RadToDeg;
                ellipse.EndAngle = Vector2.Angle(endPoint) * MathHelper.RadToDeg;
            }
        }

        private void CheckDimBlockName(string name)
        {
            // the AutoCad block names has the form *D#
            // we need to find which is the last available number, in case more dimensions are added
            if (!name.StartsWith("*D", StringComparison.OrdinalIgnoreCase)) return;

            int num;
            string token = name.Remove(0, 2);
            if (!int.TryParse(token, out num)) return;
            if (num > this.doc.DimensionBlocksGenerated)
                this.doc.DimensionBlocksGenerated = num;
        }

        private void CheckGroupName(string name)
        {
            // the AutoCad group names has the form *A#
            // we need to find which is the last available number, in case more groups are added
            if (!name.StartsWith("*A", StringComparison.OrdinalIgnoreCase)) return;
            int num;
            string token = name.Remove(0, 2);
            if (!int.TryParse(token, out num)) return;
            if (num > this.doc.GroupNamesGenerated)
                this.doc.GroupNamesGenerated = num;
        }

        private static TextAlignment ObtainAlignment(short horizontal, short vertical)
        {
            TextAlignment alignment = TextAlignment.BaselineLeft;

            if (horizontal == 0 && vertical == 3)
                alignment = TextAlignment.TopLeft;

            else if (horizontal == 1 && vertical == 3)
                alignment = TextAlignment.TopCenter;

            else if (horizontal == 2 && vertical == 3)
                alignment = TextAlignment.TopRight;

            else if (horizontal == 0 && vertical == 2)
                alignment = TextAlignment.MiddleLeft;

            else if (horizontal == 1 && vertical == 2)
                alignment = TextAlignment.MiddleCenter;

            else if (horizontal == 2 && vertical == 2)
                alignment = TextAlignment.MiddleRight;

            else if (horizontal == 0 && vertical == 1)
                alignment = TextAlignment.BottomLeft;

            else if (horizontal == 1 && vertical == 1)
                alignment = TextAlignment.BottomCenter;

            else if (horizontal == 2 && vertical == 1)
                alignment = TextAlignment.BottomRight;

            else if (horizontal == 0 && vertical == 0)
                alignment = TextAlignment.BaselineLeft;

            if (horizontal == 1 && vertical == 0)
                alignment = TextAlignment.BaselineCenter;

            else if (horizontal == 2 && vertical == 0)
                alignment = TextAlignment.BaselineRight;

            else if (horizontal == 3 && vertical == 0)
                alignment = TextAlignment.Aligned;

            else if (horizontal == 4 && vertical == 0)
                alignment = TextAlignment.Middle;

            else if (horizontal == 5 && vertical == 0)
                alignment = TextAlignment.Fit;

            return alignment;
        }

        private ApplicationRegistry GetApplicationRegistry(string name)
        {
            ApplicationRegistry appReg;
            if (this.doc.ApplicationRegistries.TryGetValue(name, out appReg))
                return appReg;

            // if an entity references a table object not defined in the tables section a new one will be created
            return this.doc.ApplicationRegistries.Add(new ApplicationRegistry(name));
        }

        private Block GetBlock(string name)
        {
            Block block;
            if (this.doc.Blocks.TryGetValue(name, out block))
                return block;
            throw new ArgumentException("The block with name " + name + " does not exist.");
        }

        private Layer GetLayer(string name)
        {
            Layer layer;
            if (this.doc.Layers.TryGetValue(name, out layer))
                return layer;

            // if an entity references a table object not defined in the tables section a new one will be created
            return this.doc.Layers.Add(new Layer(name));
        }

        private LineType GetLineType(string name)
        {
            LineType lineType;
            if (this.doc.LineTypes.TryGetValue(name, out lineType))
                return lineType;

            // if an entity references a table object not defined in the tables section a new one will be created
            return this.doc.LineTypes.Add(new LineType(name));
        }

        private TextStyle GetTextStyle(string name)
        {
            TextStyle style;
            if (this.doc.TextStyles.TryGetValue(name, out style))
                return style;

            // if an entity references a table object not defined in the tables section a new one will be created
            return this.doc.TextStyles.Add(new TextStyle(name));
        }

        private DimensionStyle GetDimensionStyle(string name)
        {
            DimensionStyle style;
            if (this.doc.DimensionStyles.TryGetValue(name, out style))
                return style;

            // if an entity references a table object not defined in the tables section a new one will be created
            return this.doc.DimensionStyles.Add(new DimensionStyle(name));
        }

        private MLineStyle GetMLineStyle(string name)
        {
            MLineStyle style;
            if (this.doc.MlineStyles.TryGetValue(name, out style))
                return style;

            return this.doc.MlineStyles.Add(new MLineStyle(name));
        }

        #endregion
    }
}