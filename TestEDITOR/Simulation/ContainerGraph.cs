// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestSIM
{
    public static class ContainerGraph
    {
        public static IEnumerable<XPoint> GetLogicPoints(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
            {
                yield break;
            }

            foreach (var shape in shapes)
            {
                if (shape is XPoint)
                {
                    var point = shape as XPoint;
                    yield return shape as XPoint;
                }
                else if (shape is XLine)
                {
                    var line = shape as XLine;
                    if (line.Style.Name.StartsWith("Logic-Wire"))
                    {
                        yield return line.Start;
                        yield return line.End;
                    }
                }
                //else if (shape is XBezier)
                //{
                //    var bezier = shape as XBezier;
                //    if (bezier.Style.Name.StartsWith("Logic-Wire"))
                //    {
                //        yield return bezier.Point1;
                //        yield return bezier.Point4;
                //    }
                //}
                //else if (shape is XQBezier)
                //{
                //    var qbezier = shape as XQBezier;
                //    if (qbezier.Style.Name.StartsWith("Logic-Wire"))
                //    {
                //        yield return qbezier.Point1;
                //        yield return qbezier.Point3;
                //    }
                //}
                else if (shape is XGroup)
                {
                    var group = shape as XGroup;
                    foreach (var point in group.Connectors)
                    {
                        yield return point;
                    }
                }
            }
        }

        public static IEnumerable<XPoint> GetGroupPoints(IEnumerable<XGroup> groups)
        {
            if (groups == null)
            {
                yield break;
            }

            foreach (var group in groups)
            {
                foreach (var point in group.Connectors)
                {
                    yield return point;
                }
            }
        }

        public static PageGraphContext Create(Container container)
        {
            var shapes = container.Layers.SelectMany(l => l.Shapes);

            return Create(
                shapes,
                shapes.Where(s => s is XGroup).Cast<XGroup>(),
                shapes.Where(s => s is XPoint && s.State.HasFlag(ShapeState.Standalone)).Cast<XPoint>(),
                shapes.Where(s => s is XLine && s.Style.Name.StartsWith("Logic-Wire")).Cast<XLine>());
        }

        public static PageGraphContext Create(
            IEnumerable<BaseShape> shapes,
            IEnumerable<XGroup> groups,
            IEnumerable<XPoint> pins,
            IEnumerable<XLine> wires)
        {
            var context = new PageGraphContext();

            context.Connections = FindConnections(shapes, pins, wires);
            //PageGraphDebug.WriteConnections(context);
            context.Dependencies = FindDependencies(groups, context.Connections);
            //PageGraphDebug.WriteDependencies(context);
            context.PinTypes = FindPinTypes(groups, wires, pins, context.Dependencies);
            //PageGraphDebug.WritePinTypes(context);
            context.OrderedGroups = SortDependencies(groups, context.Dependencies, context.PinTypes);
            //PageGraphDebug.WriteOrderedGroups(context);

            return context;
        }

        public static bool IsPinInverted(XLine wire)
        {
            return (wire.Style.LineStyle.StartArrowStyle.ArrowType == ArrowType.Ellipse)
                | (wire.Style.LineStyle.EndArrowStyle.ArrowType == ArrowType.Ellipse);
        }

        public static IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> FindConnections(
            IEnumerable<BaseShape> shapes,
            IEnumerable<XPoint> pins,
            IEnumerable<XLine> wires)
        {
            var connections = new Dictionary<XPoint, ICollection<Tuple<XPoint, bool>>>();

            foreach (var pin in GetLogicPoints(shapes))
            {
                if (!connections.ContainsKey(pin))
                {
                    connections.Add(pin, new HashSet<Tuple<XPoint, bool>>());
                }
            }

            foreach (var wire in wires)
            {
                var startConnections = connections[wire.Start];
                var endConnections = connections[wire.End];
                bool isPinInverted = IsPinInverted(wire);
  
                var et = Tuple.Create(wire.End, isPinInverted);
                if (!startConnections.Contains(et))
                {
                    startConnections.Add(et);
                }
                var st = Tuple.Create(wire.Start, isPinInverted);
                if (!endConnections.Contains(st))
                {
                    endConnections.Add(st);
                }
            }

            return connections;
        }

        public static IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> FindDependencies(
            IEnumerable<XGroup> groups,
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> connections)
        {
            var dependencies = new Dictionary<XPoint, ICollection<Tuple<XPoint, bool>>>();

            foreach (var group in groups)
            {
                foreach (var pin in group.Connectors)
                {
                    dependencies.Add(pin, new HashSet<Tuple<XPoint, bool>>());
                    FindDependencies(pin, pin, connections, dependencies);
                }
            }

            return dependencies;
        }

        public static void FindDependencies(
            XPoint next,
            XPoint start,
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> connections,
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> dependencies)
        {
            var pinConnections = connections[next];
            foreach (var connection in pinConnections)
            {
                if (connection.Item1 == start)
                {
                    continue;
                }

                var pinDependencies = dependencies[start];
                if (!pinDependencies.Contains(connection))
                {
                    if (connection.Item1.State.HasFlag(ShapeState.None)
                        || connection.Item1.State.HasFlag(ShapeState.Input)
                        || connection.Item1.State.HasFlag(ShapeState.Output))
                    {
                        pinDependencies.Add(connection);
                    }
                    else if (connection.Item1.State.HasFlag(ShapeState.Standalone))
                    {
                        pinDependencies.Add(connection);
                        FindDependencies(connection.Item1, start, connections, dependencies);
                    }
                }
            }
        }

        public static IDictionary<XPoint, ShapeState> FindPinTypes(
            IEnumerable<XGroup> groups,
            IEnumerable<XLine> wires,
            IEnumerable<XPoint> pins,
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> dependencies)
        {
            var pinTypes = new Dictionary<XPoint, ShapeState>();
            var pinsWithoutType = new List<XPoint>();

            // use pin dependencies to set pin type to Input or Output
            foreach (var group in groups)
            {
                foreach (var pin in group.Connectors)
                {
                    bool haveKey = pinTypes.ContainsKey(pin);
                    if (!haveKey && pin.State.HasFlag(ShapeState.None))
                    {
                        if (dependencies[pin].Count <= 0)
                        {
                            // nothing is connected
                            pinTypes.Add(pin, ShapeState.None);
                        }
                        else
                        {
                            var pinDependencies = dependencies[pin];
                            int noneDepCount = pinDependencies.Count(p => p.Item1.State.HasFlag(ShapeState.None));
                            int inputDepCount = pinDependencies.Count(p => p.Item1.State.HasFlag(ShapeState.Input));
                            int outputDepCount = pinDependencies.Count(p => p.Item1.State.HasFlag(ShapeState.Output));
                            if (inputDepCount == 0 && outputDepCount > 0 && noneDepCount == 0)
                            {
                                // set as Input
                                pinTypes.Add(pin, ShapeState.Input);
                            }
                            else if (inputDepCount > 0 && outputDepCount == 0 && noneDepCount == 0)
                            {
                                // set as Output
                                pinTypes.Add(pin, ShapeState.Output);

                                foreach (var p in (pin.Owner as XGroup).Connectors.Where(p => p != pin && dependencies[p].Count > 0))
                                {
                                    if (pinTypes.ContainsKey(p))
                                    {
                                        if (pinTypes[p].HasFlag(ShapeState.None))
                                        {
                                            // set as Input
                                            pinTypes[p] = ShapeState.Input;
                                        }
                                    }
                                    else
                                    {
                                        // set as Input
                                        pinTypes.Add(p, ShapeState.Input);
                                    }
                                }
                            } 
                            else if (inputDepCount > 0 && outputDepCount > 0)
                            {
                                // invalid pin connection
                                throw new Exception("Connecting Inputs and Outputs to same Pin is not allowed.");
                            }
                            else
                            {
                                // if no Input or Output is connected
                                pinsWithoutType.Add(pin);
                                pinTypes.Add(pin, ShapeState.None);
                            }
                        }
                    }
                    else
                    {
                        if (!haveKey)
                        {
                            // use pin original type
                            pinTypes.Add(pin, pin.State);
                        }
                    }
                }
            }

            if (pinsWithoutType.Count > 0)
            {
                FindPinTypes(dependencies, pinTypes, pinsWithoutType);
            }

            // standalone pins
            foreach (var pin in pins)
            {
                if (!pinTypes.ContainsKey(pin))
                {
                    pinTypes.Add(pin, pin.State);
                }
            }

            foreach (var wire in wires)
            {
                if (wire.Start.State.HasFlag(ShapeState.Standalone))
                {
                    if (!pinTypes.ContainsKey(wire.Start))
                    {
                        pinTypes.Add(wire.Start, wire.Start.State);
                    }
                }

                if (wire.End.State.HasFlag(ShapeState.Standalone))
                {
                    if (!pinTypes.ContainsKey(wire.End))
                    {
                        pinTypes.Add(wire.End, wire.End.State);
                    }
                }
            }

            return pinTypes;
        }

        private static void FindPinTypes(
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> dependencies,
            Dictionary<XPoint, ShapeState> pinTypes,
            List<XPoint> pins)
        {
            var pinsWithoutType = new List<XPoint>();

            // pins with onnections but do not have Input or Output type set
            foreach (var pin in pins)
            {
                XGroup owner = pin.Owner as XGroup;

                int noneCount = owner.Connectors.Count(p => pinTypes[p].HasFlag(ShapeState.None) && dependencies[p].Count > 0);
                int inputCount = owner.Connectors.Count(p => pinTypes[p].HasFlag(ShapeState.Input));
                int outputCount = owner.Connectors.Count(p => pinTypes[p].HasFlag(ShapeState.Output));

                var pinDependencies = dependencies[pin];
                int noneDepCount = pinDependencies.Count(
                    p => pinTypes.ContainsKey(p.Item1) && pinTypes[p.Item1].HasFlag(ShapeState.None));
                int inputDepCount = pinDependencies.Count(
                    p => pinTypes.ContainsKey(p.Item1) && pinTypes[p.Item1].HasFlag(ShapeState.Input));
                int outputDepCount = pinDependencies.Count(
                    p => pinTypes.ContainsKey(p.Item1) && pinTypes[p.Item1].HasFlag(ShapeState.Output));

                if (inputDepCount == 0 && outputDepCount > 0 && noneDepCount == 0)
                {
                    // set as Input
                    pinTypes[pin] = ShapeState.Input;
                }
                else if (inputDepCount > 0 && outputDepCount == 0 && noneDepCount == 0)
                {
                    // set as Output
                    pinTypes[pin] = ShapeState.Output;

                    foreach (var p in (pin.Owner as XGroup).Connectors.Where(p => p != pin && dependencies[p].Count > 0))
                    {
                        if (pinTypes.ContainsKey(p))
                        {
                            if (pinTypes[p] == ShapeState.None)
                            {
                                // set as Input
                                pinTypes[p] = ShapeState.Input;
                            }
                        }
                        else
                        {
                            // set as Input
                            pinTypes.Add(p, ShapeState.Input);
                        }
                    }
                }
                else
                {
                    if (pinTypes[pin].HasFlag(ShapeState.None))
                    {
                        if (noneCount == 1 && inputCount == 1 && outputCount == 0)
                        {
                            // set as Output
                            pinTypes[pin] = ShapeState.Output;
                        }
                        else
                        {
                            pinsWithoutType.Add(pin);
                        }
                    }
                }
            }

            if (pinsWithoutType.Count > 0 && pins.Count == pinsWithoutType.Count)
            {
                throw new Exception("Can not find pin types.");
            }

            if (pinsWithoutType.Count > 0)
            {
                FindPinTypes(dependencies, pinTypes, pinsWithoutType);
            }
        }

        public static IList<XGroup> SortDependencies(
            IEnumerable<XGroup> groups,
            IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> dependencies,
            IDictionary<XPoint, ShapeState> pinTypes)
        {
            var dict = new Dictionary<XGroup, IList<XGroup>>();

            foreach (var group in groups)
            {
                dict.Add(group, new List<XGroup>());

                foreach (var pin in group.Connectors)
                {
                    var pinDependencies = dependencies[pin]
                        .Where(p => pinTypes[p.Item1].HasFlag(ShapeState.Input));

                    foreach (var dependency in pinDependencies)
                    {
                        dict[group].Add(dependency.Item1.Owner as XGroup);
                    }
                }
            }

            // sort groups using Pins dependencies
            var tsort = new TopologicalSort<XGroup>();
            var sorted = tsort.Sort(
                groups, 
                group => dict[group], 
                false);

            return sorted.Reverse().ToList();
        }
    }
}
