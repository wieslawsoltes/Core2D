// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.History
{
    /// <summary>
    /// Undo/redo action pair.
    /// </summary>
    public struct UndoRedo
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
            Undo = undo;
            Redo = redo;
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
