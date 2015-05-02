// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestSIM
{
    public class BoolSimulationFactory
    {
        public IDictionary<string, Func<XGroup, BoolSimulation>> Registry { get; private set; }

        public BoolSimulationFactory()
        {
            Registry = new Dictionary<string, Func<XGroup, BoolSimulation>>();
        }

        public static T GetInstance<T>() where T: BoolSimulation
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }

        public bool Register(BoolSimulation simulation)
        {
            if (Registry.ContainsKey(simulation.Key))
            {
                return false;
            }
            Registry.Add(simulation.Key, simulation.Factory);
            return true;
        }

        public void Register(IEnumerable<BoolSimulation> simulations)
        {
            foreach (var simulation in simulations)
            {
                Register(simulation);
            }
        }

        public bool Register(string key, Func<XGroup, BoolSimulation> factory)
        {
            if (Registry.ContainsKey(key))
            {
                return false;
            }
            Registry.Add(key, factory);
            return true;
        }

        private IDictionary<XGroup, BoolSimulation> GetSimulations(PageGraphContext context)
        {
            var simulations = new Dictionary<XGroup, BoolSimulation>();
            foreach (var group in context.OrderedGroups)
            {
                if (Registry.ContainsKey(group.Name))
                {
                    simulations.Add(group, Registry[group.Name](group));
                }
                else
                {
                    throw new Exception("Not supported group simulation.");
                }
            }
            return simulations;
        }

        public IDictionary<XGroup, BoolSimulation> Create(PageGraphContext context)
        {
            var simulations = GetSimulations(context);

            // find ordered group Inputs
            foreach (var group in context.OrderedGroups)
            {
                var inputs = group.Connectors
                    .Where(pin => context.PinTypes[pin].HasFlag(ShapeState.Input))
                    .SelectMany(pin =>
                    {
                        return context.Dependencies[pin]
                            .Where(dep => context.PinTypes[dep.Item1].HasFlag(ShapeState.Output));
                    })
                    .Select(pin => pin);

                // convert inputs to BoolInput
                var simulation = simulations[group];
                simulation.Inputs = inputs.Select(input =>
                {
                    return new BoolInput()
                    {
                        Simulation = simulations[input.Item1.Owner as XGroup],
                        IsInverted = input.Item2
                    };
                }).ToArray();
            }
            return simulations;
        }

        public void Run(IDictionary<XGroup, BoolSimulation> simulations, IClock clock)
        {
            foreach (var simulation in simulations)
            {
                simulation.Value.Run(clock);
            }
        }
    }
}
