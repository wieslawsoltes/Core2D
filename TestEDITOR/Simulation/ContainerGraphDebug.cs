// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestSIM
{
    /// <summary>
    /// 
    /// </summary>
    public static class ContainerGraphDebug
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void WriteConnections(ContainerGraphContext context)
        {
            Debug.WriteLine("Connections: ");
            foreach (var kvp in context.Connections)
            {
                var pin = kvp.Key;
                var connections = kvp.Value;
                Debug.WriteLine(
                    "{0}:{1}",
                    pin.Owner == null ? "<>" : pin.Owner.Name,
                    pin.Name);
                if (connections != null && connections.Count > 0)
                {
                    foreach (var connection in connections)
                    {
                        Debug.WriteLine(
                            "\t{0}:{1}, inverted: {2}",
                            connection.Point.Owner == null ? "<>" : connection.Point.Owner.Name,
                            connection.Point.Name,
                            connection.IsInverted);
                    }
                }
                else
                {
                    Debug.WriteLine("\t<none>");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void WriteDependencies(ContainerGraphContext context)
        {
            Debug.WriteLine("Dependencies: ");
            foreach (var kvp in context.Dependencies)
            {
                var pin = kvp.Key;
                var dependencies = kvp.Value;
                if (dependencies != null && dependencies.Count > 0)
                {
                    Debug.WriteLine(
                        "{0}:{1}",
                        pin.Owner == null ? "<>" : pin.Owner.Name,
                        pin.Name);
                    foreach (var dependency in dependencies)
                    {
                        Debug.WriteLine(
                            "\t[{0}] {1}:{2}, inverted: {3}",
                            dependency.Point.State,
                            dependency.Point.Owner == null ? "<>" : dependency.Point.Owner.Name,
                            dependency.Point.Name,
                            dependency.IsInverted);
                    }
                }
                else
                {
                    Debug.WriteLine(
                        "{0}:{1}",
                        pin.Owner == null ? "<>" : pin.Owner.Name,
                        pin.Name);
                    Debug.WriteLine("\t<none>");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void WritePinTypes(ContainerGraphContext context)
        {
            Debug.WriteLine("PinTypes: ");
            foreach (var kvp in context.PinTypes)
            {
                var pin = kvp.Key;
                var type = kvp.Value;
                Debug.WriteLine(
                    "\t[{0}] {1}:{2}",
                    type,
                    pin.Owner == null ? "<>" : pin.Owner.Name,
                    pin.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void WriteOrderedGroups(ContainerGraphContext context)
        {
            Debug.WriteLine("OrderedGroups: ");
            foreach (var group in context.OrderedGroups)
            {
                Debug.WriteLine(group.Name);
            }
        }
    }
}
