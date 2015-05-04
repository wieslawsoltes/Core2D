// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Controls
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double[] a = value as double[];
                if (a != null)
                    return string.Join(" ", a.Select(x => x.ToString(culture)));
            }
            catch { }

            return null;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string s = value as string;
                if (s != null)
                {
                    string[] a = s.Split(new char [] { ' ' });
                    if (a != null && a.Length > 0)
                        return a.Select(x => double.Parse(x, culture)).ToArray();
                }
            }
            catch { }

            return null;
        }
    }
    
    public partial class StyleControl : UserControl
    {
        public StyleControl()
        {
            InitializeComponent();
        }
    }
}
