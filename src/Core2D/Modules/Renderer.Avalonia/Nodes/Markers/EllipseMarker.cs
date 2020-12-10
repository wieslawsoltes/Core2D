namespace Core2D.Renderer
{
    internal class EllipseMarker : Marker
    {
        public Avalonia.Media.EllipseGeometry EllipseGeometry { get; set; }

        public override void Draw(object dc)
        {
            var context = dc as Avalonia.Media.DrawingContext;

            using var rotationDisposable = context.PushPreTransform(Rotation);

            context.DrawGeometry(ShapeViewModel.IsFilled ? Brush : null, ShapeViewModel.IsStroked ? Pen : null, EllipseGeometry);
        }
    }
}
