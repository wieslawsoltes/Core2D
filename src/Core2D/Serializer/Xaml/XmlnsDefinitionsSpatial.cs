// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Serializer.Xaml;
using Portable.Xaml.Markup;

[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.Arc", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.ConvexHull", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.DouglasPeucker", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.Sat", AssemblyName = "Math.Spatial")]

[assembly: XmlnsPrefix(XamlConstants.SpatialNamespace, "s")]
