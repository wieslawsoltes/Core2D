// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;
using Core2D.Serializer.Xaml;

[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Containers", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Data", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Renderer", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Shapes", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Shapes.Path", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Shapes.Path.Segments", AssemblyName = "Core2D.ViewModels")]
[assembly: XmlnsDefinition(XamlConstants.ViewModelsNamespace, "Core2D.Style", AssemblyName = "Core2D.ViewModels")]

[assembly: XmlnsPrefix(XamlConstants.ModelNamespace, "vm")]
