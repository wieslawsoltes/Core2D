// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OmniXaml;
using OmniXaml.Builder;
using OmniXaml.Services.DotNetFx;
using Core2D.Xaml.Converters;

namespace Core2D.Xaml
{
    public static class Core2DXamlLoader
    {
        public static object Load(string path)
        {
            var wiringContext = WiringContext.FromAttributes(Assemblies.AssembliesInAppFolder);

            var featureProvider = (TypeFeatureProvider)wiringContext.FeatureProvider;
            featureProvider.AddTypeConverter(new TypeConverterRegistration(typeof(ArgbColor), new ArgbColorTypeConverter()));
            featureProvider.AddTypeConverter(new TypeConverterRegistration(typeof(FontStyle), new FontStyleTypeConverter()));
            featureProvider.AddTypeConverter(new TypeConverterRegistration(typeof(ShapeState), new ShapeStateTypeConverter()));
            featureProvider.AddTypeConverter(new TypeConverterRegistration(typeof(XPathGeometry), new XPathGeometryTypeConverter()));

            var loader = new DefaultXamlLoader(wiringContext);
            return loader.FromPath(path);
        }
    }
}
