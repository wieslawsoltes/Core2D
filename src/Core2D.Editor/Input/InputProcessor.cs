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
        private IDisposable _leftDownDisposable = null;
        private IDisposable _leftUpDisposable = null;
        private IDisposable _rightDownDisposable = null;
        private IDisposable _rightUpDisposable = null;
        private IDisposable _moveDisposable = null;

        /// <summary>
        /// Connects source and target inputs.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public void Connect(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor LeftDown");

            _leftDownDisposable = source.LeftDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftDown");
                    if (target.IsLeftDownAvailable())
                    {
                        target.LeftDown(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor LeftUp");

            _leftUpDisposable = source.LeftUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftUp");
                    if (target.IsLeftUpAvailable())
                    {
                        target.LeftUp(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor RightDown");

            _rightDownDisposable = source.RightDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightDown");
                    if (target.IsRightDownAvailable())
                    {
                        target.RightDown(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor RightUp");

            _rightUpDisposable = source.RightUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightUp");
                    if (target.IsRightUpAvailable())
                    {
                        target.RightUp(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor Move");

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
        /// Disconnects source and target inputs.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnect InputProcessor");
            _leftDownDisposable?.Dispose();
            _leftUpDisposable?.Dispose();
            _rightDownDisposable?.Dispose();
            _rightUpDisposable.Dispose();
            _moveDisposable?.Dispose();
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Dispose InputProcessor");
            Disconnect();
        }
    }
}
