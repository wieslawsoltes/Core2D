// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;
using Serializer.Xaml;

[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Collections", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Data", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Data.Database", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.History", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Interfaces", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Path", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Path.Segments", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Project", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Renderer", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Shape", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Shapes", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.CoreNamespace, "Core2D.Style", AssemblyName = "Core2D")]

[assembly: XmlnsPrefix(XamlConstants.CoreNamespace, "c")]

[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Bounds", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Designer", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Factories", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Input", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Interfaces", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Tools", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Views", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Recent", AssemblyName = "Core2D")]

[assembly: XmlnsPrefix(XamlConstants.EditorNamespace, "e")]

[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.Arc", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.ConvexHull", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.DouglasPeucker", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(XamlConstants.SpatialNamespace, "Spatial.Sat", AssemblyName = "Math.Spatial")]

[assembly: XmlnsPrefix(XamlConstants.SpatialNamespace, "s")]
