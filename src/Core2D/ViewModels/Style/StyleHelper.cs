using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Core2D.ViewModels.Style
{
    public static class StyleHelper
    {
        public static string ConvertDoubleArrayToDashes(double[] value)
        {
            try
            {
                if (value is { })
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

        public static string ConvertFloatArrayToDashes(float[] value)
        {
            try
            {
                if (value is { })
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

        public static double[] ConvertDashesToDoubleArray(string value, double strokeWidth)
        {
            try
            {
                if (value is { })
                {
                    string[] values = value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] array = new double[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        array[i] = Convert.ToDouble(values[i], CultureInfo.InvariantCulture) * strokeWidth;
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

        public static float[] ConvertDashesToFloatArray(string value, double strokeWidth)
        {
            try
            {
                if (value is { })
                {
                    string[] values = value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    float[] array = new float[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        array[i] = Convert.ToSingle(values[i], CultureInfo.InvariantCulture) * (float)strokeWidth;
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
