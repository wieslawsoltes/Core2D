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

    }
}
