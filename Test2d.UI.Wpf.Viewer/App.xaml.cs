// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Test2d;

namespace Test.Viewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length == 1)
            {
                Open(e.Args[0]);
            }
            else
            { 
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = ""
                };

                if (dlg.ShowDialog() == true)
                {
                    Open(dlg.FileName);
                }
            }
        }

        private void Open(string path)
        {
            try
            {
                var serializer = new NewtonsoftSerializer();
                var json = Utf8TextFile.Read(path);
                var project = serializer.FromJson<Project>(json);
                if (project != null)
                {
                    var documents = project.Documents.FirstOrDefault();
                    if (documents != null && documents.Containers != null)
                    {
                        ProjectViewer.Show(documents.Containers);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
