// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public DxfAcadVer Version { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; private set; }

        private StringBuilder _sb = new StringBuilder();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfObject(DxfAcadVer version, int id)
        {
            Version = version;
            Id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string Create();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Create(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return _sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _sb.Length = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(string code, string data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(string code, bool data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(string code, int data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(string code, double data)
        {
            _sb.AppendLine(code);
            _sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(int code, string data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(int code, bool data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(int code, int data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void Add(int code, double data)
        {
            _sb.AppendLine(code.ToString());
            _sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void Append(string str)
        {
            _sb.Append(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        public void Comment(string comment)
        {
            Add(999, comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public void Handle(string handle)
        {
            Add(5, handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        public void Handle(int handle)
        {
            Add(5, handle.ToDxfHandle());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subclass"></param>
        public void Subclass(string subclass)
        {
            Add(100, subclass);
        }

        /// <summary>
        /// 
        /// </summary>
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
