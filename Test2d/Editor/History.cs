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
        private Stack<byte[]> _undos = new Stack<byte[]>();
        private Stack<byte[]> _redos = new Stack<byte[]>();
        private byte[] _hold = default(byte[]);

        public History(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public static byte[] Compress(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream()) 
            {
                using (var cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                {
                    cs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var ms = new System.IO.MemoryStream(data)) 
            {
                using (var cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new System.IO.MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = cs.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
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
            _hold = Compress(_serializer.ToBson(obj));
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
            Snapshot(Compress(_serializer.ToBson(obj)));
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
                var bson = Compress(_serializer.ToBson(current));
                if (bson != null)
                {
                    _redos.Push(bson);
                    return _serializer.FromBson<T>(Decompress(_undos.Pop()));
                }
            }
            return default(T);
        }

        public T Redo(T current)
        {
            if (CanRedo())
            {
                var bson = Compress(_serializer.ToBson(current));
                if (bson != null)
                {
                    _undos.Push(bson);
                    return _serializer.FromBson<T>(Decompress(_redos.Pop()));
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