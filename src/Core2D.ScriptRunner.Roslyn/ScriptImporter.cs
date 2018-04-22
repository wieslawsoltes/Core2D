// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Convention;
using System.Composition.Hosting;
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
            var objectAssemblyDir = System.IO.Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var entryAssemblyPath = Assembly.GetEntryAssembly().Location;
            var entryAssemblyDir = System.IO.Path.GetDirectoryName(entryAssemblyPath);
            var mathSpatialDir = System.IO.Path.GetDirectoryName(typeof(Point2).GetTypeInfo().Assembly.Location);
            var immutableCollectionsDir = System.IO.Path.GetDirectoryName(typeof(ImmutableArray<>).GetTypeInfo().Assembly.Location);

            Console.WriteLine($"{nameof(objectAssemblyDir)}: {objectAssemblyDir}");
            Console.WriteLine($"{nameof(entryAssemblyPath)}: {entryAssemblyPath}");
            Console.WriteLine($"{nameof(entryAssemblyDir)}: {entryAssemblyDir}");
            Console.WriteLine($"{nameof(mathSpatialDir)}: {mathSpatialDir}");
            Console.WriteLine($"{nameof(immutableCollectionsDir)}: {immutableCollectionsDir}");

            var references = new List<PortableExecutableReference>();

            if (objectAssemblyDir != null)
            {
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "mscorlib.dll")));
#if NETSTANDARD2_0
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "netstandard.dll")));
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "System.Private.CoreLib.dll")));
#endif
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "System.dll")));
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "System.Core.dll")));
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(objectAssemblyDir, "System.Runtime.dll")));
            }

            if (entryAssemblyDir != null)
            {
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(entryAssemblyDir, "Core2D.dll")));
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(entryAssemblyDir, "Core2D.Editor.dll")));
            }

            if (mathSpatialDir != null)
            {
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(mathSpatialDir, "Math.Spatial.dll")));
            }

            if (immutableCollectionsDir != null)
            {
                references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(immutableCollectionsDir, "System.Collections.Immutable.dll")));
            }

            if (entryAssemblyPath != null)
            {
                references.Add(MetadataReference.CreateFromFile(entryAssemblyPath));
            }

            return references.ToArray();
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

                foreach (var diagnostic in result.Diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }

                if (result.Success)
                {
                    var assembly = Assembly.Load(ms.GetBuffer());
                    if (assembly != null)
                    {
                        return Compose<T>(assembly);
                    }
                }
            }
            return null;
        }
    }
}
