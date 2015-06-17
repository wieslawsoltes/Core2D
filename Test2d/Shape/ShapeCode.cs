// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Defines code used to create C# executable scripts encapsulated in class body and executed during simulation.
    /// </summary>
    public class ShapeCode : ObservableObject
    {
        private bool _isExecutable;
        private string _definitions;
        private string _initialization;
        private string _script;

        /// <summary>
        /// Gets or sets value indicating whether C# code is executable.
        /// </summary>
        public bool IsExecutable
        {
            get { return _isExecutable; }
            set { Update(ref _isExecutable, value); }
        }

        /// <summary>
        /// Gets or sets shape executable C# code (fields, properties, methods and events) defined in class body.
        /// </summary>
        public string Definitions
        {
            get { return _definitions; }
            set { Update(ref _definitions, value); }
        }

        /// <summary>
        /// Gets or sets shape executable C# code executed once in class constructor before simulation.
        /// </summary>
        public string Initialization
        {
            get { return _initialization; }
            set { Update(ref _initialization, value); }
        }

        /// <summary>
        /// Gets or sets shape executable C# script code executed every simulation cicle when Run method is called.
        /// </summary>
        public string Script
        {
            get { return _script; }
            set { Update(ref _script, value); }
        }

        /// <summary>
        /// Creates a new instance of the ShapeCode class.
        /// </summary>
        /// <returns>The new instance of the ShapeCode class.</returns>
        public static ShapeCode Create()
        {
            return new ShapeCode();
        }
    }
}
