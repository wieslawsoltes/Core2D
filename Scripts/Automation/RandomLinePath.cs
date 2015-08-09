var rand = new Random(Guid.NewGuid().GetHashCode());
var width = Container.Width;
var height = Container.Height;
Context.OnToolPath();
Context.OnToolLine();
for (int i = 0; i < 10; i++)
    Editor.LeftDown(rand.NextDouble() * width,rand.NextDouble() * height);
Editor.RightDown(0,0);