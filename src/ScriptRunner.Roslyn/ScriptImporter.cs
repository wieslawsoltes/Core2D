// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ScriptRunner.Roslyn
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
            var assemblyPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var immutableCollectionsPath = Path.GetDirectoryName(typeof(ImmutableArray<>).GetTypeInfo().Assembly.Location);
            var executingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(immutableCollectionsPath, "System.Collections.Immutable.dll")),
                MetadataReference.CreateFromFile(Path.Combine(executingPath, "Core2D.dll")),
                MetadataReference.CreateFromFile(Path.Combine(executingPath, "Core2D.Spatial.dll")),
                MetadataReference.CreateFromFile(Assembly.GetEntryAssembly().Location),
            };
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

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    Assembly assembly = null;
#if NET45
                    assembly = Assembly.Load(ms.GetBuffer());
#elif NETCOREAPP1_0
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
