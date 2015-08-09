// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public static class ShapePropertyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringPropertyValue(this XGroup group, string key)
        {
            if (group == null || group.ShapesProperties == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.ShapesProperties.FirstOrDefault(p => p.Name == key);
            if (property != null
                && property.Value != null
                && property.Value is string)
            {
                return property.Value as string;
            }
            else
            {
                throw new Exception(string.Format("Can not find {0} property.", key));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntPropertyValue(this XGroup group, string key)
        {
            if (group == null || group.ShapesProperties == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.ShapesProperties.FirstOrDefault(p => p.Name == key);
            int value;
            if (property != null
                && property.Value != null
                && property.Value is string)
            {
                if (!int.TryParse(property.Value as string, out value))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBoolPropertyValue(this XGroup group, string key)
        {
            if (group == null || group.ShapesProperties == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.ShapesProperties.FirstOrDefault(p => p.Name == key);
            bool value;
            if (property != null
                && property.Value != null
                && property.Value is string)
            {
                if (!bool.TryParse(property.Value as string, out value))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double GetDoublePropertyValue(this XGroup group, string key)
        {
            if (group == null || group.ShapesProperties == null)
            {
                throw new ArgumentNullException();
            }
            ShapeProperty property = group.ShapesProperties.FirstOrDefault(p => p.Name == key);
            double value;
            if (property != null
                && property.Value != null
                && property.Value is string)
            {
                if (!double.TryParse(property.Value as string, out value))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
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
