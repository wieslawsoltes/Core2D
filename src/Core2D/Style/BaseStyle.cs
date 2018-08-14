// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Core2D.Style
{
    /// <summary>
    /// Define base style contract.
    /// </summary>
    public interface IBaseStyle : IObservableObject
    {
        /// <summary>
        /// Gets or sets stroke color.
        /// </summary>
        IColor Stroke { get; set; }

        /// <summary>
        /// Gets or sets fill color.
        /// </summary>
        IColor Fill { get; set; }

        /// <summary>
        /// Gets or sets stroke thickness.
        /// </summary>
        double Thickness { get; set; }

        /// <summary>
        /// Gets or sets line cap.
        /// </summary>
        LineCap LineCap { get; set; }

        /// <summary>
        /// Gets or sets line dashes.
        /// </summary>
        string Dashes { get; set; }

        /// <summary>
        /// Gets or sets line dash offset.
        /// </summary>
        double DashOffset { get; set; }
    }

    /// <summary>
    /// Base style.
    /// </summary>
    public abstract class BaseStyle : ObservableObject, IBaseStyle
    {
        private IColor _stroke;
        private IColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;

        /// <summary>
        /// Gets or sets stroke color.
        /// </summary>
        public IColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        /// <summary>
        /// Gets or sets fill color.
        /// </summary>
        public IColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        /// <summary>
        /// Gets or sets stroke thickness.
        /// </summary>
        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
        }

        /// <summary>
        /// Gets or sets line cap.
        /// </summary>
        public LineCap LineCap
        {
            get => _lineCap;
            set => Update(ref _lineCap, value);
        }

        /// <summary>
        /// Gets or sets line dashes.
        /// </summary>
        public string Dashes
        {
            get => _dashes;
            set => Update(ref _dashes, value);
        }

        /// <summary>
        /// Gets or sets line dash offset.
        /// </summary>
        public double DashOffset
        {
            get => _dashOffset;
            set => Update(ref _dashOffset, value);
        }

        /// <summary>
        /// Convert line dashes doubles array to string format.
        /// </summary>
        /// <param name="value">The line dashes doubles array.</param>
        /// <returns>The converted line dashes string.</returns>
        public static string ConvertDoubleArrayToDashes(double[] value)
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
        /// Convert line dashes floats array to string format.
        /// </summary>
        /// <param name="value">The line dashes floats array.</param>
        /// <returns>The converted line dashes string.</returns>
        public static string ConvertFloatArrayToDashes(float[] value)
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
        /// Convert line dashes string format to doubles array.
        /// </summary>
        /// <param name="value">The line dashes string.</param>
        /// <returns>The converted line dashes doubles array.</returns>
        public static double[] ConvertDashesToDoubleArray(string value)
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
        /// Convert line dashes string format to floats array.
        /// </summary>
        /// <param name="value">The line dashes string.</param>
        /// <returns>The converted line dashes floats array.</returns>
        public static float[] ConvertDashesToFloatArray(string value)
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

        /// <summary>
        /// Check whether the <see cref="Stroke"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStroke() => _stroke != null;

        /// <summary>
        /// Check whether the <see cref="Fill"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFill() => _fill != null;

        /// <summary>
        /// Check whether the <see cref="Thickness"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeThickness() => _thickness != default;

        /// <summary>
        /// Check whether the <see cref="LineCap"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLineCap() => _lineCap != default;

        /// <summary>
        /// Check whether the <see cref="Dashes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDashes() => !string.IsNullOrWhiteSpace(_dashes);

        /// <summary>
        /// Check whether the <see cref="DashOffset"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDashOffset() => _dashOffset != default;
    }
}
