using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Core2D.Style
{
    /// <summary>
    /// Style helper.
    /// </summary>
    public static class StyleHelper
    {
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
                    return string.Join(" ", value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
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
                    return string.Join(" ", value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
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
        /// <param name="strokeWidth">The stroke width.</param>
        /// <returns>The converted line dashes doubles array.</returns>
        public static double[] ConvertDashesToDoubleArray(string value, double strokeWidth)
        {
            try
            {
                if (value != null)
                {
                    string[] values = value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] array = new double[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        array[i] = Convert.ToDouble(values[i]) * strokeWidth;
                    }
                    if (array.Length >= 2 && array.Length % 2 == 0)
                    {
                        return array;
                    }
                    return null;
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
        /// <param name="strokeWidth">The stroke width.</param>
        /// <returns>The converted line dashes floats array.</returns>
        public static float[] ConvertDashesToFloatArray(string value, double strokeWidth)
        {
            try
            {
                if (value != null)
                {
                    string[] values = value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    float[] array = new float[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        array[i] = Convert.ToSingle(values[i]) * (float)strokeWidth;
                    }
                    if (array.Length >= 2 && array.Length % 2 == 0)
                    {
                        return array;
                    }
                    return null;
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
