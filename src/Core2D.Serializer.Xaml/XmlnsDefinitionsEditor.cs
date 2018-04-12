// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;
using Core2D.Serializer.Xaml;

[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Bounds", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Designer", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Factories", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Input", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Interfaces", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Tools", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Views", AssemblyName = "Core2D.Editor")]
[assembly: XmlnsDefinition(XamlConstants.EditorNamespace, "Core2D.Editor.Recent", AssemblyName = "Core2D.Editor")]

[assembly: XmlnsPrefix(XamlConstants.EditorNamespace, "e")]
