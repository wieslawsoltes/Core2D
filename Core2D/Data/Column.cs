// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Column : ObservableObject
    {
        private Guid _id;
        private string _name;
        private double _width;
        private bool _isVisible;
        private Database _owner;

        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { Update(ref _id, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { Update(ref _width, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Update(ref _isVisible, value); }
        }

        /// <summary>
        /// Gets or sets column owner object.
        /// </summary>
        public Database Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Column"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="width"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static Column Create(
            string name,
            Database owner,
            double width = double.NaN,
            bool isVisible = true)
        {
            return new Column()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Width = width,
                IsVisible = isVisible,
                Owner = owner
            };
        }
    }
}
