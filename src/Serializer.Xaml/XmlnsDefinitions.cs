// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;
using Serializer.Xaml;

[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Collections", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Data", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Data.Database", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Interfaces", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Path", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Path.Segments", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Project", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Renderer", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Shape", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Shapes", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Style", AssemblyName = "Core2D")]

[assembly: XmlnsPrefix(CoreXamlSchemaContext.CoreNamespace, "c")]

[assembly: XmlnsDefinition(CoreXamlSchemaContext.SpatialNamespace, "Spatial", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.SpatialNamespace, "Spatial.Arc", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.SpatialNamespace, "Spatial.ConvexHull", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.SpatialNamespace, "Spatial.DouglasPeucker", AssemblyName = "Math.Spatial")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.SpatialNamespace, "Spatial.Sat", AssemblyName = "Math.Spatial")]

[assembly: XmlnsPrefix(CoreXamlSchemaContext.SpatialNamespace, "s")]
