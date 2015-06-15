
if (States[Id] == null)
{
    try
    {
        var shapes = Context.Editor.Project.Documents
            .SelectMany(d => d.Containers)
            .SelectMany(c => c.Layers)
            .SelectMany(l => l.Shapes);
        if (shapes != null)
        {
            var graph = ContainerGraph.Create(shapes);
            if (graph != null)
            {
                var factory = new BoolSimulationFactory();
                factory.Register(new SignalSimulation());
                factory.Register(new InputSimulation());
                factory.Register(new OutputSimulation());
                factory.Register(new ShortcutSimulation());
                factory.Register(new AndSimulation());
                factory.Register(new OrSimulation());
                factory.Register(new InverterSimulation());
                factory.Register(new XorSimulation());
                factory.Register(new TimerOnSimulation());
                factory.Register(new TimerOffSimulation());
                factory.Register(new TimerPulseSimulation());
                factory.Register(new MemoryResetPriorityVSimulation());
                factory.Register(new MemorySetPriorityVSimulation());
                factory.Register(new MemoryResetPrioritySimulation());
                factory.Register(new MemorySetPrioritySimulation());

                var simulations = factory.Create(graph);
                if (simulations != null)
                {
                    States[Id] = Tuple.Create(factory, simulations);
                }
            }
        }
    }
    catch (Exception ex)
    {
        if (Context.Editor.Log != null)
        {
            Context.Editor.Log.LogError("{0}{1}{2}",
                ex.Message,
                Environment.NewLine,
                ex.StackTrace);
        }
    }
}
else
{
    try
    {
        if (States[Id] != null)
        {
            var tuple = States[Id] as Tuple<BoolSimulationFactory, IDictionary<XGroup, BoolSimulation>>;
            var factory = tuple.Item1;
            var simulations = tuple.Item2;
            factory.Run(simulations, Context.Clock);
        }

        Shape.Text = Context.Clock.Cycle.ToString();
    }
    catch (Exception ex)
    {
        if (Context.Editor.Log != null)
        {
            Context.Editor.Log.LogError("{0}{1}{2}",
                ex.Message,
                Environment.NewLine,
                ex.StackTrace);
        }
    }
}
