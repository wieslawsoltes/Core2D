// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfRawTag
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GroupCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataElement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfRawTag Parent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<DxfRawTag> Children { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Dxf
        {
            get
            {
                var sb = new StringBuilder();
                ToDxf(this, sb);
                return sb.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="sb"></param>
        private static void ToDxf(DxfRawTag tag, StringBuilder sb)
        {
            if (tag.IsEnabled && sb != null)
            {
                sb.Append(tag.GroupCode);
                sb.Append(Environment.NewLine);
                sb.Append(tag.DataElement);
                sb.Append(Environment.NewLine);
                if (tag.Children != null)
                {
                    for (int i = 0; i < tag.Children.Count; i++)
                    {
                        var child = tag.Children[i];
                        if (child.IsEnabled)
                        {
                            ToDxf(child, sb);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public DxfRawTag()
        {
            IsEnabled = true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Children != null)
            {
                var tagName = Children.FirstOrDefault(t => t.GroupCode == 2);
                if (tagName != null)
                {
                    return string.Concat(DataElement, ':', tagName.DataElement);
                }
            }
            return (GroupCode == 0 || GroupCode == 2) ? DataElement : string.Concat(GroupCode.ToString(), ',', DataElement);
        }
    }
}
