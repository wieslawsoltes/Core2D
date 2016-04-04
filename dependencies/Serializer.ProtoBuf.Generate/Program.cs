// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using System;
using System.Diagnostics;
using System.IO;

namespace Serializer.ProtoBuf
{
    class Program
    {
        const string Name = "ProtoBufSerializer";

        static void Main(string[] args)
        {
            if (args?.Length == 1)
            {
                Generate(args[0]);
            }
        }

        static void Generate(string path)
        {
            var sw = Stopwatch.StartNew();

            var rtm = ProtoBufModel.ForProject();
            var schema = rtm.GetSchema(typeof(XProject));

            var schemaPath = Path.Combine(path, "Schema", Name + ".proto");
            Console.WriteLine("Writing schema: {0}", schemaPath);
            File.WriteAllText(schemaPath, schema);

            var serializerPath = Path.Combine(path, "Serializer", Name + ".dll");
            Console.WriteLine("Writing serializer: {0}", serializerPath);
            rtm.Compile(Name, Name + ".dll");
            File.Copy(Name + ".dll", serializerPath, true);

            sw.Stop();
            Console.WriteLine("Generate: " + sw.Elapsed.TotalMilliseconds + "ms");
        }
    }
}
