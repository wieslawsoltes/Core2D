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
    /// <summary>
    /// 
    /// </summary>
    public class BoolSimulationFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, Func<XGroup, BoolSimulation>> Registry { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BoolSimulationFactory()
        {
            Registry = new Dictionary<string, Func<XGroup, BoolSimulation>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T: BoolSimulation
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public bool Register(BoolSimulation simulation)
        {
            if (Registry.ContainsKey(simulation.Key))
            {
                return false;
            }
            Registry.Add(simulation.Key, simulation.Factory);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulations"></param>
        public void Register(IEnumerable<BoolSimulation> simulations)
        {
            foreach (var simulation in simulations)
            {
                Register(simulation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public bool Register(string key, Func<XGroup, BoolSimulation> factory)
        {
            if (Registry.ContainsKey(key))
            {
                return false;
            }
            Registry.Add(key, factory);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IDictionary<XGroup, BoolSimulation> GetSimulations(ContainerGraphContext context)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IDictionary<XGroup, BoolSimulation> Create(ContainerGraphContext context)
        {
            var simulations = GetSimulations(context);

            // find ordered group Inputs
            foreach (var group in context.OrderedGroups)
            {
                var inputs = group.Connectors
                    .Where(pin => context.PinTypes[pin].HasFlag(ShapeState.Input))
                    .SelectMany(
                        pin =>
                        {
                            return context.Dependencies[pin]
                                .Where(dep => context.PinTypes[dep.Point].HasFlag(ShapeState.Output));
                        })
                        .Select(pin => pin);

                // convert inputs to BoolInput
                var simulation = simulations[group];
                simulation.Inputs = inputs.Select(
                    input =>
                    {
                        return new BoolInput()
                        {
                            Simulation = simulations[input.Point.Owner as XGroup],
                            IsInverted = input.IsInverted
                        };
                    }).ToArray();
            }
            return simulations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulations"></param>
        /// <param name="clock"></param>
        public void Run(IDictionary<XGroup, BoolSimulation> simulations, IClock clock)
        {
            foreach (var simulation in simulations)
            {
                simulation.Value.Run(clock);
            }
        }
    }
}
