// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using Dxf;
using Test2d;
using TestSIM;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ScriptOptions GetOptions()
        {
            var path = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location);

            ScriptOptions options = ScriptOptions.Default
                .AddReferences(MetadataReference.CreateFromFile(System.IO.Path.Combine(path, "mscorlib.dll")))
                .AddReferences(MetadataReference.CreateFromFile(System.IO.Path.Combine(path, "System.dll")))
                .AddReferences(MetadataReference.CreateFromFile(System.IO.Path.Combine(path, "System.Core.dll")))
                .AddReferences(MetadataReference.CreateFromFile(System.IO.Path.Combine(path, "System.Runtime.dll")))
                .AddNamespaces("System")
                .AddNamespaces("System.Collections.Generic")
                .AddNamespaces("System.Text")
                .AddNamespaces("System.Threading")
                .AddNamespaces("System.Threading.Tasks")
                .AddReferences(Assembly.GetAssembly(typeof(ObservableCollection<>)))
                .AddNamespaces("System.Collections.ObjectModel")
                .AddReferences(Assembly.GetAssembly(typeof(ImmutableArray<>)))
                .AddNamespaces("System.Collections.Immutable")
                .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                .AddNamespaces("System.Linq")
                // Core
                .AddReferences(Assembly.GetAssembly(typeof(ObservableObject)))
                .AddNamespaces("Test2d")
                // Interfaces
                .AddReferences(Assembly.GetAssembly(typeof(IView)))
                // Math
                .AddReferences(Assembly.GetAssembly(typeof(Vector2)))
                // Editor
                .AddReferences(Assembly.GetAssembly(typeof(Editor)))
                // Simulation
                .AddReferences(Assembly.GetAssembly(typeof(Clock)))
                .AddNamespaces("TestSIM")
                // Dxf
                .AddReferences(Assembly.GetAssembly(typeof(DxfObject)))
                .AddNamespaces("Dxf");
            return options;
        }
    }
}
