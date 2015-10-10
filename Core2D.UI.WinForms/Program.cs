// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Forms;

namespace TestWinForms
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
