
if (States[Id] == null)
{
    States[Id] = new Random(Guid.NewGuid().GetHashCode());
}
else
{
    var r = States[Id] as Random;
    Shape.TopLeft.X = r.NextDouble() * 810;
    Shape.TopLeft.Y = r.NextDouble() * 600;
    Shape.BottomRight.X = r.NextDouble() * 810;
    Shape.BottomRight.Y = r.NextDouble() * 600;
}
