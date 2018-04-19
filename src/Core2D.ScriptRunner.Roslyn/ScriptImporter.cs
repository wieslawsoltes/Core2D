// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Spatial;

namespace Core2D.ScriptRunner.Roslyn
{
    /// <summary>
    /// CSharp script importer.
    /// </summary>
    public class ScriptImporter
    {
        /// <summary>
        /// Gets portable references array for script execution context.
        /// </summary>
        /// <returns>The portable references array.</returns>
        public static PortableExecutableReference[] GetReferences()
        {
#if NETCORE5_0
            return new PortableExecutableReference[] { };
#else
            var assemblyPath = System.IO.Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var immutableCollectionsPath = System.IO.Path.GetDirectoryName(typeof(ImmutableArray<>).GetTypeInfo().Assembly.Location);
            var mathSpatialPath = System.IO.Path.GetDirectoryName(typeof(Point2).GetTypeInfo().Assembly.Location);
            var executingPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return new[]
            {
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "mscorlib.dll")),
#if NETSTANDARD2_0
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "netstandard.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
#endif
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(immutableCollectionsPath, "System.Collections.Immutable.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(executingPath, "Core2D.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(executingPath, "Core2D.Editor.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(mathSpatialPath, "Math.Spatial.dll")),
                MetadataReference.CreateFromFile(Assembly.GetEntryAssembly().Location)
            };
#endif
        }

        /// <summary>
        /// Composes C# script class imports.
        /// </summary>
        /// <typeparam name="T">The script class type</typeparam>
        /// <param name="assembly">The compiled assembly.</param>
        /// <returns>The imported script objects collection.</returns>
        public static IEnumerable<T> Compose<T>(Assembly assembly)
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<T>()
                .Export<T>()
                .SelectConstructor(selector => selector.FirstOrDefault());

            var configuration = new ContainerConfiguration()
                .WithAssembly(assembly)
                .WithDefaultConventions(builder);

            using (var container = configuration.CreateContainer())
            {
                return container.GetExports<T>();
            }
        }

        /// <summary>
        /// Imports C# scripts from compiled assembly.
        /// </summary>
        /// <typeparam name="T">The script class type.</typeparam>
        /// <param name="csharp">The C# code.</param>
        /// <returns>The imported script objects collection.</returns>
        public static IEnumerable<T> Import<T>(string csharp)
        {
            var references = GetReferences();
            var syntaxTree = CSharpSyntaxTree.ParseText(csharp);
            var compilation = CSharpCompilation.Create(
                "Imports",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(
                    outputKind: OutputKind.DynamicallyLinkedLibrary,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            using (var ms = new System.IO.MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    Assembly assembly = null;
#if NET461
                    assembly = Assembly.Load(ms.GetBuffer());
#elif NETCOREAPP2_0
                    assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);
#endif
                    if (assembly != null)
                    {
                        return Compose<T>(assembly);
                    }
                }
#if DEBUG
                else
                {
                    Debug.WriteLine("Failed to compile script:");
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Debug.WriteLine(diagnostic);
                    }
                }
#endif
            }
            return null;
        }
    }
}
