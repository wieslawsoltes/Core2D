// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;
using Serializer.Xaml;
using System.Reflection;
using System.Resources;

[assembly: AssemblyTitle("Serializer.Xaml")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Serializer.Xaml")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Collections", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Data", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Data.Database", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Bounds", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Designer", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Factories", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Input", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Interfaces", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Tools", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Views", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Editor.Recent", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.History", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Interfaces", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Math", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Spatial.Arc", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Spatial.ConvexHull", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Spatial.Sat", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Path", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Path.Segments", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Project", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Renderer", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Shape", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Shapes", AssemblyName = "Core2D")]
[assembly: XmlnsDefinition(CoreXamlSchemaContext.CoreNamespace, "Core2D.Style", AssemblyName = "Core2D")]
[assembly: XmlnsPrefix(CoreXamlSchemaContext.CoreNamespace, "c2d")]
