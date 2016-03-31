#region netDxf, Copyright(C) 2015 Daniel Carvajal, Licensed under LGPL.
// 
//                         netDxf library
//  Copyright (C) 2009-2015 Daniel Carvajal (haplokuon@gmail.com)
//  
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;

namespace netDxf.IO
{
    public class DxfException : Exception
    {
        public DxfException()
        {
        }

        public DxfException(string message)
            : base(message)
        {
        }

        public DxfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    #region section exceptions

    public class DxfHeaderVariableException : DxfException
    {

        public DxfHeaderVariableException(string name)
            : base()
        {
        }

        public DxfHeaderVariableException(string name, string message)
            : base(message)
        {
        }

    }

    public class DxfSectionException : DxfException
    {
        private readonly string section;

        public DxfSectionException(string section)
            : base()
        {
            this.section = section;
        }

        public DxfSectionException(string section, string message)
            : base(message)
        {
            this.section = section;
        }

        public string Section
        {
            get { return this.section; }
        }
    }

    public class InvalidDxfSectionException : DxfSectionException
    {
        public InvalidDxfSectionException(string section)
            : base(section)
        {
        }

        public InvalidDxfSectionException(string section, string message)
            : base(section, message)
        {
        }
    }

    public class OpenDxfSectionException : DxfSectionException
    {
        public OpenDxfSectionException(string section)
            : base(section)
        {
        }

        public OpenDxfSectionException(string section, string message)
            : base(section, message)
        {
        }
    }

    public class ClosedDxfSectionException : DxfSectionException
    {
        public ClosedDxfSectionException(string section)
            : base(section)
        {
        }

        public ClosedDxfSectionException(string section, string message)
            : base(section, message)
        {
        }
    }

    #endregion

    //#region table exceptions

    public class DxfTableException : DxfException
    {
        private readonly string table;

        public DxfTableException(string table)
        {
            this.table = table;
        }

        public DxfTableException(string table, string message)
            : base(message)
        {
            this.table = table;
        }

        public string Table
        {
            get { return this.table; }
        }
    }

    #region entity exceptions

    public class DxfObjectException : DxfException
    {
        private readonly string name;

        public DxfObjectException(string name, string message)
            : base(message)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
    }

    public class DxfInvalidCodeValueEntityException : DxfException
    {
        private readonly int code;
        private readonly string value;

        public DxfInvalidCodeValueEntityException(int code, string value, string message)
            : base(message)
        {
            this.code = code;
            this.value = value;
        }

        public int Code
        {
            get { return this.code; }
        }

        public string Value
        {
            get { return this.value; }
        }
    }

    #endregion
}