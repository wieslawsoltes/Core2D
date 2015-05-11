// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace Test2d
{
    public class History<T>
    {
        private ISerializer _serializer = default(ISerializer);
        private ICompressor _compressor = default(ICompressor);
        private Stack<byte[]> _undos = new Stack<byte[]>();
        private Stack<byte[]> _redos = new Stack<byte[]>();
        private byte[] _hold = default(byte[]);

        public History(ISerializer serializer, ICompressor compressor)
        {
            _serializer = serializer;
            _compressor = compressor;
        }

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

        public void Hold(T obj)
        {
            _hold = _compressor.Compress(_serializer.ToBson(obj));
        }

        public void Commit()
        {
            Snapshot(_hold);
        }

        public void Release()
        {
            _hold = default(byte[]);
        }

        public void Snapshot(T obj)
        {
            Snapshot(_compressor.Compress(_serializer.ToBson(obj)));
        }

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

        public bool CanUndo()
        {
            return _undos != null && _undos.Count > 0;
        }

        public bool CanRedo()
        {
            return _redos != null && _redos.Count > 0;
        }
    }
}