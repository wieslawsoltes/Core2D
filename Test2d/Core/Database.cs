// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Database : ObservableObject
    {
        private string _name;
        private IList<KeyValuePair<string, ShapeProperty>> _records;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<KeyValuePair<string, ShapeProperty>> Records
        {
            get { return _records; }
            set
            {
                if (value != _records)
                {
                    _records = value;
                    Notify("Database");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void AddProperty(ShapeProperty property)
        {
            _records.Add(
                new KeyValuePair<string, ShapeProperty>(
                    property.Name,
                    property));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="property"></param>
        public void AddProperty(string key, ShapeProperty property)
        {
            _records.Add(
                new KeyValuePair<string, ShapeProperty>(
                    key,
                    property));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Database Create(string name = "")
        {
            return new Database()
            {
                Name = name,
                Records = new ObservableCollection<KeyValuePair<string, ShapeProperty>>(),
            };
        }
    }
}
