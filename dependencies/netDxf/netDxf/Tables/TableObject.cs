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

namespace netDxf.Tables
{
    /// <summary>
    /// Defines classes that can be accessed by name. They are usually part of the dxf table section but can also be part of the objects section.
    /// </summary>
    public abstract class TableObject :
        DxfObject,
        ICloneable,
        IComparable,
        IComparable<TableObject>,
        IEquatable<TableObject>
    {
        #region delegates and events

        public delegate void NameChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<string> e);
        public event NameChangedEventHandler NameChanged;
        protected virtual void OnNameChangedEvent(string oldName, string newName)
        {
            NameChangedEventHandler ae = this.NameChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<string> eventArgs = new TableObjectChangedEventArgs<string>(oldName, newName);
                ae(this, eventArgs);
            }
        }

        #endregion

        #region private fields

        /// <summary>
        /// Gets the array of characters not supported as table object names.
        /// </summary>
        public static readonly string[] InvalidCharacters = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|", ";", ",", "=", "`" };
        protected bool reserved;
        protected string name;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>TableObject</c> class.
        /// </summary>
        /// <param name="name">Table name. The following characters \&lt;&gt;/?":;*|,=` are not supported for table object names.</param>
        /// <param name="codeName">Table <see cref="DxfObjectCode">code name</see>.</param>
        /// <param name="checkName">Defines if the table object name needs to be checked for invalid characters.</param>
        protected TableObject(string name, string codeName, bool checkName)
            : base(codeName)
        {
            if (checkName)
            {
                if (!IsValidName(name))
                    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", nameof(name));
            }

            this.name = name;
            this.reserved = false;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the name of the table object.
        /// </summary>
        /// <remarks>Table object names are case insensitive.</remarks>
        public virtual string Name
        {
            get { return this.name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                if (this.IsReserved)
                    throw new ArgumentException("Reserved table objects cannot be renamed.", nameof(value));
                if (!IsValidName(value))
                    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", nameof(value));
                if (string.Equals(this.name, value, StringComparison.OrdinalIgnoreCase))
                    return;
                this.OnNameChangedEvent(this.name, value);
                this.name = value;
            }
        }

        /// <summary>
        /// Gets if the table object is reserved and cannot be deleted.
        /// </summary>
        public bool IsReserved
        {
            get { return this.reserved; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Checks if a string is valid as a table object name.
        /// </summary>
        /// <param name="name">String to check.</param>
        /// <returns>True if the string is valid as a table object name, or false otherwise.</returns>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            foreach (string s in InvalidCharacters)
            {
                if (name.Contains(s))
                    return false;
            }

            // using regular expressions is slower
            //if (Regex.IsMatch(name, "[\\<>/?\":;*|,=`]"))
            //    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", "name");

            return true;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Name;
        }


        #endregion

        #region implements IComparable

        /// <summary>
        /// Compares the current TableObject with another TableObject of the same type.
        /// </summary>
        /// <param name="other">A TableObject to compare with this TableObject.</param>
        /// <returns>
        /// An integer that indicates the relative order of the table objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        /// <remarks>If both table objects are no of the same type it will return zero. The comparison is made by their names.</remarks>
        public int CompareTo(object other)
        {
            return this.CompareTo((TableObject)other);
        }

        /// <summary>
        /// Compares the current TableObject with another TableObject of the same type.
        /// </summary>
        /// <param name="other">A TableObject to compare with this TableObject.</param>
        /// <returns>
        /// An integer that indicates the relative order of the table objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        /// <remarks>If both table objects are no of the same type it will return zero. The comparison is made by their names.</remarks>
        public int CompareTo(TableObject other)
        {
            return this.GetType() == other.GetType() ? string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) : 0;
        }

        #endregion

        #region implements IEquatable

        /// <summary>
        /// Check if two TableObject are equal.
        /// </summary>
        /// <param name="obj">Another TableObject to compare to.</param>
        /// <returns>True if two TableObject are equal or false in any other case.</returns>
        /// <remarks>
        /// Two TableObjects are considered equals if their names are the same, regardless of their internal values.
        /// This is done this way because in a dxf two TableObjects cannot have the same name.
        /// </remarks>
        public bool Equals(TableObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            return string.Equals(this.Name, obj.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new table object that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">TableObject name of the copy.</param>
        /// <returns>A new table object that is a copy of this instance.</returns>
        public abstract TableObject Clone(string newName);

        /// <summary>
        /// Creates a new table object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new table object that is a copy of this instance.</returns>
        public abstract object Clone();

        #endregion
    }
}