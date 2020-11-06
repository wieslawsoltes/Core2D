namespace Core2D.Renderer
{
    internal class EllipseMarker : Marker
    {
        public Avalonia.Media.EllipseGeometry EllipseGeometry { get; set; }

        public override void Draw(object dc)
        {
            var context = dc as Avalonia.Media.DrawingContext;

            using var rotationDisposable = context.PushPreTransform(Rotation);

            context.DrawGeometry(Shape.IsFilled ? Brush : null, Shape.IsStroked ? Pen : null, EllipseGeometry);
        }
    }
}
