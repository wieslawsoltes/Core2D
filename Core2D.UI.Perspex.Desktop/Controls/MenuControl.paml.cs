// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.UI.Perspex.Desktop.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MenuControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
