// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dxf
{
    public class DxfRawTag
    {
        public bool IsEnabled { get; set; }
        public int GroupCode { get; set; }
        public string DataElement { get; set; }
        public DxfRawTag Parent { get; set; }
        public IList<DxfRawTag> Children { get; set; }
        
        public string Dxf
        {
            get
            {
                var sb = new StringBuilder();
                ToDxf(this, sb);
                return sb.ToString();
            }
        }

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
        
        public DxfRawTag()
        {
            IsEnabled = true;
        }
        
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
