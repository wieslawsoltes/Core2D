// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace TestWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) 
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.copyAsMetafileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.groupSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupCurrentLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.selectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.pointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ellipseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bezierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qBezierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultIsFilledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.snapToGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.tryToConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.tickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.styleEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.shapesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.containerEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.scriptsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.simulationToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(900, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator1,
            this.openToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.exportToolStripMenuItem,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(160, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(160, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(160, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exportToolStripMenuItem.Text = "&Export...";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator21,
            this.copyAsMetafileToolStripMenuItem,
            this.toolStripSeparator5,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator6,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator22,
            this.clearAllToolStripMenuItem,
            this.toolStripSeparator7,
            this.groupSelectedToolStripMenuItem,
            this.groupCurrentLayerToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(235, 6);
            // 
            // copyAsMetafileToolStripMenuItem
            // 
            this.copyAsMetafileToolStripMenuItem.Name = "copyAsMetafileToolStripMenuItem";
            this.copyAsMetafileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.copyAsMetafileToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.copyAsMetafileToolStripMenuItem.Text = "Copy As &Metafile";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(235, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(235, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(235, 6);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.clearAllToolStripMenuItem.Text = "Cl&ear All";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(235, 6);
            // 
            // groupSelectedToolStripMenuItem
            // 
            this.groupSelectedToolStripMenuItem.Name = "groupSelectedToolStripMenuItem";
            this.groupSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.groupSelectedToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.groupSelectedToolStripMenuItem.Text = "&Group";
            // 
            // groupCurrentLayerToolStripMenuItem
            // 
            this.groupCurrentLayerToolStripMenuItem.Name = "groupCurrentLayerToolStripMenuItem";
            this.groupCurrentLayerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.groupCurrentLayerToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.groupCurrentLayerToolStripMenuItem.Text = "Group &Layer";
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.toolStripSeparator8,
            this.selectionToolStripMenuItem,
            this.toolStripSeparator9,
            this.pointToolStripMenuItem,
            this.toolStripSeparator17,
            this.lineToolStripMenuItem,
            this.rectangleToolStripMenuItem,
            this.ellipseToolStripMenuItem,
            this.arcToolStripMenuItem,
            this.bezierToolStripMenuItem,
            this.qBezierToolStripMenuItem,
            this.toolStripSeparator10,
            this.textToolStripMenuItem,
            this.toolStripSeparator23,
            this.imageToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.toolToolStripMenuItem.Text = "&Tool";
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.ShortcutKeyDisplayString = "N";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.noneToolStripMenuItem.Text = "&None";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(149, 6);
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.ShortcutKeyDisplayString = "S";
            this.selectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selectionToolStripMenuItem.Text = "&Selection";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(149, 6);
            // 
            // pointToolStripMenuItem
            // 
            this.pointToolStripMenuItem.Name = "pointToolStripMenuItem";
            this.pointToolStripMenuItem.ShortcutKeyDisplayString = "P";
            this.pointToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pointToolStripMenuItem.Text = "&Point";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(149, 6);
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.ShortcutKeyDisplayString = "L";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lineToolStripMenuItem.Text = "&Line";
            // 
            // rectangleToolStripMenuItem
            // 
            this.rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
            this.rectangleToolStripMenuItem.ShortcutKeyDisplayString = "R";
            this.rectangleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rectangleToolStripMenuItem.Text = "&Rectangle";
            // 
            // ellipseToolStripMenuItem
            // 
            this.ellipseToolStripMenuItem.Name = "ellipseToolStripMenuItem";
            this.ellipseToolStripMenuItem.ShortcutKeyDisplayString = "E";
            this.ellipseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ellipseToolStripMenuItem.Text = "&Ellipse";
            // 
            // arcToolStripMenuItem
            // 
            this.arcToolStripMenuItem.Name = "arcToolStripMenuItem";
            this.arcToolStripMenuItem.ShortcutKeyDisplayString = "A";
            this.arcToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.arcToolStripMenuItem.Text = "&Arc";
            // 
            // bezierToolStripMenuItem
            // 
            this.bezierToolStripMenuItem.Name = "bezierToolStripMenuItem";
            this.bezierToolStripMenuItem.ShortcutKeyDisplayString = "B";
            this.bezierToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.bezierToolStripMenuItem.Text = "&Bezier";
            // 
            // qBezierToolStripMenuItem
            // 
            this.qBezierToolStripMenuItem.Name = "qBezierToolStripMenuItem";
            this.qBezierToolStripMenuItem.ShortcutKeyDisplayString = "Q";
            this.qBezierToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.qBezierToolStripMenuItem.Text = "&QBezier";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(149, 6);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.ShortcutKeyDisplayString = "T";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textToolStripMenuItem.Text = "&Text";
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.evaluateToolStripMenuItem});
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.scriptToolStripMenuItem.Text = "&Script";
            // 
            // evaluateToolStripMenuItem
            // 
            this.evaluateToolStripMenuItem.Name = "evaluateToolStripMenuItem";
            this.evaluateToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.evaluateToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.evaluateToolStripMenuItem.Text = "&Evaluate...";
            // 
            // scriptsToolStripMenuItem
            // 
            this.scriptsToolStripMenuItem.Name = "scriptsToolStripMenuItem";
            this.scriptsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.scriptsToolStripMenuItem.Text = "S&cripts";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultIsFilledToolStripMenuItem,
            this.toolStripSeparator11,
            this.snapToGridToolStripMenuItem,
            this.toolStripSeparator16,
            this.tryToConnectToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // defaultIsFilledToolStripMenuItem
            // 
            this.defaultIsFilledToolStripMenuItem.Name = "defaultIsFilledToolStripMenuItem";
            this.defaultIsFilledToolStripMenuItem.ShortcutKeyDisplayString = "F";
            this.defaultIsFilledToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.defaultIsFilledToolStripMenuItem.Text = "Default is &Filled";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(166, 6);
            // 
            // snapToGridToolStripMenuItem
            // 
            this.snapToGridToolStripMenuItem.Name = "snapToGridToolStripMenuItem";
            this.snapToGridToolStripMenuItem.ShortcutKeyDisplayString = "G";
            this.snapToGridToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.snapToGridToolStripMenuItem.Text = "&Snap to Grid";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(166, 6);
            // 
            // tryToConnectToolStripMenuItem
            // 
            this.tryToConnectToolStripMenuItem.Name = "tryToConnectToolStripMenuItem";
            this.tryToConnectToolStripMenuItem.ShortcutKeyDisplayString = "C";
            this.tryToConnectToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.tryToConnectToolStripMenuItem.Text = "Try To &Connect";
            // 
            // simulationToolStripMenuItem
            // 
            this.simulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripSeparator18,
            this.restartToolStripMenuItem,
            this.toolStripSeparator19,
            this.pauseToolStripMenuItem,
            this.toolStripSeparator20,
            this.tickToolStripMenuItem});
            this.simulationToolStripMenuItem.Name = "simulationToolStripMenuItem";
            this.simulationToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.simulationToolStripMenuItem.Text = "S&imulation";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.startToolStripMenuItem.Text = "&Start";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.stopToolStripMenuItem.Text = "S&top";
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(153, 6);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.restartToolStripMenuItem.Text = "&Restart";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(153, 6);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pauseToolStripMenuItem.Text = "&Pause";
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(153, 6);
            // 
            // tickToolStripMenuItem
            // 
            this.tickToolStripMenuItem.Name = "tickToolStripMenuItem";
            this.tickToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.tickToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.tickToolStripMenuItem.Text = "Ti&ck";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layersToolStripMenuItem,
            this.toolStripSeparator12,
            this.styleEditorToolStripMenuItem,
            this.stylesToolStripMenuItem,
            this.toolStripSeparator13,
            this.shapesToolStripMenuItem,
            this.toolStripSeparator14,
            this.containerEditorToolStripMenuItem,
            this.toolStripSeparator15,
            this.propertiessToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.layersToolStripMenuItem.Text = "&Layers...";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(207, 6);
            // 
            // styleEditorToolStripMenuItem
            // 
            this.styleEditorToolStripMenuItem.Name = "styleEditorToolStripMenuItem";
            this.styleEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.styleEditorToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.styleEditorToolStripMenuItem.Text = "St&yle Editor...";
            // 
            // stylesToolStripMenuItem
            // 
            this.stylesToolStripMenuItem.Name = "stylesToolStripMenuItem";
            this.stylesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.stylesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.stylesToolStripMenuItem.Text = "S&tyles...";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(207, 6);
            // 
            // shapesToolStripMenuItem
            // 
            this.shapesToolStripMenuItem.Name = "shapesToolStripMenuItem";
            this.shapesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.shapesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.shapesToolStripMenuItem.Text = "S&hapes...";
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(207, 6);
            // 
            // containerEditorToolStripMenuItem
            // 
            this.containerEditorToolStripMenuItem.Name = "containerEditorToolStripMenuItem";
            this.containerEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.containerEditorToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.containerEditorToolStripMenuItem.Text = "Containe&r Editor...";
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(207, 6);
            // 
            // propertiessToolStripMenuItem
            // 
            this.propertiessToolStripMenuItem.Name = "propertiessToolStripMenuItem";
            this.propertiessToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Return)));
            this.propertiessToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.propertiessToolStripMenuItem.Text = "&Propertiess...";
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.ShortcutKeyDisplayString = "I";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.imageToolStripMenuItem.Text = "&Image";
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(149, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(900, 720);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAsMetafileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem groupSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupCurrentLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem selectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rectangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ellipseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bezierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qBezierToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultIsFilledToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem snapToGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem styleEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stylesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem shapesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem containerEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem propertiessToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem tryToConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem scriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem tickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
    }
}
