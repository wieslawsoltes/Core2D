// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace Test2d
{
    // TODO: Add ImmutableArray support.

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class History<T>
    {
        private ISerializer _serializer = default(ISerializer);
        private ICompressor _compressor = default(ICompressor);
       // private Stack<byte[]> _undos = new Stack<byte[]>();
        //private Stack<byte[]> _redos = new Stack<byte[]>();
        //private byte[] _hold = default(byte[]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="compressor"></param>
        public History(ISerializer serializer, ICompressor compressor)
        {
            _serializer = serializer;
            _compressor = compressor;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Hold(T obj)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Snapshot(T obj)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bson"></param>
        private void Snapshot(byte[] bson)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public T Undo(T current)
        {
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public T Redo(T current)
        {
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return false;
        }
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class History<T>
    {
        private ISerializer _serializer = default(ISerializer);
        private ICompressor _compressor = default(ICompressor);
        private Stack<byte[]> _undos = new Stack<byte[]>();
        private Stack<byte[]> _redos = new Stack<byte[]>();
        private byte[] _hold = default(byte[]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="compressor"></param>
        public History(ISerializer serializer, ICompressor compressor)
        {
            _serializer = serializer;
            _compressor = compressor;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            if (_undos != null && _undos.Count > 0)
            {
                _undos.Clear();
            }

            if (_redos != null && _redos.Count > 0)
            {
                _redos.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Hold(T obj)
        {
            _hold = _compressor.Compress(_serializer.ToBson(obj));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            Snapshot(_hold);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
            _hold = default(byte[]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Snapshot(T obj)
        {
            Snapshot(_compressor.Compress(_serializer.ToBson(obj)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bson"></param>
        private void Snapshot(byte[] bson)
        {
            if (bson != null)
            {
                if (_redos.Count > 0)
                {
                    _redos.Clear();
                }
                _undos.Push(bson);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public T Undo(T current)
        {
            if (CanUndo())
            {
                var bson = _compressor.Compress(_serializer.ToBson(current));
                if (bson != null)
                {
                    _redos.Push(bson);
                    return _serializer.FromBson<T>(_compressor.Decompress(_undos.Pop()));
                }
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public T Redo(T current)
        {
            if (CanRedo())
            {
                var bson = _compressor.Compress(_serializer.ToBson(current));
                if (bson != null)
                {
                    _undos.Push(bson);
                    return _serializer.FromBson<T>(_compressor.Decompress(_redos.Pop()));
                }
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return _undos != null && _undos.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return _redos != null && _redos.Count > 0;
        }
    }
    */
}