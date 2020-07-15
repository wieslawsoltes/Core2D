using System;

namespace Core2D.Editor.History
{
    /// <summary>
    /// Undo/redo action pair.
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
