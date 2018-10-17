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

        private static IDisposable ConnectLeftDown(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor LeftDown begin!");
            return source.LeftDown.Subscribe(OnNext);
            void OnNext(InputArgs args)
            {
                Console.WriteLine("InputProcessor LeftDown");
                if (target.IsLeftDownAvailable())
                {
                    target.LeftDown(args);
                }
            }
        }

        private static IDisposable ConnectLeftUp(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor LeftUp begin!");
            return source.LeftUp.Subscribe(OnNext);
            void OnNext(InputArgs args)
            {
                Console.WriteLine("InputProcessor LeftUp");
                if (target.IsLeftUpAvailable())
                {
                    target.LeftUp(args);
                }
            }
        }

        private static IDisposable ConnectRightDown(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor RightDown begin!");
            return source.RightDown.Subscribe(OnNext);
            void OnNext(InputArgs args)
            {
                Console.WriteLine("InputProcessor RightDown");
                if (target.IsRightDownAvailable())
                {
                    target.RightDown(args);
                }
            }
        }

        private static IDisposable ConnectRightUp(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor RightUp begin!");
            return source.RightUp.Subscribe(OnNext);
            void OnNext(InputArgs args)
            {
                Console.WriteLine("InputProcessor RightUp");
                if (target.IsRightUpAvailable())
                {
                    target.RightUp(args);
                }
            }
        }

        private static IDisposable ConnectMove(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor Move begin!");
            return source.Move.Subscribe(OnNext);
            void OnNext(InputArgs args)
            {
                Console.WriteLine("InputProcessor Move");
                if (target.IsMoveAvailable())
                {
                    target.Move(args);
                }
            }
        }

        private static void DisconnectLeftDown(IDisposable disposable)
        {
            Console.WriteLine("DisconnectLeftDown InputProcessor");
            disposable?.Dispose();
        }

        private static void DisconnectLeftUp(IDisposable disposable)
        {
            Console.WriteLine("DisconnectLeftUp InputProcessor");
            disposable?.Dispose();
        }

        private static void DisconnectRightDown(IDisposable disposable)
        {
            Console.WriteLine("DisconnectRightDown InputProcessor");
            disposable?.Dispose();
        }

        private static void DisconnectRightUp(IDisposable disposable)
        {
            Console.WriteLine("DisconnectRightUp InputProcessor");
            disposable.Dispose();
        }

        private static void DisconnectMove(IDisposable disposable)
        {
            Console.WriteLine("DisconnectMove InputProcessor");
            disposable?.Dispose();
        }

        /// <summary>
        /// Connects source and target inputs.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public void Connect(IInputSource source, IInputTarget target)
        {
            Console.WriteLine("Connect InputProcessor Begin!");
            _leftDownDisposable = ConnectLeftDown(source, target);
            _leftUpDisposable = ConnectLeftUp(source, target);
            _rightDownDisposable = ConnectRightDown(source, target);
            _rightUpDisposable = ConnectRightUp(source, target);
            _moveDisposable = ConnectMove(source, target);
            Console.WriteLine("Connect InputProcessor End!");
        }

        /// <summary>
        /// Disconnects source and target inputs.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnect InputProcessor Begin!");
            DisconnectLeftDown(_leftDownDisposable);
            DisconnectLeftUp(_leftUpDisposable);
            DisconnectRightDown(_rightDownDisposable);
            DisconnectRightUp(_rightUpDisposable);
            DisconnectMove(_moveDisposable);
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
