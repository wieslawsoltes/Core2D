// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public interface ISerializer
    {
        string ToJson<T>(T value);
        byte[] ToBson<T>(T value);
        T FromBson<T>(byte[] bson);
        T FromJson<T>(string json);
    }
}
