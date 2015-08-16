// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace TestWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// 
        /// </summary>
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
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.selectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.pointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bezierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qBezierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.rectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ellipseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultIsFilledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.snapToGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.tryToConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.optionsToolStripMenuItem});
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
            this.arcToolStripMenuItem,
            this.bezierToolStripMenuItem,
            this.qBezierToolStripMenuItem,
            this.toolStripSeparator5,
            this.pathToolStripMenuItem,
            this.toolStripSeparator6,
            this.rectangleToolStripMenuItem,
            this.ellipseToolStripMenuItem,
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
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.noneToolStripMenuItem.Text = "&None";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(137, 6);
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.ShortcutKeyDisplayString = "S";
            this.selectionToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.selectionToolStripMenuItem.Text = "&Selection";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(137, 6);
            // 
            // pointToolStripMenuItem
            // 
            this.pointToolStripMenuItem.Name = "pointToolStripMenuItem";
            this.pointToolStripMenuItem.ShortcutKeyDisplayString = "P";
            this.pointToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.pointToolStripMenuItem.Text = "&Point";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(137, 6);
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.ShortcutKeyDisplayString = "L";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.lineToolStripMenuItem.Text = "&Line";
            // 
            // arcToolStripMenuItem
            // 
            this.arcToolStripMenuItem.Name = "arcToolStripMenuItem";
            this.arcToolStripMenuItem.ShortcutKeyDisplayString = "A";
            this.arcToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.arcToolStripMenuItem.Text = "&Arc";
            // 
            // bezierToolStripMenuItem
            // 
            this.bezierToolStripMenuItem.Name = "bezierToolStripMenuItem";
            this.bezierToolStripMenuItem.ShortcutKeyDisplayString = "B";
            this.bezierToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.bezierToolStripMenuItem.Text = "&Bezier";
            // 
            // qBezierToolStripMenuItem
            // 
            this.qBezierToolStripMenuItem.Name = "qBezierToolStripMenuItem";
            this.qBezierToolStripMenuItem.ShortcutKeyDisplayString = "Q";
            this.qBezierToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.qBezierToolStripMenuItem.Text = "&QBezier";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(137, 6);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.ShortcutKeyDisplayString = "H";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.pathToolStripMenuItem.Text = "Pat&h";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(137, 6);
            // 
            // rectangleToolStripMenuItem
            // 
            this.rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
            this.rectangleToolStripMenuItem.ShortcutKeyDisplayString = "R";
            this.rectangleToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.rectangleToolStripMenuItem.Text = "&Rectangle";
            // 
            // ellipseToolStripMenuItem
            // 
            this.ellipseToolStripMenuItem.Name = "ellipseToolStripMenuItem";
            this.ellipseToolStripMenuItem.ShortcutKeyDisplayString = "E";
            this.ellipseToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.ellipseToolStripMenuItem.Text = "&Ellipse";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(137, 6);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.ShortcutKeyDisplayString = "T";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.textToolStripMenuItem.Text = "&Text";
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(137, 6);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.ShortcutKeyDisplayString = "I";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.imageToolStripMenuItem.Text = "&Image";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(900, 662);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test2d";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem defaultIsFilledToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem snapToGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem tryToConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}
