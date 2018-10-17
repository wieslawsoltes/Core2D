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

        private void ConnectLeftDown(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor LeftDown begin!");

            _leftDownDisposable = source.LeftDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftDown");
                    if (target.IsLeftDownAvailable())
                    {
                        target.LeftDown(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor LeftDown end!");
        }

        private void ConnectLeftUp(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor LeftUp begin!");

            _leftUpDisposable = source.LeftUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor LeftUp");
                    if (target.IsLeftUpAvailable())
                    {
                        target.LeftUp(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor LeftUp end!");
        }

        private void ConnectRightDown(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor RightDown begin!");

            _rightDownDisposable = source.RightDown.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightDown");
                    if (target.IsRightDownAvailable())
                    {
                        target.RightDown(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor RightDown end!");
        }

        private void ConnectRightUp(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor RightUp begin!");

            _rightUpDisposable = source.RightUp.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor RightUp");
                    if (target.IsRightUpAvailable())
                    {
                        target.RightUp(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor RightUp end!");
        }

        private void ConnectMove(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor Move begin!");

            _moveDisposable = source.Move.Subscribe(
                (args) =>
                {
                    Console.WriteLine("InputProcessor Move");
                    if (target.IsMoveAvailable())
                    {
                        target.Move(args);
                    }
                });

            Console.WriteLine("Connect InputProcessor Move end!");
        }

        private void DisconnectLeftDown()
        {
            Console.WriteLine("DisconnectLeftDown InputProcessor");
            _leftDownDisposable?.Dispose();
        }

        private void DisconnectLeftUp()
        {
            Console.WriteLine("DisconnectLeftUp InputProcessor");
            _leftUpDisposable?.Dispose();
        }

        private void DisconnectRightDown()
        {
            Console.WriteLine("DisconnectRightDown InputProcessor");
            _rightDownDisposable?.Dispose();
        }

        private void DisconnectRightUp()
        {
            Console.WriteLine("DisconnectRightUp InputProcessor");
            _rightUpDisposable.Dispose();
        }

        private void DisconnectMove()
        {
            Console.WriteLine("DisconnectMove InputProcessor");
            _moveDisposable?.Dispose();
        }

        /// <summary>
        /// Connects source and target inputs.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public void Connect(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor Begin!");
            ConnectLeftDown(source, target);
            ConnectLeftUp(source, target);
            ConnectRightDown(source, target);
            ConnectRightUp(source, target);
            ConnectMove(source, target);
            Console.WriteLine("Connect InputProcessor End!");
        }

        /// <summary>
        /// Disconnects source and target inputs.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnect InputProcessor Begin!");
            DisconnectLeftDown();
            DisconnectLeftUp();
            DisconnectRightDown();
            DisconnectRightUp();
            DisconnectMove();
            Console.WriteLine("Disconnect InputProcessor End!");
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Dispose InputProcessor Begin!");
            Disconnect();
            Console.WriteLine("Dispose InputProcessor End!");
        }
    }
}
