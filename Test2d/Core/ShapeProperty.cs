// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    public class ShapeProperty : ObservableObject
    {
        private object _data;

        public object Data
        {
            get { return _data; }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    Notify("Data");
                }
            }
        }

        public static ShapeProperty Create(object data)
        {
            return new ShapeProperty() 
            { 
                Data = data 
            };
        }

        public override string ToString() 
        { 
            return Data.ToString(); 
        }
    }

    public static class ShapePropertyExtensions
    {
        public static string GetStringPropertyValue(this XGroup group, string key)
        {
            if (group == null || group.Database == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.Database.Where(p => p.Key == key).FirstOrDefault().Value;
            if (property != null
                && property.Data != null
                && property.Data is string)
            {
                return property.Data as string;
            }
            else
            {
                throw new Exception(string.Format("Can not find {0} property.", key));
            }
        }

        public static int GetIntPropertyValue(this XGroup group, string key)
        {
            if (group == null || group.Database == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.Database.Where(p => p.Key == key).FirstOrDefault().Value;
            int value;
            if (property != null
                && property.Data != null
                && property.Data is string)
            {
                if (!int.TryParse(property.Data as string, out value))
                {
                    throw new Exception(string.Format("Invalid format of {0} property.", key));
                }
            }
            else
            {
                throw new Exception(string.Format("Can not find {0} property.", key));
            }
            return value;
        }

        public static double GetDoublePropertyValue(this XGroup group, string key)
        {
            if (group == null || group.Database == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.Database.Where(p => p.Key == key).FirstOrDefault().Value;
            double value;
            if (property != null
                && property.Data != null
                && property.Data is string)
            {
                if (!double.TryParse(property.Data as string, out value))
                {
                    throw new Exception(string.Format("Invalid format of {0} property.", key));
                }
            }
            else
            {
                throw new Exception(string.Format("Can not find {0} property.", key));
            }
            return value;
        }

        public static double ConvertToSeconds(this double delay, string unit)
        {
            switch (unit)
            {
                // seconds
                case "s":
                    // delay is by default in seconds
                    return delay;
                // milliseconds
                case "ms":
                    // convert milliseconds to seconds
                    return delay / 1000.0;
                // minutes
                case "m":
                    // convert minutes to seconds
                    return delay * 60.0;
                // hours
                case "h":
                    // convert hours to seconds
                    return delay * 60.0 * 60.0;
                // days
                case "d":
                    // convert hours to seconds
                    return delay * 60.0 * 60.0 * 24.0;
                default:
                    throw new Exception("Invalid delay Unit property format.");
            };
        }
    }
}
