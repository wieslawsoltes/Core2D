
void Animate(int delay, int count)
{
    var c = Context.Editor.Container;
    var rand = new Random(Guid.NewGuid().GetHashCode());
    var s = c.CurrentStyleGroup.CurrentStyle;
    double width = c.Width;
    double height = c.Height;

    Task.Run(() => 
    { 
        for (int i = 0; i < count; i++)
        {
            var line = XLine.Create(
                rand.NextDouble() * width, 
                rand.NextDouble() * height, 
                rand.NextDouble() * width, 
                rand.NextDouble() * height, 
                s, c.PointShape);
            Execute(() => c.CurrentLayer.Shapes.Add(line));
            Execute(() => c.CurrentLayer.Invalidate());
            Thread.Sleep(delay);
        }
    });
}

var delay = (int)(1000.0 / 10.0);
var count = 100;
Animate(delay, count);