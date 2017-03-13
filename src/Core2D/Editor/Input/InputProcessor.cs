// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Provides mouse input for target object.
    /// </summary>
    public class InputProcessor : IDisposable
    {
        private readonly IDisposable _leftDownDisposable;
        private readonly IDisposable _leftUpDisposable;
        private readonly IDisposable _rightDownDisposable;
        private readonly IDisposable _rightUpDisposable;
        private readonly IDisposable _moveDisposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputProcessor"/> class.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public InputProcessor(IInputSource source, IInputTarget target)
        {
            _leftDownDisposable = source.LeftDown.Subscribe(
                (args) =>
                {
                    if (target.IsLeftDownAvailable())
                    {
                        target.LeftDown(args.Position.X, args.Position.Y, args.Modifier);
                    }
                });

            _leftUpDisposable = source.LeftUp.Subscribe(
                (args) =>
                {
                    if (target.IsLeftUpAvailable())
                    {
                        target.LeftUp(args.Position.X, args.Position.Y, args.Modifier);
                    }
                });

            _rightDownDisposable = source.RightDown.Subscribe(
                (args) =>
                {
                    if (target.IsRightDownAvailable())
                    {
                        target.RightDown(args.Position.X, args.Position.Y, args.Modifier);
                    }
                });

            _rightUpDisposable = source.RightUp.Subscribe(
                (args) =>
                {
                    if (target.IsRightUpAvailable())
                    {
                        target.RightUp(args.Position.X, args.Position.Y, args.Modifier);
                    }
                });

            _moveDisposable = source.Move.Subscribe(
                (args) =>
                {
                    if (target.IsMoveAvailable())
                    {
                        target.Move(args.Position.X, args.Position.Y, args.Modifier);
                    }
                });
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~InputProcessor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftDownDisposable.Dispose();
                _leftUpDisposable.Dispose();
                _rightDownDisposable.Dispose();
                _rightUpDisposable.Dispose();
                _moveDisposable.Dispose();
            }
        }
    }
}
