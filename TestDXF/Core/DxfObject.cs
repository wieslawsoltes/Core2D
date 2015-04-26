// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Dxf
{
    public abstract class DxfObject
    {
        public DxfAcadVer Version { get; private set; }
        public int Id { get; private set; }

        private StringBuilder _sb = new StringBuilder();
        
        public DxfObject(DxfAcadVer version, int id)
        {
            Version = version;
            Id = id;
        }

        public abstract string Create();

        public override string ToString() { return Create(); }

        public string Build()
        {
            return _sb.ToString();
        }

        public void Reset()
        {
            _sb.Length = 0;
        }

        public void Add(string code, string data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data);
        }

        public void Add(string code, bool data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
        }

        public void Add(string code, int data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data.ToString());
        }

        public void Add(string code, double data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
        }

        public void Add(int code, string data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data);
        }

        public void Add(int code, bool data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
        }

        public void Add(int code, int data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data.ToString());
        }

        public void Add(int code, double data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
        }

        public void Append(string str)
        {
            _sb.Append(str);
        }

        public void Comment(string comment)
        {
            Add(999, comment);
        }

        public void Handle(string handle)
        {
            Add(5, handle);
        }

        public void Handle(int handle)
        {
            Add(5, handle.ToDxfHandle());
        }

        public void Subclass(string subclass)
        {
            Add(100, subclass);
        }

        public void Entity()
        {
            if (Version > DxfAcadVer.AC1009)
            {
                Add(5, Id.ToDxfHandle());
                Add(100, DxfSubclassMarker.Entity);
            }

            // TODO: Unify common Entity codes for all Entities.

            //Add(8, Layer);
            //Add(62, Color);
            //Add(6, LineType);
            //Add(370, Lineweight);
            //Add(78, LineTypeScale);
            //Add(60, IsVisible);
        }
    }
}
