// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseStyle : ObservableObject
    {
        private string _name;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArgbColor Stroke
        {
            get { return _stroke; }
            set { Update(ref _stroke, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArgbColor Fill
        {
            get { return _fill; }
            set { Update(ref _fill, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Thickness
        {
            get { return _thickness; }
            set { Update(ref _thickness, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LineCap LineCap
        {
            get { return _lineCap; }
            set { Update(ref _lineCap, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Dashes
        {
            get { return _dashes; }
            set { Update(ref _dashes, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double DashOffset
        {
            get { return _dashOffset; }
            set { Update(ref _dashOffset, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DoubleArrayToDashes(double[] value)
        {
            try
            {
                if (value != null)
                {
                    return string.Join(
                        " ",
                        value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FloatArrayToDashes(float[] value)
        {
            try
            {
                if (value != null)
                {
                    return string.Join(
                        " ",
                        value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double[] DashesToDoubleArray(string value)
        {
            try
            {
                if (value != null)
                {
                    string[] a = value.Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (a != null && a.Length > 0)
                    {
                        return a.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float[] DashesToFloatArray(string value)
        {
            try
            {
                if (value != null)
                {
                    string[] a = value.Split(
                        new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (a != null && a.Length > 0)
                    {
                        return a.Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}
