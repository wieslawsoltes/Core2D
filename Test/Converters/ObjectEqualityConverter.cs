// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Test.Converters
{
    public class ObjectEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)  
        {
            return (values[0] == values[1]) ? true : false;
        }  
 
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)  
        {  
            throw new NotImplementedException();  
        }
    }
}
