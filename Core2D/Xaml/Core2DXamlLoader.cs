// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OmniXaml;
using OmniXaml.Services.DotNetFx;

namespace Core2D
{
    public static class Core2DXamlLoader
    {
        public static object Load(string path)
        {
            var wiringContext = WiringContext.FromAttributes(Assemblies.AssembliesInAppFolder);
            var loader = new DefaultXamlLoader(wiringContext);
            return loader.FromPath(path);
        }
    }
}
