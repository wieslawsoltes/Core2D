// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Dxf
{
    public abstract class DxfObject<T> where T : DxfObject<T>
    {
        public virtual DxfAcadVer Version { get; private set; }
        public virtual int Id { get; private set; }
        protected StringBuilder sb = new StringBuilder();
        
        public DxfObject(DxfAcadVer version, int id)
        {
            this.Version = version;
            this.Id = id;
        }

        public override string ToString()
        {
            return this.Build();
        }

        public virtual void Reset()
        {
            this.sb.Length = 0;
        }

        public virtual string Build()
        {
            return this.sb.ToString();
        }

        public virtual T Add(string code, string data)
        {
            this.sb.AppendLine(code);
            this.sb.AppendLine(data);
            return this as T;
        }

        public virtual T Add(string code, bool data)
        {
            this.sb.AppendLine(code);
            this.sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
            return this as T;
        }

        public virtual T Add(string code, int data)
        {
            this.sb.AppendLine(code);
            this.sb.AppendLine(data.ToString());
            return this as T;
        }

        public virtual T Add(string code, double data)
        {
            this.sb.AppendLine(code);
            this.sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
            return this as T;
        }

        public virtual T Add(int code, string data)
        {
            this.sb.AppendLine(code.ToString());
            this.sb.AppendLine(data);
            return this as T;
        }

        public virtual T Add(int code, bool data)
        {
            this.sb.AppendLine(code.ToString());
            this.sb.AppendLine(data == true ? 1.ToString() : 0.ToString());
            return this as T;
        }

        public virtual T Add(int code, int data)
        {
            this.sb.AppendLine(code.ToString());
            this.sb.AppendLine(data.ToString());
            return this as T;
        }

        public virtual T Add(int code, double data)
        {
            this.sb.AppendLine(code.ToString());
            this.sb.AppendLine(data.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB")));
            return this as T;
        }

        protected virtual T Append(string str)
        {
            this.sb.Append(str);
            return this as T;
        }

        public virtual T Comment(string comment)
        {
            Add(999, comment);
            return this as T;
        }

        public virtual T Handle(string handle)
        {
            Add(5, handle);
            return this as T;
        }

        public virtual T Handle(int handle)
        {
            Add(5, handle.ToDxfHandle());
            return this as T;
        }

        public virtual T Subclass(string subclass)
        {
            Add(100, subclass);
            return this as T;
        }

        public virtual T Entity()
        {
            if (Version > DxfAcadVer.AC1009)
            {
                Add(5, Id.ToDxfHandle());
                Add(100, DxfSubclassMarker.Entity);
            }

            // TODO: Unify common Entity codes for all Entities.
            //Add(8, layer);
            //Add(62, color);
            //Add(6, lineType);
            //Add(370, lineweight);
            //Add(78, lineTypeScale);
            //Add(60, isVisible);

            return this as T;
        }
    }
}
