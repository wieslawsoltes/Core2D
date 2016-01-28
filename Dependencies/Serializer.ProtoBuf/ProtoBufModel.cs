// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ProtoBuf.Meta;
using Core2D;

namespace Dependencies
{
    public static class ProtoBufModel
    {
        public static RuntimeTypeModel Create()
        {
            var model = TypeModel.Create();
            return model;
        }
    }
}
