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
            Console.WriteLine("Create InputProcessor LeftDown");

            _leftDownDisposable = source.LeftDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftDown");
                    if (target.IsLeftDownAvailable())
                    {
                        target.LeftDown(args);
                    }
                });

            Console.WriteLine("Create InputProcessor LeftUp");

            _leftUpDisposable = source.LeftUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftUp");
                    if (target.IsLeftUpAvailable())
                    {
                        target.LeftUp(args);
                    }
                });

            Console.WriteLine("Create InputProcessor RightDown");

            _rightDownDisposable = source.RightDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightDown");
                    if (target.IsRightDownAvailable())
                    {
                        target.RightDown(args);
                    }
                });

            Console.WriteLine("Create InputProcessor RightUp");

            _rightUpDisposable = source.RightUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightUp");
                    if (target.IsRightUpAvailable())
                    {
                        target.RightUp(args);
                    }
                });

            Console.WriteLine("Create InputProcessor Move");

            _moveDisposable = source.Move.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor Move");
                    if (target.IsMoveAvailable())
                    {
                        target.Move(args);
                    }
                });

            Console.WriteLine("Create InputProcessor Done!");
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Dispose InputProcessor");
            _leftDownDisposable.Dispose();
            _leftUpDisposable.Dispose();
            _rightDownDisposable.Dispose();
            _rightUpDisposable.Dispose();
            _moveDisposable.Dispose();
        }
    }
}
