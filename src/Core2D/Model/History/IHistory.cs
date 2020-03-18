using System;

namespace Core2D.History
{
    /// <summary>
    /// Undo/redo action history contract.
    /// </summary>
    public interface IHistory
    {
        /// <summary>
        /// Make undo/redo history snapshot.
        /// </summary>
        /// <param name="previous">The previous state.</param>
        /// <param name="next">The next state</param>
        /// <param name="update">The update method.</param>
        void Snapshot<T>(T previous, T next, Action<T> update);

        /// <summary>
        /// Check if undo action can execute.
        /// </summary>
        /// <returns>True if undo action can execute.</returns>
        bool CanUndo();

        /// <summary>
        /// Check if redo action can execute.
        /// </summary>
        /// <returns>True if redo action can execute.</returns>
        bool CanRedo();

        /// <summary>
        /// Execute undo action.
        /// </summary>
        /// <returns>True if undo action was executed.</returns>
        bool Undo();

        /// <summary>
        /// Execute redo action.
        /// </summary>
        /// <returns>True if redo action was executed.</returns>
        bool Redo();

        /// <summary>
        /// Reset undo/redo actions history.
        /// </summary>
        void Reset();
    }
}
