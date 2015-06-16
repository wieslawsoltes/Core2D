BoolSimulationFactory factory;
IDictionary<XGroup, BoolSimulation> simulations;

void Init(EditorContext context)
{
    try
    {
        var shapes = context.Editor.Project.Documents
            .SelectMany(d => d.Containers)
            .SelectMany(c => c.Layers)
            .SelectMany(l => l.Shapes);
        if (shapes != null)
        {
            var graph = ContainerGraph.Create(shapes);
            if (graph != null)
            {
                factory = new BoolSimulationFactory();
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

                simulations = factory.Create(graph);
            }
        }
    }
    catch (Exception ex)
    {
        if (context.Editor.Log != null)
        {
            context.Editor.Log.LogError("{0}{1}{2}",
                ex.Message,
                Environment.NewLine,
                ex.StackTrace);
        }
    }
}

void Run(EditorContext context, XText shape)
{
    try
    {
        if (simulations != null)
        {
            factory.Run(simulations, context.Clock);
        }

        shape.Text = context.Clock.Cycle.ToString();
    }
    catch (Exception ex)
    {
        if (context.Editor.Log != null)
        {
            context.Editor.Log.LogError("{0}{1}{2}",
                ex.Message,
                Environment.NewLine,
                ex.StackTrace);
        }
    }
}