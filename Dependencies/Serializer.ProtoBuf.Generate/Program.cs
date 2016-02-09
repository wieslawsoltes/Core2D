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
        const string SerializerDllPath = @"..\..\Serializer\";
        const string ProtoSchemaPath = @"..\..\Schema\";

        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var rtm = ProtoBufModel.ForProject();
            var schema = rtm.GetSchema(typeof(XProject));
            File.WriteAllText(ProtoSchemaPath + Name + ".proto", schema);
            rtm.Compile(Name, Name + ".dll");
            File.Copy(Name + ".dll", SerializerDllPath + Name + ".dll", true);
            sw.Stop();
            Console.WriteLine("Generate: " + sw.Elapsed.TotalMilliseconds + "ms");
        }
    }
}
