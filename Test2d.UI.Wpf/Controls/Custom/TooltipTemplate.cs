// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class TooltipTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached(
                "Header",
                typeof(string),
                typeof(TooltipTemplate),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetScript(DependencyObject obj)
        {
            return (string)obj.GetValue(ScriptProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetScript(DependencyObject obj, string value)
        {
            obj.SetValue(ScriptProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScriptProperty =
            DependencyProperty.RegisterAttached(
                "Script",
                typeof(string),
                typeof(TooltipTemplate),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));
    }
}
