// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Test.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class DoubleToStringConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                double[] a = value as double[];
                if (a != null)
                    return string.Join(
                        " ",
                        a.Select(x => x.ToString(culture)));
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                string s = value as string;
                if (s != null)
                {
                    string[] a = s.Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (a != null && a.Length > 0)
                        return a.Select(x => double.Parse(x, culture)).ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }

            return null;
        }
    }
}
