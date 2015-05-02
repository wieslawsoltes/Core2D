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
                else if (shape is XBezier)
                {
                    var bezier = shape as XBezier;
                    if (bezier.Style.Name.StartsWith("Logic-Wire"))
                    {
                        yield return bezier.Point1;
                        yield return bezier.Point4;
                    }
                }
                else if (shape is XQBezier)
                {
                    var qbezier = shape as XQBezier;
                    if (qbezier.Style.Name.StartsWith("Logic-Wire"))
                    {
                        yield return qbezier.Point1;
                        yield return qbezier.Point3;
                    }
                }
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

        public static bool IsValidWireType(BaseShape shape)
        {
            return (shape is XLine 
                || shape is XBezier 
                || shape is XQBezier)
                && shape.Style.Name.StartsWith("Logic-Wire");
        }

        public static bool IsPinInverted(ShapeStyle style)
        {
            var ls = style.LineStyle;
            return (ls.StartArrowStyle.ArrowType == ArrowType.Ellipse)
                | (ls.EndArrowStyle.ArrowType == ArrowType.Ellipse);
        }

        public static bool IsPinStandalone(BaseShape shape)
        {
            return shape is XPoint
                && shape.State.HasFlag(ShapeState.Standalone);
        }

        public static ContainerGraphContext Create(Container container)
        {
            var shapes = container.Layers.SelectMany(l => l.Shapes);
            var groups = shapes.Where(s => s is XGroup).Cast<XGroup>();
            var pins = shapes.Where(s => IsPinStandalone(s)).Cast<XPoint>();
            var wires = shapes.Where(s => IsValidWireType(s));
            return Create(shapes, groups, pins, wires);
        }

        public static ContainerGraphContext Create(
            IEnumerable<BaseShape> shapes,
            IEnumerable<XGroup> groups,
            IEnumerable<XPoint> pins,
            IEnumerable<BaseShape> wires)
        {
            var context = new ContainerGraphContext();

            context.Connections = CreateConnectios(
                shapes, 
                pins, 
                wires);

            context.Dependencies = FindDependencies(
                groups, 
                context.Connections);

            context.PinTypes = FindConnectorPinTypes(
                groups, 
                context.Dependencies);

            AddStandalonePinTypes(
                wires, 
                pins, 
                context.PinTypes);

            context.OrderedGroups = SortDependencies(
                groups, 
                context.Dependencies, 
                context.PinTypes);

#if DEBUG
            ContainerGraphDebug.WriteConnections(context);
            ContainerGraphDebug.WriteDependencies(context);
            ContainerGraphDebug.WritePinTypes(context);
            ContainerGraphDebug.WriteOrderedGroups(context);
#endif
            return context;
        }

        public static void SetWireConnections(
            XPoint p1, 
            XPoint p2, 
            ShapeStyle style, 
            IDictionary<XPoint, ICollection<Pin>> connections)
        {
            var sc = connections[p1];
            var ec = connections[p2];
            bool isPinInverted = IsPinInverted(style);

            var pe = Pin.Create(p2, isPinInverted);
            if (!sc.Contains(pe))
            {
                sc.Add(pe);
            }

            var ps = Pin.Create(p1, isPinInverted);
            if (!ec.Contains(ps))
            {
                ec.Add(ps);
            }
        }

        public static IDictionary<XPoint, ICollection<Pin>> CreateConnectios(
            IEnumerable<BaseShape> shapes,
            IEnumerable<XPoint> pins,
            IEnumerable<BaseShape> wires)
        {
            var connections = new Dictionary<XPoint, ICollection<Pin>>();

            foreach (var pin in GetLogicPoints(shapes))
            {
                if (!connections.ContainsKey(pin))
                {
                    connections.Add(pin, new HashSet<Pin>());
                }
            }

            foreach (var wire in wires)
            {
                if (wire is XLine)
                {
                    var line = wire as XLine;
                    SetWireConnections(
                        line.Start, 
                        line.End, 
                        line.Style, 
                        connections);
                }
                else if (wire is XBezier)
                {
                    var bezier = wire as XBezier;
                    SetWireConnections(
                        bezier.Point1, 
                        bezier.Point4, 
                        bezier.Style, 
                        connections);
                }
                else if (wire is XQBezier)
                {
                    var qbezier = wire as XQBezier;
                    SetWireConnections(
                        qbezier.Point1, 
                        qbezier.Point3, 
                        qbezier.Style, 
                        connections);
                }
                else
                {
                    throw new Exception(
                        "Invalid wire type. Supported wire types: XLine, XBezier and XQBezier.");
                }
            }

            return connections;
        }

        public static IDictionary<XPoint, ICollection<Pin>> FindDependencies(
            IEnumerable<XGroup> groups,
            IDictionary<XPoint, ICollection<Pin>> connections)
        {
            var dependencies = new Dictionary<XPoint, ICollection<Pin>>();

            foreach (var group in groups)
            {
                foreach (var connector in group.Connectors)
                {
                    dependencies.Add(connector, new HashSet<Pin>());
                    FindDependencies(connector, connector, connections, dependencies);
                }
            }

            return dependencies;
        }

        public static void FindDependencies(
            XPoint next,
            XPoint start,
            IDictionary<XPoint, ICollection<Pin>> connections,
            IDictionary<XPoint, ICollection<Pin>> dependencies)
        {
            var pinConnections = connections[next];
            foreach (var connection in pinConnections)
            {
                if (connection.Point == start)
                {
                    continue;
                }

                var dep = dependencies[start];
                if (!dep.Contains(connection))
                {
                    if (connection.Point.State.HasFlag(ShapeState.None)
                        || connection.Point.State.HasFlag(ShapeState.Input)
                        || connection.Point.State.HasFlag(ShapeState.Output))
                    {
                        dep.Add(connection);
                    }
                    else if (connection.Point.State.HasFlag(ShapeState.Standalone))
                    {
                        dep.Add(connection);
                        FindDependencies(connection.Point, start, connections, dependencies);
                    }
                }
            }
        }

        public static IDictionary<XPoint, ShapeState> FindConnectorPinTypes(
            IEnumerable<XGroup> groups,
            IDictionary<XPoint, ICollection<Pin>> dependencies)
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
                            var dep = dependencies[pin];
                            int noneDepCount = dep.Count(p => p.Point.State.HasFlag(ShapeState.None));
                            int inputDepCount = dep.Count(p => p.Point.State.HasFlag(ShapeState.Input));
                            int outputDepCount = dep.Count(p => p.Point.State.HasFlag(ShapeState.Output));

                            if (inputDepCount == 0 && outputDepCount > 0 && noneDepCount == 0)
                            {
                                // set as Input
                                pinTypes.Add(pin, ShapeState.Input);
                            }
                            else if (inputDepCount > 0 && outputDepCount == 0 && noneDepCount == 0)
                            {
                                // set as Output
                                pinTypes.Add(pin, ShapeState.Output);

                                var ownerDep = (pin.Owner as XGroup).Connectors.Where(p => p != pin && dependencies[p].Count > 0);

                                foreach (var p in ownerDep)
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
                                throw new Exception(
                                    "Connecting Inputs and Outputs to same Pin is not allowed.");
                            }
                            else
                            {
                                // if no Input or Output is connected set as None
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
                FindConnectorPinTypes(
                    dependencies, 
                    pinTypes, 
                    pinsWithoutType);
            }

            return pinTypes;
        }

        private static void AddStandalonePinTypes(
            IEnumerable<BaseShape> wires, 
            IEnumerable<XPoint> pins, 
            IDictionary<XPoint, ShapeState> pinTypes)
        {
            // add standalone pins
            foreach (var pin in pins)
            {
                if (!pinTypes.ContainsKey(pin))
                {
                    pinTypes.Add(pin, pin.State);
                }
            }

            // add standalone pins in wires
            foreach (var wire in wires)
            {
                if (wire is XLine)
                {
                    var line = wire as XLine;

                    if (line.Start.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(line.Start))
                        {
                            pinTypes.Add(line.Start, line.Start.State);
                        }
                    }

                    if (line.End.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(line.End))
                        {
                            pinTypes.Add(line.End, line.End.State);
                        }
                    }
                }
                else if (wire is XBezier)
                {
                    var bezier = wire as XBezier;

                    if (bezier.Point1.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(bezier.Point1))
                        {
                            pinTypes.Add(bezier.Point1, bezier.Point1.State);
                        }
                    }

                    if (bezier.Point4.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(bezier.Point4))
                        {
                            pinTypes.Add(bezier.Point4, bezier.Point4.State);
                        }
                    }
                }
                else if (wire is XQBezier)
                {
                    var qbezier = wire as XQBezier;

                    if (qbezier.Point1.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(qbezier.Point1))
                        {
                            pinTypes.Add(qbezier.Point1, qbezier.Point1.State);
                        }
                    }

                    if (qbezier.Point3.State.HasFlag(ShapeState.Standalone))
                    {
                        if (!pinTypes.ContainsKey(qbezier.Point3))
                        {
                            pinTypes.Add(qbezier.Point3, qbezier.Point3.State);
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        "Invalid wire type. Supported wire types: XLine, XBezier and XQBezier.");
                }
            }
        }

        private static void FindConnectorPinTypes(
            IDictionary<XPoint, ICollection<Pin>> dependencies,
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
                var dep = dependencies[pin];
                int noneDepCount = dep.Count(
                    p => pinTypes.ContainsKey(p.Point) && pinTypes[p.Point].HasFlag(ShapeState.None));
                int inputDepCount = dep.Count(
                    p => pinTypes.ContainsKey(p.Point) && pinTypes[p.Point].HasFlag(ShapeState.Input));
                int outputDepCount = dep.Count(
                    p => pinTypes.ContainsKey(p.Point) && pinTypes[p.Point].HasFlag(ShapeState.Output));

                if (inputDepCount == 0 && outputDepCount > 0 && noneDepCount == 0)
                {
                    // set as Input
                    pinTypes[pin] = ShapeState.Input;
                }
                else if (inputDepCount > 0 && outputDepCount == 0 && noneDepCount == 0)
                {
                    // set as Output
                    pinTypes[pin] = ShapeState.Output;

                    var ownerDep = (pin.Owner as XGroup).Connectors.Where(p => p != pin && dependencies[p].Count > 0);

                    foreach (var p in ownerDep)
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
                FindConnectorPinTypes(dependencies, pinTypes, pinsWithoutType);
            }
        }

        public static IList<XGroup> SortDependencies(
            IEnumerable<XGroup> groups,
            IDictionary<XPoint, ICollection<Pin>> dependencies,
            IDictionary<XPoint, ShapeState> pinTypes)
        {
            var dict = new Dictionary<XGroup, IList<XGroup>>();

            foreach (var group in groups)
            {
                dict.Add(group, new List<XGroup>());

                foreach (var pin in group.Connectors)
                {
                    var dep = dependencies[pin].Where(p => pinTypes[p.Point].HasFlag(ShapeState.Input));

                    foreach (var dependency in dep)
                    {
                        dict[group].Add(dependency.Point.Owner as XGroup);
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
