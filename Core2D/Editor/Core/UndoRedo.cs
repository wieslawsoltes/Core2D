// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// The Undo/Redo action pair.
    /// </summary>
    internal struct UndoRedo
    {
        /// <summary>
        /// The undo <see cref="Action"/>.
        /// </summary>
        public readonly Action Undo;

        /// <summary>
        /// The redo <see cref="Action"/>.
        /// </summary>
        public readonly Action Redo;

        /// <summary>
        /// Initializes a new <see cref="UndoRedo"/> instance.
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public UndoRedo(Action undo, Action redo)
        {
            this.Undo = undo;
            this.Redo = redo;
        }

        /// <summary>
        /// Creates a new <see cref="UndoRedo"/> instance.
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        /// <returns></returns>
        public static UndoRedo Create(Action undo, Action redo)
        {
            return new UndoRedo(undo, redo);
        }
    }
}
